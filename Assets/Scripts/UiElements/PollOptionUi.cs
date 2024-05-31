using Infrastructure;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UiElements
{
    public struct PollOptionUiInitializeData
    {
        public int Id;
        public string Title;
        public string ImageUrl;
    }

    public class PollOptionUi : MonoBehaviour
    {
        [SerializeField] private RectTransform _rectTransform;
        [SerializeField] private Image _mainImage;
        [SerializeField] private RectTransform _sparksContainer;
        [SerializeField] private Button _button;
        [SerializeField] private TextMeshProUGUI[] _titleTexts;
        [SerializeField] private RectTransform[] _nonSelectedContainers;
        [SerializeField] private RectTransform[] _selectedContainers;
        public Action<int> OnClicked;
        public RectTransform RectTransform => _rectTransform;
        private int _optionId;

        private void Awake()
        {
            _button.onClick.AddListener(ButtonClickedCallback);
        }

        public void Initialize(PollOptionUiInitializeData initData)
        {
            _optionId = initData.Id;
            _mainImage.sprite = ClientServices.Instance.ImageStore.LoadImage(initData.ImageUrl);
            var sparksRotation = _sparksContainer.eulerAngles;
            sparksRotation.z = UnityEngine.Random.Range(0, 360);
            _sparksContainer.eulerAngles = sparksRotation;
            for (var i = 0; i < _titleTexts.Length; i++) 
            {
                var titleText = _titleTexts[i];
                titleText.text = initData.Title;
            }
            SetIsSelected(false);
        }

        public void SetIsSelected(bool isSelected)
        {
            for (var i = 0; i < _selectedContainers.Length; i++) 
            {
                var container = _selectedContainers[i];
                container.gameObject.SetActive(isSelected);
            }
            for (var i = 0; i < _nonSelectedContainers.Length; i++)
            {
                var container = _nonSelectedContainers[i];
                container.gameObject.SetActive(!isSelected);
            }
        }

        private void ButtonClickedCallback()
        {
            OnClicked?.Invoke(_optionId);
        }
    }
}