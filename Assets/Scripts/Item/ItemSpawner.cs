using System.Collections;
using System.Collections.Generic;
using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ItemSpawner : MonoBehaviour
{
    [SerializeField] Transform[] itemMarkerChild;
    [SerializeField] GameObject[] itemsToSpawn;
    [SerializeField] GameObject Battery;
    [SerializeField] NavMeshSurface NavSur;
    List<int> usedIndices = new List<int>();
    [SerializeField] private float minbatSpawn;
    [SerializeField] private float maxbatSpawn;
    [SerializeField] private int maxBatteries = 5;
    [SerializeField] private float Yoffset = 1f;

    // Start is called before the first frame update
    IEnumerator SpawnBatteryRoutine()
    {
        while (true)
        {
            float waitTime = Random.Range(minbatSpawn, maxbatSpawn);
            yield return new WaitForSeconds(waitTime);
            SpawnBattery();
        }
    }

    void SpawnBattery()
    {
        GameObject[] allBatteries = GameObject.FindGameObjectsWithTag("Battery");
        if (Battery == null || NavSur == null || allBatteries.Length >= maxBatteries) return;
        // Try up to 20 times to find a valid position
        for (int attempt = 0; attempt < 20; attempt++)
        {
            
            Vector3 randomPoint = GetRandomPointOnNavMesh();
            randomPoint.y += Yoffset;
            if (randomPoint == Vector3.zero) continue;
            
            // Check distance to all spawned items
            bool valid = true;
            GameObject[] allItems = GameObject.FindGameObjectsWithTag("Item");
            foreach (var obj in allItems)
            {
                
                if (Vector3.Distance(obj.transform.position, randomPoint) < 2f)
                {
                    valid = false;
                    break;
                }
                
            }
            if (!valid) continue;
            
            // Check distance to all batteries
            
            foreach (var obj in allBatteries)
            {
                
                if (Vector3.Distance(obj.transform.position, randomPoint) < 2f)
                {
                    valid = false;
                    break;
                }
                
            }
           
            if (!valid) continue;

            // Spawn battery
           
            GameObject batteryObj = Instantiate(Battery, randomPoint, Quaternion.identity);
            batteryObj.tag = "Battery";
            return;
        }
    }

    Vector3 GetRandomPointOnNavMesh()
    {
        var triangulation = UnityEngine.AI.NavMesh.CalculateTriangulation();

        if (triangulation.indices.Length == 0)
        {
            return Vector3.zero;
        }

        // Pick a random triangle
        int triIndex = Random.Range(0, triangulation.indices.Length / 3) * 3;

        Vector3 v0 = triangulation.vertices[triangulation.indices[triIndex]];
        Vector3 v1 = triangulation.vertices[triangulation.indices[triIndex + 1]];
        Vector3 v2 = triangulation.vertices[triangulation.indices[triIndex + 2]];

        // Random point inside triangle (barycentric coordinates)
        float r1 = Random.value;
        float r2 = Random.value;

        if (r1 + r2 > 1f)
        {
            r1 = 1f - r1;
            r2 = 1f - r2;
        }

        Vector3 randomPoint =
            v0 +
            r1 * (v1 - v0) +
            r2 * (v2 - v0);

        return randomPoint;
    }



    // Add this to Start()
    void Start()
    {
        List<Transform> markerList = new List<Transform>();

        itemMarkerChild = GetComponentsInChildren<Transform>();
        // Skip the first element as it is the parent object
        itemMarkerChild = GameObject.Find("ItemMarkers").GetComponentsInChildren<Transform>();

        for (int i = 1; i <= itemMarkerChild.Length - 1; i++)
        {
            markerList.Add(itemMarkerChild[i]);
            itemMarkerChild[i].GetComponent<MeshRenderer>().enabled = false;
        }
        itemMarkerChild = markerList.ToArray();

        //instantiate items at markers
        itemChecktoSpawn();

        // Start battery spawn coroutine
        StartCoroutine(SpawnBatteryRoutine());
    }


    void itemChecktoSpawn()
    {
        switch (LevelSceneManager.currentLevel)
        { 
            case Level.Level1:
            
                foreach (var item in itemsToSpawn)
                {
                    ItemData data = item.GetComponent<ItemData>();

                    switch (data.itemType)
                    {
                        case ItemType.Goldfish:
                            Spawn(item, 2);
                            break;

                        case ItemType.Chest:
                            Spawn(item, 1);
                            break;
                        case ItemType.Key:
                            Spawn(item, 1);
                            break;
                    }
                }
                break;
            case Level.Level2:
                foreach (var item in itemsToSpawn)
                {
                    ItemData data = item.GetComponent<ItemData>();

                    switch (data.itemType)
                    {
                        case ItemType.Goldfish:
                            Spawn(item, 3);
                            break;

                        case ItemType.Fuse:
                            Spawn(item, 3);
                            break;
                    }
                }
                break;
        }
    }
    void Spawn(GameObject prefab, int count)
    {
        for (int i = 0; i < count; i++)
        {
            int index;
            // Find an unused index
            do
            {
                index = Random.Range(0, itemMarkerChild.Length);
            } while (usedIndices.Contains(index) && usedIndices.Count < itemMarkerChild.Length);

            usedIndices.Add(index);
            Instantiate(prefab, itemMarkerChild[index].position, Quaternion.identity);
        }
    }
}
