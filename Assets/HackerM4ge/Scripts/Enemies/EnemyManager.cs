using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour {

    public GameObject enemyPrefab; // to be spawned
    public float spawnTime = 3f;
    public Transform[] sources;
    public Transform[] sinks;
    
	void Start () {
        InvokeRepeating("Spawn", spawnTime, spawnTime); // delay of spawn time and interval of spawn time
	}

    void Spawn() {
        int sourceIndex = Random.Range(0, sources.Length);

        Instantiate(enemyPrefab, sources[sourceIndex].position, sources[sourceIndex].rotation);
    }
    
	void Update () {
		
	}
}
