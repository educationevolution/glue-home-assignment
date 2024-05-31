using Scripts.Infrastructure;
using Scripts.ServerClientCommunication;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Scripts.Screens
{
    public class MainScreenManager : MonoBehaviour
    {
        [SerializeField] private Button _startPollButton;

        private void Awake()
        {
            _startPollButton.onClick.AddListener(StartPollClickedCallback);
        }

        private void StartPollClickedCallback()
        {
            FakeServerLink.Instance.RequestToEnterPoll(new EnterPollRequest(), EnterPollResponseHandler);
        }

        private void EnterPollResponseHandler(BaseServerResponse reponse)
        {
            var enterPollReponse = reponse as EnterPollResponse;
            ScenesManager.Instance.LoadScene(SceneName.Poll);
        }
    }
}