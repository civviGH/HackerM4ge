using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour {
    public float spawnTime = 3f;
    public Transform[] spawners;
    public Transform[] homeNetworks;
    
	void Start () {
        InvokeRepeating("Spawn", spawnTime, spawnTime); // delay of spawn time and interval of spawn time
	}

    void Spawn() {
        int remainingCount = 0;
        SpawnPoint selectedSpawner = null;

        foreach(Transform spawner in spawners)
        {
            remainingCount += spawner.gameObject.GetComponent<SpawnPoint>().GetTotalCount();
        }
        
        int rand = Random.Range(0, remainingCount);

        foreach (Transform spawner in spawners)
        {
            rand -= spawner.gameObject.GetComponent<SpawnPoint>().GetTotalCount();
            if (rand < 0)
            {
                selectedSpawner = spawner.gameObject.GetComponent<SpawnPoint>();
                break;
            }
        }

        if (selectedSpawner == null) return;

        StartCoroutine(waitAndSpwan(selectedSpawner));
    }

    IEnumerator waitAndSpwan(SpawnPoint spawner)
    {
        int wait = Random.Range(0, 60);
        yield return new WaitForSeconds(1f / wait);
        spawner.SpawnRandom(homeNetworks[Random.Range(0, homeNetworks.Length)]);
    }
}
