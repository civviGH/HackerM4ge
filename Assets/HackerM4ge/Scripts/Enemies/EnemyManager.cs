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

    Transform[] CloneAndShuffleHomeNetworks()
    {
        Transform[] ret = new Transform[homeNetworks.Length];
        for (int i = 0; i < homeNetworks.Length; i++)
        {
            ret[i] = homeNetworks[i];
        }
        for (int i = homeNetworks.Length - 1; i > 0; i--)
        {
            int rand = Random.Range(0, i + 1);
            Transform tmp = ret[i];
            ret[i] = ret[rand];
            ret[rand] = tmp;
        }
        return ret;
    }

    IEnumerator waitAndSpwan(SpawnPoint spawner)
    {
        int wait = Random.Range(0, 60);
        yield return new WaitForSeconds(1f / wait);
        
        spawner.SpawnRandom(CloneAndShuffleHomeNetworks());
    }
}
