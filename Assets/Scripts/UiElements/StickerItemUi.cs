using Infrastructure;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace UiElements
{
    public class StickerItemUi : PooledObject, IPointerDownHandler
    {
        [SerializeField] private Image _stickerImage;
        public Action<int> OnPointerIsDown;
        private int _id;

        public void Initialize(int id, UserStickerData data)
        {
            _id = id;
            _stickerImage.sprite = ClientServices.Instance.ImageStore.LoadImage(data.ImageUrl);
        }

        public override void HandlePostBorrowFromPool()
        {
            
        }

        public override void HandlePreRevertToPool()
        {
            
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            OnPointerIsDown?.Invoke(_id);
        }
    }
}