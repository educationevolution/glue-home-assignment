using Effects;
using Infrastructure;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Popups
{
    public class AddImageToAvatarInventoryPopup : MonoBehaviour
    {
        [SerializeField] private Button _yesButton;
        [SerializeField] private Button _noButton;
        [SerializeField] private GenericUiElementAnimator _genericAnimator;
        [SerializeField] private RectTransform _raycastBlocker;
        [SerializeField] private Image _image;
        private Action<bool> _decisionHandler;


        private void Awake()
        {
            Deactivate();
            _yesButton.onClick.AddListener(YesButtonClickedCallback);
            _noButton.onClick.AddListener(NoButtonClickedCallback);
        }

        public void Activate(string imageUrl, Action<bool> decisionHandler)
        {
            _raycastBlocker.gameObject.SetActive(true);
            gameObject.SetActive(true);
            _genericAnimator.Bounce(0.2f);
            _image.sprite = ClientServices.Instance.ImageStore.LoadImage(imageUrl);
            _decisionHandler = decisionHandler;
        }

        public void Deactivate()
        {
            gameObject.SetActive(false);
            _raycastBlocker.gameObject.SetActive(false);
        }

        private void YesButtonClickedCallback()
        {
            Deactivate();
            _decisionHandler(true);
        }

        private void NoButtonClickedCallback()
        {
            Deactivate();
            _decisionHandler(false);
        }
    }
}