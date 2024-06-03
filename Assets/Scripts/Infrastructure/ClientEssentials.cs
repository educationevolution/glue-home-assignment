using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace Infrastructure
{
    /// <summary>
    /// A component which contains all the -must have- components for any scene to run.
    /// This is used to Play the unity editor directly from a non Main scene.
    /// This component and all its children are moved to DontDestroyOnLoad on awake.
    /// </summary>
    public class ClientEssentials : MonoBehaviour
    {
        private static ClientEssentials Instance;

        private void Awake()
        {
            if (Instance != null)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }
}