using ServerClientCommunication;
using UiElements;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Infrastructure;

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
        [SerializeField] private PollOptionUi _pollOptionUiPrefab;
        [SerializeField] private PollOptionPosition[] _pollOptionPositions;
        [SerializeField] private RectTransform _pollOptionsContainer;
        [SerializeField] private TextMeshProUGUI _pollQuestionText;
        [SerializeField] private CountdownTimerUi _countdownTimer;
        [SerializeField] private PollBottomBar _bottomBar;
        [SerializeField] private DrawingController _drawingController;
        private Dictionary<int, List<PollOptionPosition>> _optionPositionsByOptionsCount;
        private int? _lastSelectedId;
        private Dictionary<int, PollOptionUi> _pollOptionUiById;
        private PollPhase _pollPhase;
        private bool _isDrawingEnabled;

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
        }
        
        private void Start()
        {
            SetPollPhase(PollPhase.Spectate);
#if UNITY_EDITOR
            if (PollServerData == null)
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
            while (PollServerData == null)
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
                case PollOptionPositionCategory.FourOptions:
                    return 4;
            }
            throw new Exception($"Unhandled {nameof(PollOptionPositionCategory)} {category}!");
        }

        private EnterPollResponseData PollServerData => ClientServices.Instance.PollStore.CurrentPollServerData;

        private void SetPollPhase(PollPhase pollPhase)
        {
            _pollPhase = pollPhase;
            _bottomBar.RefreshUi(_pollPhase);
            _bottomBar.RefreshDrawingButtonSprite(_isDrawingEnabled);
        }

        public void DisplayPoll()
        {
            _pollQuestionText.text = PollServerData.Question;

            _pollOptionUiById = new();
            var optionsCount = PollServerData.OptionsData.Count;
            for (var i = 0; i < _optionPositionsByOptionsCount[optionsCount].Count; i++) 
            {
                var optionPosition = _optionPositionsByOptionsCount[optionsCount][i];
                var newOptionUi = Instantiate(_pollOptionUiPrefab, _pollOptionsContainer);
                var optionData = PollServerData.OptionsData[i];
                newOptionUi.RectTransform.position = optionPosition.transform.position;
                newOptionUi.Initialize(new PollOptionUiInitializeData()
                {
                    Id = i,
                    Title = optionData.Title,
                    ImageUrl = optionData.ImageUrl
                });
                newOptionUi.OnClicked += OptionClickedCallback;
                _pollOptionUiById.Add(i, newOptionUi);
            }
            _countdownTimer.StartCountdown(63.3f);
        }

        private void OptionClickedCallback(int id)
        {
            if (_lastSelectedId != null)
            {
                if (_lastSelectedId.Value == id)
                {
                    return;
                }
                _pollOptionUiById[_lastSelectedId.Value].SetIsSelected(false);
            }
            _lastSelectedId = id;
            _pollOptionUiById[_lastSelectedId.Value].SetIsSelected(true);
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