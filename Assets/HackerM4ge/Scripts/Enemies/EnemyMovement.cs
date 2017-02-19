using UnityEngine;
using UnityEngine.AI;

public class EnemyMovement : MonoBehaviour {
    private Enemy enemy;
    private NavMeshAgent nav;
    private Animator anim;
    private Transform[] transforms;
    private int transformIndex;

    void Awake() {
        nav = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
    }

    public void SetDestinations(Transform[] transforms)
    {
        this.transformIndex = 0;
        this.transforms = transforms;
    }

    // Update is called once per frame
    void Update() {
        while ((transformIndex < transforms.Length) && (transforms[transformIndex] == null))
        {
            transformIndex++;
        }
        if (transformIndex >= transforms.Length)
        {
            nav.enabled = false;
            return;
        }
        nav.SetDestination(transforms[transformIndex].position);

        if (nav.enabled && enemy != null && enemy.GetHealth() <= 0f)
        {
            anim.SetTrigger("Dead");
        }
    }

    internal void SetEnemy(Enemy newEnemy)
    {
        enemy = newEnemy;
    }
}
