using ServerClientCommunication;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UiElements;
using UnityEngine;

namespace Chat
{
    public class ChatMessagesDisplayer : MonoBehaviour
    {
        [SerializeField] private ChatMessageUi _messageUiPrefab;
        [SerializeField] private int _maxDisplayedMessages = 10;
        [SerializeField] private RectTransform _messagesContainer;
        private List<ChatMessageUi> _messages = new();
        private float _nextMessageTime;
        private int _messageCounter;

        private void Awake()
        {
            var garbageMessages = _messagesContainer.GetComponentsInChildren<ChatMessageUi>();
            if (garbageMessages.Length > 0 )
            {
                for (var i = garbageMessages.Length - 1; i >= 0; i--) 
                {
                    var message = garbageMessages[i];
                    Destroy(message.gameObject);
                }
            }
        }

        private void Start()
        {
            FakeServerLink.Instance.OnChatMessageDataReceived += HandleChatMessageDataReceived;
        }

        private void HandleChatMessageDataReceived(ServerChatMessageData messageData)
        {
            ChatMessageUi newMessageUi;
            if (_messages.Count >= _maxDisplayedMessages)
            {
                newMessageUi = _messages[0];
                newMessageUi.transform.SetParent(null);
                _messages.RemoveAt(0);
                newMessageUi.transform.SetParent(_messagesContainer);
            } else
            {
                newMessageUi = Instantiate(_messageUiPrefab, _messagesContainer);
            }
            newMessageUi.Initialize(messageData.Text, messageData.AvatarImageUrl);
            _messages.Add(newMessageUi);
        }
    }
}