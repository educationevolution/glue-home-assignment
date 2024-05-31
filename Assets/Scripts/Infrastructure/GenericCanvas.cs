using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Scripts.Infrastructure
{
    public class GenericCanvas : MonoBehaviour
    {
        [SerializeField] private RectTransform _genericLoadingMessage;

        public static GenericCanvas Instance;
        
        private void Awake()
        {
            if (Instance != null)
            {
                Destroy(gameObject);
                return;
            }
            DontDestroyOnLoad(gameObject);
            Instance = this;
            HideGenericLoadingMessage();
        }

        public void ShowGenericLoadingMessage()
        {
            _genericLoadingMessage.gameObject.SetActive(true);
        }

        public void HideGenericLoadingMessage()
        {
            _genericLoadingMessage.gameObject.SetActive(false);
        }
    }
}