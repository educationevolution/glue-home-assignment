using Effects;
using Infrastructure;
using ServerClientCommunication;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace UiElements
{
    public class DraggableImage : PooledObject, IPointerDownHandler, IPointerUpHandler
    {
        [SerializeField] private RectTransform _rootRectTrans;
        [SerializeField] private Image _image;
        [SerializeField] private GenericUiElementAnimator _genericAnimator;
        [SerializeField] private Image[] _frameImageAssets;
        private bool _isManualDragActive;

        public void ShowImage(Texture2D texture, bool showFrame = true, bool bounce = true)
        {
            var rect = new Rect(0, 0, texture.width, texture.height);
            var sprite = Sprite.Create(texture, rect, pivot: Vector2.zero, pixelsPerUnit: 100f);
            ShowImageInternal(sprite, showFrame, bounce);
        }

        public void ShowImage(string imageUrl, bool showFrame, bool bounce)
        {
            var sprite = ClientServices.Instance.ImageStore.LoadImage(imageUrl);
            ShowImageInternal(sprite, showFrame, bounce);
        }

        private void ShowImageInternal(Sprite sprite, bool showFrame, bool bounce)
        {
            _image.sprite = sprite;
            if (bounce)
            {
                const float BOUNCE_DURATION = 0.2f;
                _genericAnimator.Bounce(BOUNCE_DURATION);
            }
            for (var i = 0; i < _frameImageAssets.Length; i++)
            {
                _frameImageAssets[i].gameObject.SetActive(showFrame);
            }
        }

        private void Update()
        {
            if (_isManualDragActive == false)
            {
                return;
            }
            if (IsBeingTouched())
            {
                SetAtMousePosition();
            } else
            {
                _isManualDragActive = false;
            }
        }

        private bool IsBeingTouched()
        {
#if UNITY_EDITOR
            return Input.GetMouseButtonUp(0) == false;
#endif
            return Input.touchCount > 0;
        }

        public void StartManualDrag()
        {
            SetAtMousePosition();
            _isManualDragActive = true;
        }

        private void SetAtMousePosition()
        {
            Vector3 newPosition;
#if UNITY_EDITOR
            newPosition = Input.mousePosition;
#else
            var touchPosition = Input.touches[0].position;
            newPosition = new Vector3(touchPosition.x, touchPosition.y, 0);
#endif
            newPosition.z = _rootRectTrans.position.z;
            newPosition.x -= Screen.width / 2;
            newPosition.y -= Screen.height / 2;
            _rootRectTrans.anchoredPosition = newPosition;
        }

        public void AnimateToFullTransparency()
        {
            _genericAnimator.AnimateToFullTransparency();
        }

        public override void HandlePostBorrowFromPool()
        {
            
        }

        public override void HandlePreRevertToPool()
        {
            _genericAnimator.ResetAll();
            _isManualDragActive = false;
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            _isManualDragActive = true;
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            _isManualDragActive = false;
        }
    }
}