using Effects;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Popups
{
    /// <summary>
    /// A popup for a generic message with an Ok button.
    /// </summary>
    public class GenericMessagePopup : MonoBehaviour
    {
        [SerializeField] private Button _okButton;
        [SerializeField] private GenericUiElementAnimator _genericAnimator;
        [SerializeField] private TextMeshProUGUI _text;
        [SerializeField] private RectTransform _raycastBlocker;

        private void Awake()
        {
            Deactivate();
            _okButton.onClick.AddListener(OkClickedCallback);
        }

        public void Activate(string message)
        {
            gameObject.SetActive(true);
            _text.text = message;
            _genericAnimator.Bounce(0.2f);
            _raycastBlocker.gameObject.SetActive(true);
        }

        private void OkClickedCallback()
        {
            Deactivate();
        }

        private void Deactivate()
        {
            gameObject.SetActive(false);
            _raycastBlocker.gameObject.SetActive(false);
        }
    }
}