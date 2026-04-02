using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BossSpawner : MonoBehaviour
{
    [SerializeField] GameObject[] BossPrefab;
    
    // Start is called before the first frame update
    void Start()
    {
        //Spawner invisible at start()
        GetComponent<MeshRenderer>().enabled = false;
        CheckLevelBeforeSpawn();
    }

    void Spawn(Vector3 SpawnPosition, BossType Chosenboss)
    {
        for (int i = 0; i < BossPrefab.Length; i++)
        {
            GameObject go = BossPrefab[i];
            if (go.GetComponent<BossData>().bossType == Chosenboss)
            {
                Debug.Log("Spawning FearLight Boss");
                Instantiate(BossPrefab[i], SpawnPosition, Quaternion.identity);
            }

            
        }
    }
    void CheckLevelBeforeSpawn()
    {
        switch (LevelSceneManager.currentLevel)
        {
            case Level.Level1:
                Spawn(transform.position, BossType.FearLight);
                break;
            case Level.Level2:
                Spawn(transform.position, BossType.GoodEar);
                break;

        }
    }
}
