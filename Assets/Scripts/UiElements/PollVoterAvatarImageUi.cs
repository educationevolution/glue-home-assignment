using Infrastructure;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UiElements
{
    public class PollVoterAvatarImageUi : PooledObject
    {
        [SerializeField] private Image _avatarImage;

        public void ShowImage(string imageUrl)
        {
            _avatarImage.sprite = ClientServices.Instance.ImageStore.LoadImage(imageUrl);
        }

        public override void HandlePostBorrowFromPool()
        {
            
        }

        public override void HandlePreRevertToPool()
        {
            
        }
    }
}