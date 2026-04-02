using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;


public class LoadingManager : MonoBehaviour
{
    public Slider progressBar;
    public TextMeshProUGUI LevelName;
    public static LoadingManager instance;

    private void Awake()
    {
        ScriptManager.MakeManager(this, ref instance);
    }

    public IEnumerator LoadScene(int sceneToLoad)
    {
        AsyncOperation load = SceneManager.LoadSceneAsync(sceneToLoad);
        load.allowSceneActivation = false;

        if (UIManager.instance != null)
            UIManager.instance.NavigateP("Loading");

        if (LevelName != null)
            LevelName.text = "Loading " + ((Level)sceneToLoad);

        float smoothProgress = 0f;

        while (load.progress < 0.9f)
        {
            smoothProgress = Mathf.MoveTowards(
                smoothProgress,
                load.progress,
                Time.unscaledDeltaTime
            );

            progressBar.value = smoothProgress;
            yield return null;
        }

        // from 0.9 → 1
        while (smoothProgress < 1f)
        {
            smoothProgress = Mathf.MoveTowards(
                smoothProgress,
                1f,
                Time.unscaledDeltaTime
            );

            progressBar.value = smoothProgress;
            yield return null;
        }

        yield return new WaitForSecondsRealtime(0.3f);

        load.allowSceneActivation = true;
    }

}