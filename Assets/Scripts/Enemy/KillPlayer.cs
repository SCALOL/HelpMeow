using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class KillPlayer : MonoBehaviour
{
    [SerializeField] bool isGameOver = false;

    private void Start()
    {
        isGameOver = false;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (!isGameOver && other.CompareTag("Player"))
        {
                isGameOver = true;
                Debug.Log("Game Over");
                UIManager.instance.NavigateP("Lose");
                AudioManager.instance.LoseAudio();
                StartCoroutine(RestartAfterDelay());
                UIManager.instance.GameOverUI();
        }

        
    }

    IEnumerator RestartAfterDelay()
    {
        Time.timeScale = 0f;

        yield return new WaitForSecondsRealtime(2f);

        Time.timeScale = 1f; 
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}

