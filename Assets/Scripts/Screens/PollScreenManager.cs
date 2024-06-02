using ServerClientCommunication;
using UiElements;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Infrastructure;
using static UnityEditor.Progress;
using UnityEngine.UI;
using Chat;

namespace Screens
{
    public enum PollPhase
    {
        Spectate,
        WaitingForAnswer,
        WaitingForResults,
        Results
    }

    public class PollScreenManager : MonoBehaviour
    {
        [SerializeField] private PollOptionPosition[] _pollOptionPositions;
        [SerializeField] private RectTransform _pollOptionsContainer;
        [SerializeField] private TextMeshProUGUI _pollQuestionText;
        [SerializeField] private CountdownTimerUi _countdownTimer;
        [SerializeField] private PollBottomBar _bottomBar;
        [SerializeField] private DrawingController _drawingController;
        [SerializeField] private RectTransform _pollResultsContainer;
        [SerializeField] private PollOptionUi _pollOptionUiPrefab;
        [SerializeField] private PollOptionResultUi _optionResultUiPrefab;
        [SerializeField] private RectTransform _rootContainer;
        [SerializeField] private ChatMessagesDisplayer _chatMessagesDisplayer;
        [SerializeField] private PollResultsCallToAction _resultsCallToAction;
        private Dictionary<int, List<PollOptionPosition>> _optionPositionsByOptionsCount;
        private int? _lastSelectedId;
        private List<PollOptionUi> _pollOptionsUi;
        private List<PollOptionResultUi> _pollOptionResultsUi;
        private PollPhase _pollPhase;
        private bool _isDrawingEnabled;
        private EnterPollResponseData PollProperties => ClientServices.Instance.PollStore.CurrentPollProperties;
        private PollResultsResponseData PollResults => ClientServices.Instance.PollStore.CurrentPollResults;

        private void Awake()
        {
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
            _bottomBar.OnStartPollButtonClicked += StartPollButtonClickedCallback;
            _bottomBar.OnDrawingButtonClicked += DrawingButtonClickedCallback;
            _bottomBar.OnGalleryButtonClicked += GalleryButtonClickedCallback;
            _bottomBar.OnStickersButtonClicked += StickersButtonClickedCallback;
            var children = _pollResultsContainer.GetComponentsInChildren<PollOptionResultUi>();
            for (var i = 0; i < children.Length; i++)
            {
                var child = children[i];
                Destroy(child.gameObject);
            }
            FakeServerLink.Instance.OnPollResultsDataReceived += HandlePollResultsReceived;
        }
        
        private void Start()
        {
            SetPollPhase(PollPhase.Spectate);
#if UNITY_EDITOR
            if (PollProperties == null)
            {
                // Required only when running this scene directly
                FakeServerLink.Instance.SendRequestToServer(new EnterPollRequest(), null);
                StartCoroutine(DisplayPollWhenReady());
                return;
            }
#endif
            DisplayPoll();
        }

#if UNITY_EDITOR
        private IEnumerator DisplayPollWhenReady()
        {
            while (PollProperties == null)
            {
                yield return null;
            }
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
            if (_pollPhase == PollPhase.Spectate)
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
            }
            _bottomBar.RefreshUi(_pollPhase);
            _bottomBar.RefreshDrawingButtonSprite(_isDrawingEnabled);
        }

        public void DisplayPoll()
        {
            _pollQuestionText.text = PollProperties.Question;

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
            _countdownTimer.StartCountdown(PollProperties.SecondsLeft);
        }

        private void HandlePollResultsReceived()
        {
            SetPollPhase(PollPhase.Results);
            var winningOptionIndex = 0;
            var highestResult01 = 0f;
            for (var i = 0; i < PollResults.Results01.Count; i++)
            {
                var result01 = PollResults.Results01[i];
                if (highestResult01 < result01)
                {
                    highestResult01 = result01;
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
                var result01 = PollResults.Results01[i];
                var isUserChoice = _lastSelectedId == null ?
                    false : _lastSelectedId.Value == i;
                var isWinningOption = winningOptionIndex == i;
                var newOptionResultUi = _pollOptionResultsUi[i];
                var positionDeltaFromOptionToResult = optionUi.ImageRectPosition - newOptionResultUi.RectTransform.position;
                var uiData = new PollOptionResultData()
                {
                    Ratio01 = result01,
                    IsUserChoice = isUserChoice,
                    IsWinningOption = isWinningOption,
                    ImageUrl = optionData.ImageUrl,
                    PositionDeltaToOptionImage = positionDeltaFromOptionToResult,
                    ImageOriginSize = optionUi.ImageSize
                };
                newOptionResultUi.DisplayResult(uiData);
            }
        }

        private void OptionClickedCallback(int id)
        {
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
        }

        private void StickersButtonClickedCallback()
        {
            SetIsDrawingEnabled(false);
        }
    }
}