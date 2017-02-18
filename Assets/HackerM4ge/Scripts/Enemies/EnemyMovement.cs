using UnityEngine;
using UnityEngine.AI;

public class EnemyMovement : MonoBehaviour {
    Enemy enemy;
    NavMeshAgent nav;
    Animator anim;

    void Awake() {
        nav = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
    }

    public void SetDestination(Transform transform)
    {
        nav.SetDestination(transform.position);
    }

    // Update is called once per frame
    void Update() {
        if (nav.enabled && enemy != null && enemy.GetHealth() <= 0f)
        {
            anim.SetTrigger("Dead");
        }
        // nav.enabled = false; TODO set this, if Player is dead
    }

    internal void SetEnemy(Enemy newEnemy)
    {
        enemy = newEnemy;
    }
}
