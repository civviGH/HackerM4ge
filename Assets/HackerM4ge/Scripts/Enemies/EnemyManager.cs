using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour {

    public GameObject spiderPrefab;
    public GameObject insectPrefab;
    public float spawnTime = 3f;
    public Transform[] spawners;
    public Transform[] homeNetworks;
    
	void Start () {
        InvokeRepeating("Spawn", spawnTime, spawnTime); // delay of spawn time and interval of spawn time
	}

    void Spawn() {
        int remainingSpiderCount = 0;
        int remainingInsectCount = 0;
        SpawnPoint spawnPoint = null;

        foreach(Transform spawner in spawners)
        {
            remainingSpiderCount += spawner.gameObject.GetComponent<SpawnPoint>().spiderCount;
            remainingInsectCount += spawner.gameObject.GetComponent<SpawnPoint>().insectCount;
        }
        
        int rand = Random.Range(0, remainingSpiderCount + remainingInsectCount);
        GameObject prefab;
        if (rand < remainingSpiderCount)
        {
            prefab = spiderPrefab;
            foreach (Transform spawner in spawners)
            {
                spawnPoint = spawner.gameObject.GetComponent<SpawnPoint>();
                rand -= spawnPoint.spiderCount;
                if (rand < 0) break;
            }
            spawnPoint.spiderCount--;

        } else
        {
            prefab = insectPrefab;
            rand -= remainingSpiderCount;
            foreach (Transform spawner in spawners)
            {
                spawnPoint = spawner.gameObject.GetComponent<SpawnPoint>();
                rand -= spawnPoint.insectCount;
                if (rand < 0) break;
            }
            spawnPoint.insectCount--;
        }

        int homeNetworkIndex = Random.Range(0, homeNetworks.Length);

        if (spawnPoint != null)
        {
            Instantiate(prefab, spawnPoint.transform.position, spawnPoint.transform.rotation);
        }
    }
    
	void Update () {
		
	}
}
