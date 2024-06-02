using Infrastructure;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.UI;

namespace UiElements
{
    public class StickerItemUi : PooledObject
    {
        [SerializeField] private Image _stickerImage;
        public Action<int> OnItemMouseDown;
        private int _id;

        public void Initialize(int id, UserStickerData data)
        {
            _id = id;
            _stickerImage.sprite = ClientServices.Instance.ImageStore.LoadImage(data.ImageUrl);
        }

        private void OnMouseDown()
        {
            OnItemMouseDown?.Invoke(_id);
        }

        public override void HandlePostBorrowFromPool()
        {
            
        }

        public override void HandlePreRevertToPool()
        {
            
        }
    }
}