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
        private bool _isActive;
        private readonly List<float> _topMessagesAlpha = new()
        {
            0.25f, 0.5f, 0.75f
        };

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
            var rectTransform = GetComponent<RectTransform>();
            _maxDisplayedMessages = Mathf.FloorToInt(rectTransform.rect.height / _defaultMessageUiPrefab.RectTransform.rect.height);
            Debug.LogError(">>>>>>>>>>>>>> " + _defaultMessageUiPrefab.RectTransform.rect.height + "   " + rectTransform.rect.height);
        }

        public void Activate()
        {
            if (_isActive)
            {
                return;
            }
            _isActive = true;
            FakeServerLink.Instance.OnChatMessageDataReceived += HandleChatMessageDataReceived;
            _messagesContainer.gameObject.SetActive(true);
        }

        public void Deactivate()
        {
            if (_isActive == false)
            {
                return;
            }
            _isActive = false;
            FakeServerLink.Instance.OnChatMessageDataReceived -= HandleChatMessageDataReceived;
            _messagesContainer.gameObject.SetActive(false);
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
            if (_messages.Count > 0)
            {
                _messages[0].SetAlpha(1);
            }
            _messages.Add(newMessageUi);

            var nonTransparentMessagesCount = _maxDisplayedMessages - _topMessagesAlpha.Count;
            if (_messages.Count >= nonTransparentMessagesCount)
            {
                var messagesToMakeTransparent = _messages.Count - nonTransparentMessagesCount;
                for (var i = 0; i < messagesToMakeTransparent; i++)
                {
                    var messageUi = _messages[i];
                    messageUi.SetAlpha(i >= _topMessagesAlpha.Count ?
                        1 : _topMessagesAlpha[i]);
                }
            }
        }
    }
}