using ServerClientCommunication;
using UiElements;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Infrastructure;
using UnityEngine.UI;
using Chat;
using DraggableImages;
using Popups;

namespace Screens
{
    public enum PollPhase
    {
        EditorOnlyWaitingForData,
        Spectate,
        WaitingForAnswer,
        WaitingForResults,
        Results,
        ExitingPoll
    }

    /// <summary>
    /// Manages entire poll Ui flow and syncs between all relevant components.
    /// </summary>
    public class PollScreenManager : MonoBehaviour
    {
        [Header("Poll Options")]
        [SerializeField] private RectTransform _pollOptionsContainer;
        [SerializeField] private PollOptionPosition[] _pollOptionPositions;
        [SerializeField] private PollOptionUi _pollOptionUiPrefab;

        [Header("Poll Results")]
        [SerializeField] private PollOptionResultUi _optionResultUiPrefab;
        [SerializeField] private PollResultsCallToAction _resultsCallToAction;
        [SerializeField] private RectTransform _pollResultsContainer;
        
        [Header("Popups")]
        [SerializeField] private AddImageToAvatarInventoryPopup _addImageToAvatarInventoryPopup;
        [SerializeField] private GenericMessagePopup _genericMessagePopup;

        [Header("Special Features")]
        [SerializeField] private ChatMessagesDisplayer _chatMessagesDisplayer;
        [SerializeField] private GalleryImagesController _galleryImagesController;
        [SerializeField] private StickersInventoryUi _stickersInventory;

        [Header("Misc")]
        [SerializeField] private PollTopUi _topUi;
        [SerializeField] private PollBottomBar _bottomBar;
        [SerializeField] private DrawingController _drawingController;
        [SerializeField] private RectTransform _rootContainer;
        private Dictionary<int, List<PollOptionPosition>> _optionPositionsByOptionsCount;
        private int? _lastSelectedId;
        private List<PollOptionUi> _pollOptionsUi;
        private List<PollOptionResultUi> _pollOptionResultsUi;
        private PollPhase _pollPhase;
        private bool _isDrawingEnabled;
        private EnterPollResponseData PollProperties => ClientServices.Instance.PollStore.CurrentPollProperties;
        private PollResultsResponseData PollResults => ClientServices.Instance.PollStore.CurrentPollResults;
        private PollPhase GetPollPhase() => _pollPhase;

        private void Awake()
        {
            // Initialize possible option positions
            _optionPositionsByOptionsCount = new();
            for (var i = 0; i < _pollOptionPositions.Length; i++) 
            {
                var optionPosition = _pollOptionPositions[i];
                var optionsCount = GetOptionCountByCategory(optionPosition.Category);
                if (_optionPositionsByOptionsCount.ContainsKey(optionsCount) == false)
                {
                    _optionPositionsByOptionsCount[optionsCount] = new();
                }
                _optionPositionsByOptionsCount[optionsCount].Add(optionPosition);
            }

            // Create button listeres
            _bottomBar.OnStartPollButtonClicked += StartPollButtonClickedCallback;
            _bottomBar.OnDrawingButtonClicked += DrawingButtonClickedCallback;
            _bottomBar.OnGalleryButtonClicked += GalleryButtonClickedCallback;
            _bottomBar.OnStickersButtonClicked += StickersButtonClickedCallback;
            _resultsCallToAction.OnExitClicked += ExitPollClickedCallback;

            // Clean up mock poll results
            var children = _pollResultsContainer.GetComponentsInChildren<PollOptionResultUi>();
            for (var i = 0; i < children.Length; i++)
            {
                var child = children[i];
                Destroy(child.gameObject);
            }

            _stickersInventory.gameObject.SetActive(true);
            _galleryImagesController.Initialize(GetPollPhase);
        }

        private void OnDestroy()
        {
            FakeServerLink.Instance.OnPollResultsDataReceived -= HandlePollResultsReceived;
        }

