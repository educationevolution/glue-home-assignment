using Chat;
using Effects;
using Infrastructure;
using Screens;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UiElements
{
    public class PollBottomBar : MonoBehaviour
    {
        [SerializeField] private ChatMessageInputController _chatInputController;
        [SerializeField] private Button _startPollButton;
        [SerializeField] private Button _drawingButton;
        [SerializeField] private Button _stickersButton;
        [SerializeField] private Button _galleryButton;
        [SerializeField] private Sprite _drawingDisabledButtonSprite;
        [SerializeField] private Sprite _drawingEnabledButtonSprite;
        [SerializeField] private RectTransform _spectatePhaseContainer;
        [SerializeField] private RectTransform _participationPhaseContainer;
        [SerializeField] private RectTransform _resultsPhaseContainer;
        [SerializeField] private GenericUiElementAnimator _startPollButtonGenericAnimator;
        public event Action OnStartPollButtonClicked;
        public event Action OnDrawingButtonClicked;
        public event Action OnStickersButtonClicked;
        public event Action OnGalleryButtonClicked;
        

        private void Awake()
        {
            _startPollButton.onClick.AddListener(StartPollButtonClickedCallback);
            _drawingButton.onClick.AddListener(DrawingButtonClickedCallback);
            _stickersButton.onClick.AddListener(StickersButtonClickedCallback);
            _galleryButton.onClick.AddListener(GalleryButtonClickedCallback);
        }

        public void RefreshUi(PollPhase phase)
        {
            _spectatePhaseContainer.gameObject.SetActive(phase == PollPhase.Spectate);
            _participationPhaseContainer.gameObject.SetActive(phase == PollPhase.WaitingForAnswer || phase == PollPhase.WaitingForResults);
            _resultsPhaseContainer.gameObject.SetActive(phase == PollPhase.Results);
        }

        public void RefreshDrawingButtonSprite(bool isDrawingEnabled)
        {
            _drawingButton.image.sprite = isDrawingEnabled ?
                _drawingEnabledButtonSprite : _drawingDisabledButtonSprite;
        }

        private void StartPollButtonClickedCallback()
        {
            OnStartPollButtonClicked?.Invoke();
        }

        private void DrawingButtonClickedCallback()
        {
            OnDrawingButtonClicked?.Invoke();
        }

        private void StickersButtonClickedCallback()
        {
            OnStickersButtonClicked?.Invoke();
        }

        private void GalleryButtonClickedCallback()
        {
            OnGalleryButtonClicked?.Invoke();
        }

        public void BounceStartPollButton()
        {
            _startPollButtonGenericAnimator.Bounce();
        }
    }
}