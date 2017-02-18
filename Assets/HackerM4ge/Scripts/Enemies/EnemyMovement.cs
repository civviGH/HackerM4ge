using UnityEngine;
using UnityEngine.AI;

public class EnemyMovement : MonoBehaviour {
    Enemy enemy;
    NavMeshAgent nav;

    void Awake() {
        nav = GetComponent<NavMeshAgent> ();
    }

	// Use this for initialization
	void Start () {
        enemy = GameObject.FindGameObjectWithTag("Enemy").GetComponent<Enemy>();
        GameObject destination = GameObject.FindGameObjectWithTag("EnemyDestination");
        nav.SetDestination(destination.transform.position);
    }
	
	// Update is called once per frame
	void Update () {
		if (enemy.GetHealth() <= 0f)
        {
            nav.enabled = false;
        }
	}
}
