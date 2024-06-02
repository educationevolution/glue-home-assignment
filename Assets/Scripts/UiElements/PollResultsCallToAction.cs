using Effects;
using Infrastructure;
using Polls;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UiElements
{
    public class PollResultsCallToAction : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _pollEndedTitleText;
        [SerializeField] private TextMeshProUGUI _pollEndedDescriptionText;
        [SerializeField] private Button _exitPollButton;
        [SerializeField] private GenericUiElementAnimator _genericAnimator;
        public Action OnExitClicked;
        private PollStore PollStore => ClientServices.Instance.PollStore;

        private void Awake()
        {
            _exitPollButton.onClick.AddListener(ExitButtonClickedCallback);
        }

        public void Activate()
        {
            gameObject.SetActive(true);
            _pollEndedTitleText.text = PollStore.CurrentPollResults.CallToActionTitle;
            _pollEndedDescriptionText.text = PollStore.CurrentPollResults.CallToActionDescription;
            _genericAnimator.Bounce();
        }

        public void Deactivate()
        {
            gameObject.SetActive(false);
        }

        private void ExitButtonClickedCallback()
        {
            OnExitClicked?.Invoke();
        }
    }
}