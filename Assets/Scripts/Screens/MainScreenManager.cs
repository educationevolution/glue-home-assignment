using Scripts.Infrastructure;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Scripts.Screens
{
    public class MainScreenManager : MonoBehaviour
    {
        [SerializeField] private Button _startPollButton;

        private void Awake()
        {
            _startPollButton.onClick.AddListener(StartPollClickedCallback);
        }

        private void StartPollClickedCallback()
        {
            ScenesManager.Instance.LoadScene(SceneName.Poll);
        }
    }
}