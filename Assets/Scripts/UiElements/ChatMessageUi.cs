using Infrastructure;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UiElements
{
    public class ChatMessageUi : PooledObject
    {
        [SerializeField] private TextMeshProUGUI _messageText;
        [SerializeField] private Image _avatarImage;
        [SerializeField] private CanvasGroup _canvasGroup;
        [SerializeField] private RectTransform _rectTransform;

        public RectTransform RectTransform => _rectTransform;

        public void Initialize(string text, string avatarImageUrl)
        {
            _avatarImage.sprite = ClientServices.Instance.ImageStore.LoadImage(avatarImageUrl);
            _messageText.text = text;
        }

        public void SetAlpha(float alpha)
        {
            _canvasGroup.alpha = alpha;
        }

        public override void HandlePostBorrowFromPool()
        {
            SetAlpha(1);
        }

        public override void HandlePreRevertToPool()
        {
            
        }
    }
}