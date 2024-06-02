using Infrastructure;
using Polls;
using Screens;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace UiElements
{
    public class PollTopUi : MonoBehaviour
    {
        [SerializeField] private CountdownTimerUi _countdownTimer;
        [SerializeField] private TextMeshProUGUI _questionText;
        [SerializeField] private TextMeshProUGUI _resultsTitleText;
        private PollStore PollStore => ClientServices.Instance.PollStore;

        public void UpdateUi(PollPhase phase)
        {
            _resultsTitleText.gameObject.SetActive(phase == PollPhase.Results);
            _questionText.text = phase == PollPhase.EditorOnlyWaitingForData ?
                string.Empty : PollStore.CurrentPollProperties.Question;
            var isCountdownTimerActive = phase == PollPhase.Spectate || phase == PollPhase.WaitingForAnswer || phase == PollPhase.WaitingForResults;
            _countdownTimer.gameObject.SetActive(isCountdownTimerActive);
            if (phase == PollPhase.Spectate)
            {
                _countdownTimer.StartCountdown(PollStore.CurrentPollProperties.SecondsLeft);
            }
        }
    }
}