        private void Start()
        {
            // Listen for relevant server responses
            FakeServerLink.Instance.OnPollResultsDataReceived += HandlePollResultsReceived;
#if UNITY_EDITOR
            SetPollPhase(PollPhase.EditorOnlyWaitingForData);
            if (PollProperties == null)
            {
                // Required only when running this scene directly
                FakeServerLink.Instance.SendRequestToServer(new EnterPollRequest(), null);
                StartCoroutine(DisplayPollWhenReady());
                return;
            }
#endif
            SetPollPhase(PollPhase.Spectate);
            DisplayPoll();
        }

#if UNITY_EDITOR
        private IEnumerator DisplayPollWhenReady()
        {
            while (PollProperties == null)
            {
                yield return null;
            }
            SetPollPhase(PollPhase.Spectate);
            DisplayPoll();
        }
#endif

        private int GetOptionCountByCategory(PollOptionPositionCategory category) 
        {
            switch (category)
            {
                case PollOptionPositionCategory.TwoOptions:
                    return 2;
                case PollOptionPositionCategory.ThreeOptions:
                    return 3;
            }
            throw new Exception($"Unhandled {nameof(PollOptionPositionCategory)} {category}!");
        }

        private void SetPollPhase(PollPhase pollPhase)
        {
            _pollPhase = pollPhase;
            if (_pollPhase == PollPhase.EditorOnlyWaitingForData)
            {
                _chatMessagesDisplayer.Deactivate();
                _resultsCallToAction.Deactivate();
            }
            else if (_pollPhase == PollPhase.Spectate)
            {
                _chatMessagesDisplayer.Activate();
                _resultsCallToAction.Deactivate();
            }
            else if (_pollPhase == PollPhase.Results)
            {
                _drawingController.SetIsEnabled(false);
                _drawingController.Clear();
                _chatMessagesDisplayer.Deactivate();
                _resultsCallToAction.Activate();
                _galleryImagesController.HideAll();
                _stickersInventory.HideAll();
                _stickersInventory.Deactivate();
            }
            _bottomBar.RefreshUi(_pollPhase);
            _bottomBar.RefreshDrawingButtonSprite(_isDrawingEnabled);
            _topUi.UpdateUi(_pollPhase);
        }

        /// <summary>
        /// Displays a new poll once the data from the server is received.
        /// </summary>
        public void DisplayPoll()
        {
            _pollOptionsUi = new();
            var optionsCount = PollProperties.OptionsData.Count;
            for (var i = 0; i < _optionPositionsByOptionsCount[optionsCount].Count; i++) 
            {
                var optionPosition = _optionPositionsByOptionsCount[optionsCount][i];
                var newOptionResultUi = Instantiate(_pollOptionUiPrefab, _pollOptionsContainer);
                var optionData = PollProperties.OptionsData[i];
                newOptionResultUi.RootRectTransform.position = optionPosition.transform.position;
                newOptionResultUi.Initialize(new PollOptionUiInitializeData()
                {
                    Id = i,
                    Title = optionData.Title,
                    ImageUrl = optionData.ImageUrl
                });
                newOptionResultUi.OnClicked += OptionClickedCallback;
                _pollOptionsUi.Add(newOptionResultUi);
            }
        }

