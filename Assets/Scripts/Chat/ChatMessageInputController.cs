using ServerClientCommunication;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Chat
{
    public class ChatMessageInputController : MonoBehaviour
    {
        [SerializeField] private TMP_InputField _chatInput;

        private void Awake()
        {
#if UNITY_EDITOR
            _chatInput.onEndEdit.AddListener(val =>
            {
                TryToSendChatMessageRequest(_chatInput.text);
            });
#endif

            _chatInput.onTouchScreenKeyboardStatusChanged.AddListener(val =>
            {
                if (val == TouchScreenKeyboard.Status.Done)
                {
                    TryToSendChatMessageRequest(_chatInput.text);
                }
            });
        }

        private void TryToSendChatMessageRequest(string message)
        {
            if (message == string.Empty)
            {
                return;
            }
            var chatMessageRequest = new SendChatMessageRequest(new SendChatMessageRequestData()
            {
                Message = message
            });
            FakeServerLink.Instance.SendRequestToServer(chatMessageRequest, null);
            _chatInput.text = string.Empty;
        }
    }
}