using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum Level
{ 
    Menu = 0,
    Level1 = 1,
    Level2 = 2,
}

public class LevelSceneManager : MonoBehaviour
{
    public static LevelSceneManager Instance;
    public static Level currentLevel;
    public static bool isLoading;

    void Awake()
    {
        ScriptManager.MakeManager(this, ref Instance);

        SceneManager.sceneLoaded += OnSceneLoaded;

        Scene currentScene = SceneManager.GetActiveScene();
        OnSceneLoaded(currentScene, LoadSceneMode.Single);
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        int sceneIndex = scene.buildIndex;

        if (System.Enum.IsDefined(typeof(Level), sceneIndex))
        {
            currentLevel = (Level)sceneIndex;
            Debug.Log("Current level: " + currentLevel);
        }
        else
        {
            Debug.LogError("Invalid scene index for Level enum");
        }
    }
    public void GotoScene(int SceneNumber)
    {
        if (isLoading)
        { return; }
        //var scene = SceneManager.LoadSceneAsync(SceneNumber);

        //Run Loading Coroutine
        LoadingManager.instance.StartCoroutine(LoadingManager.instance.LoadScene(SceneNumber));

    }

    void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}
