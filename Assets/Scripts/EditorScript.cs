using Infrastructure;
using System.Collections;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
#endif
using UnityEngine;

public class EditorScript
{
#if UNITY_EDITOR
    [MenuItem("Glue/Run Main")]
    public static void RunMainScene()
    {
        EditorSceneManager.OpenScene($"Assets/Scenes/{SceneName.Main}.unity");
        EditorApplication.isPlaying = true;
    }

    [MenuItem("Glue/Open Main")]
    public static void OpenMainScene()
    {
        EditorSceneManager.OpenScene($"Assets/Scenes/{SceneName.Main}.unity");
    }

    [MenuItem("Glue/Open Poll")]
    public static void OpenPollScene()
    {
        EditorSceneManager.OpenScene($"Assets/Scenes/{SceneName.Poll}.unity");
    }
#endif
}
