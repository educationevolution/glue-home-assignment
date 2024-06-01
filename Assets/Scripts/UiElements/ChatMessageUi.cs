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

        public void Initialize(string text, string avatarImageUrl)
        {
            _avatarImage.sprite = ClientServices.Instance.ImageStore.LoadImage(avatarImageUrl);
            _messageText.text = text;
        }

        public override void HandlePostBorrowFromPool()
        {
            
        }

        public override void HandlePreRevertToPool()
        {
            
        }
    }
}