using Effects;
using Infrastructure;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UiElements
{
    public class DragableImage : PooledObject
    {
        [SerializeField] private RectTransform _rootRectTrans;
        [SerializeField] private Image _image;
        [SerializeField] private GenericUiElementAnimator _genericAnimator;
        private bool _isManualDragActive;

        private void Awake()
        {
            //_button.OnPointerDown += ButtonMouseDownCallback;
        }

        private void OnMouseDrag()
        {
            SetAtMousePosition();
        }

        private void Update()
        {
            if (_isManualDragActive == false)
            {
                return;
            }
            if (Input.GetMouseButtonUp(0))
            {
                _isManualDragActive = false;
            } else
            {
                SetAtMousePosition();
            }
        }

        public void StartManualDrag()
        {
            SetAtMousePosition();
            _isManualDragActive = true;
        }

        private void SetAtMousePosition()
        {
            var newPosition = Input.mousePosition;
            newPosition.z = _rootRectTrans.position.z;
            newPosition.x -= Screen.width / 2;
            newPosition.y -= Screen.height / 2;
            _rootRectTrans.anchoredPosition = newPosition;
        }

        public void ShowImage(Texture2D texture)
        {
            var rect = new Rect(0, 0, texture.width, texture.height);
            var sprite = Sprite.Create(texture, rect, pivot: Vector2.zero, pixelsPerUnit: 100f);
            ShowImageInternal(sprite);
        }

        public void ShowImage(string imageUrl, bool bounce)
        {
            var sprite = ClientServices.Instance.ImageStore.LoadImage(imageUrl);
            ShowImageInternal(sprite, bounce);
        }

        private void ShowImageInternal(Sprite sprite, bool bounce = true)
        {
            _image.sprite = sprite;
            if (bounce)
            {
                const float BOUNCE_DURATION = 0.2f;
                _genericAnimator.Bounce(BOUNCE_DURATION);
            }
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
    }
}