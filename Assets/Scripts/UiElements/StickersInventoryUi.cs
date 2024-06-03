using Effects;
using Infrastructure;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UiElements
{
    public class StickersInventoryUi : MonoBehaviour
    {
        const float OUT_OF_SCREEN_Y_ADDON = 1000;
        [SerializeField] private GenericUiElementAnimator _genericAnimator;
        [SerializeField] private Button _closeButton;
        [SerializeField] private StickerItemUi _stickerItemPrefab;
        [SerializeField] private DraggableImage _dragableStickerPrefab;
        [SerializeField] private RectTransform _stickerItemsContainer;
        [SerializeField] private RectTransform _dragableStickersContainer;
        [SerializeField] private Canvas _mainCanvas;
        private List<DraggableImage> _dragableStickers = new();

        private void Awake()
        {
            _closeButton.onClick.AddListener(CloseClickedCallback);
            var children = _stickerItemsContainer.GetComponentsInChildren<StickerItemUi>();
            for (var i = 0; i < children.Length; i++)
            {
                var child = children[i];
                Destroy(child.gameObject);
            }
        }

        private void Start()
        {
            Deactivate(isImmediate: true);

            var userStickersData = ClientServices.Instance.StickersStore.GetAllUserStickersData;
            var allStickerItems = new List<StickerItemUi>();
            foreach (var stickerId in userStickersData.Keys)
            {
                var stickerData = userStickersData[stickerId];
                var stickerItem = ObjectPool.Instance.Borrow(_stickerItemPrefab, _stickerItemsContainer).GetComponent<StickerItemUi>();
                stickerItem.Initialize(stickerId, stickerData);
                stickerItem.OnPointerIsDown += StickerItemMouseDownCallback;
                allStickerItems.Add(stickerItem);
            }
        }

        public void HideAll()
        {
            for (var i = 0; i < _dragableStickers.Count; i++)
            {
                var dragableSticker = _dragableStickers[i];
                dragableSticker.AnimateToFullTransparency();
            }
        }

        private void StickerItemMouseDownCallback(int id)
        {
            var stickerData = ClientServices.Instance.StickersStore.GetUserStickerData(id);
            var dragableSticker = ObjectPool.Instance.Borrow(_dragableStickerPrefab, _dragableStickersContainer).GetComponent<DraggableImage>();
            dragableSticker.ShowImage(stickerData.ImageUrl, canvasScaleFactor: _mainCanvas.scaleFactor, 
                showFrame: false, bounce: false);
            dragableSticker.StartManualDrag();
            _dragableStickers.Add(dragableSticker);
        }

        private void CloseClickedCallback()
        {
            Deactivate();
        }

        public void Activate()
        {
            _genericAnimator.MoveToPosition(Vector2.zero);
        }

        public void Deactivate(bool isImmediate = false)
        {
            _genericAnimator.MoveToPosition(Vector2.down * OUT_OF_SCREEN_Y_ADDON, isImmediate);
        }
    }
}