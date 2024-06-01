using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace UiElements
{
    public class CountdownTimerUi : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _timeLeftText;
        private Coroutine _countdownCoroutine;
        private float _realtimeSinceStartupCountdownEnd;

        private void Awake()
        {
            EndCountdown();
        }

        public void StartCountdown(float secondsLeft)
        {
            EndCountdown();
            _realtimeSinceStartupCountdownEnd = Time.realtimeSinceStartup + secondsLeft;
            _countdownCoroutine = StartCoroutine(CountdownCoroutine());
        }

        private IEnumerator CountdownCoroutine()
        {
            UpdateTimeLeftText();
            var initialDelay = _realtimeSinceStartupCountdownEnd - Mathf.FloorToInt(_realtimeSinceStartupCountdownEnd);
            yield return new WaitForSeconds(initialDelay);
            while (Time.realtimeSinceStartup < _realtimeSinceStartupCountdownEnd)
            {
                UpdateTimeLeftText();
                yield return new WaitForSeconds(1);
            }
            EndCountdown();
        }

        private void UpdateTimeLeftText()
        {
            var secondsLeft = Mathf.RoundToInt(_realtimeSinceStartupCountdownEnd - Time.realtimeSinceStartup);
            var minutesLeft = Mathf.FloorToInt(secondsLeft / 60);
            secondsLeft -= minutesLeft * 60;
            var timeLeftStr = minutesLeft > 0 ?
                $"{minutesLeft}:" : "0:";
            timeLeftStr += secondsLeft > 9 ?
                secondsLeft : $"0{secondsLeft}";
            _timeLeftText.text = timeLeftStr;
        }

        public void EndCountdown()
        {
            _timeLeftText.text = string.Empty;
            if (_countdownCoroutine == null)
            {
                return;
            }
            StopCoroutine(_countdownCoroutine);
            _countdownCoroutine = null;
        }
    }
}