using Infrastructure;
using ServerClientCommunication;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Screens
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
            ClientServices.Instance.PollStore.SetCurrentPollServerData(enterPollReponse.Data);
            ScenesManager.Instance.LoadScene(SceneName.Poll);
        }
    }
}