        private void HandlePollResultsReceived()
        {
            SetPollPhase(PollPhase.Results);
            var winningOptionIndex = 0;
            var highestResult01 = 0f;
            for (var i = 0; i < PollResults.ResultsData.Count; i++)
            {
                var resultData = PollResults.ResultsData[i];
                if (highestResult01 < resultData.Result01)
                {
                    highestResult01 = resultData.Result01;
                    winningOptionIndex = i;
                }
            }

            _pollOptionResultsUi = new();
            for (var i = 0; i < PollProperties.OptionsData.Count; i++)
            {
                var newOptionResultUi = ObjectPool.Instance.Borrow(_optionResultUiPrefab, _pollResultsContainer).GetComponent<PollOptionResultUi>();
                _pollOptionResultsUi.Add(newOptionResultUi);
            }

            // Forcing horizontal layout group to rebuild in order to measure the delta between the option images
            // in the OptionUi components to their corresponding OptionResultUi components.
            LayoutRebuilder.ForceRebuildLayoutImmediate(_pollResultsContainer);

            for (var i = 0; i < _pollOptionResultsUi.Count; i++) 
            {
                var optionUi = _pollOptionsUi[i];
                optionUi.AnimateToFullTransparency();
                var optionData = PollProperties.OptionsData[i];
                var resultData = PollResults.ResultsData[i];
                var isUserChoice = _lastSelectedId == null ?
                    false : _lastSelectedId.Value == i;
                var isWinningOption = winningOptionIndex == i;
                var newOptionResultUi = _pollOptionResultsUi[i];
                var positionDeltaFromOptionToResult = optionUi.ImageRectPosition - newOptionResultUi.RectTransform.position;
                if (isUserChoice)
                {
                    newOptionResultUi.OnAddAvatarImageClicked += AddImageToAvatarInventoryClickedCallback;
                }
                var uiData = new PollOptionResultData()
                {
                    ResultRatio01 = resultData.Result01,
                    IsUserChoice = isUserChoice,
                    IsWinningOption = isWinningOption,
                    ImageUrl = optionData.ImageUrl,
                    PositionDeltaToOptionImage = positionDeltaFromOptionToResult,
                    ImageOriginSize = optionUi.ImageSize,
                    VotersAvatarImageUrls = resultData.VotersAvatarImageUrls
                };
                newOptionResultUi.DisplayResult(uiData);
            }
        }
        
        private void OptionClickedCallback(int id)
        {
            if (_pollPhase == PollPhase.Spectate)
            {
                _bottomBar.BounceStartPollButton();
                return;
            }
            if (_drawingController.IsEnabled)
            {
                return;
            }
            if (_lastSelectedId != null)
            {
                if (_lastSelectedId.Value == id)
                {
                    return;
                }
                _pollOptionsUi[_lastSelectedId.Value].SetIsSelected(false);
            }
            _lastSelectedId = id;
            _pollOptionsUi[_lastSelectedId.Value].SetIsSelected(true);
        }

        private void AddImageToAvatarInventoryClickedCallback()
        {
            var imageUrl = PollProperties.OptionsData[_lastSelectedId.Value].ImageUrl;
            _addImageToAvatarInventoryPopup.Activate(imageUrl, HandleAddImageToAvatarInventoryPopupSelection);
        }

        private void HandleAddImageToAvatarInventoryPopupSelection(bool shouldAddImage)
        {
            if (shouldAddImage)
            {
                var message = "Image added to your avatar inventory!";
                _genericMessagePopup.Activate(message);
            }
        }

        private void StartPollButtonClickedCallback()
        {
            SetPollPhase(PollPhase.WaitingForAnswer);
        }

        private void DrawingButtonClickedCallback()
        {
            SetIsDrawingEnabled(!_isDrawingEnabled);
        }

        private void SetIsDrawingEnabled(bool isEnabled)
        {
            _isDrawingEnabled = isEnabled;
            _drawingController.SetIsEnabled(_isDrawingEnabled);
            _bottomBar.RefreshDrawingButtonSprite(_isDrawingEnabled);
        }

        private void GalleryButtonClickedCallback()
        {
            SetIsDrawingEnabled(false);
            _galleryImagesController.GetImageFromGallery();
        }

        private void StickersButtonClickedCallback()
        {
            SetIsDrawingEnabled(false);
            _stickersInventory.Activate();
        }

        private void ExitPollClickedCallback()
        {
            SetPollPhase(PollPhase.ExitingPoll);
            FakeServerLink.Instance.SendRequestToServer(new ExitPollRequest(), HandleExitPollSuccess);
        }

        private void HandleExitPollSuccess(BaseServerResponse response)
        {
            ScenesManager.Instance.LoadScene(SceneName.Main);
        }
    }
}