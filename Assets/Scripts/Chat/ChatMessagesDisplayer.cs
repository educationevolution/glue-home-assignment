using Infrastructure;
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
        [SerializeField] private ChatMessageUi _defaultMessageUiPrefab;
        [SerializeField] private ChatMessageUi _ownMessageUiPrefab;
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
            if (_messages.Count >= _maxDisplayedMessages)
            {
                var messageUiToRemove = _messages[0];
                _messages.RemoveAt(0);
                ObjectPool.Instance.Revert(messageUiToRemove);
            }
            var prefab = messageData.UserId == ClientServices.Instance.FakeUserId ?
                _ownMessageUiPrefab : _defaultMessageUiPrefab;
            var newMessageUi = ObjectPool.Instance.Borrow(prefab, _messagesContainer).GetComponent<ChatMessageUi>();
            newMessageUi.Initialize(messageData.Text, messageData.AvatarImageUrl);
            _messages.Add(newMessageUi);
        }
    }
}