using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Infrastructure
{
    public enum SceneName
    {
        None,
        Main,
        Poll
    }

    /// <summary>
    /// A manager for switching between scenes.
    /// </summary>
    public class ScenesManager : MonoBehaviour
    {
        [SerializeField] private int _defaultTargetFrameRate = 60;
        public static ScenesManager Instance { get; private set; }
        private SceneName? _currentScene;

        private void Awake()
        {
            if (Instance != null)
            {
                Destroy(gameObject);
                return;
            }
            QualitySettings.vSyncCount = 0;
            Application.targetFrameRate = _defaultTargetFrameRate;
            Instance = this;
            DontDestroyOnLoad(gameObject);
            var currentSceneName = SceneManager.GetActiveScene().name;
            Enum.TryParse(currentSceneName, out SceneName sceneName);
            _currentScene = sceneName;
        }

        /// <summary>
        /// Load a new scene.
        /// Also revert all borrowed instances back into the object pool.
        /// </summary>
        public void LoadScene(SceneName sceneToLoad)
        {
            if (_currentScene == sceneToLoad)
            {
                throw new Exception($"Scene {sceneToLoad} is already opened!");
            }
            ObjectPool.Instance.RevertAllInstancesToPool();
            _currentScene = sceneToLoad;
            SceneManager.LoadScene(_currentScene.ToString());
        }
    }
}