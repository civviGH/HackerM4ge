using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;

public class EnemyMovement : MonoBehaviour {
    private Enemy enemy;
    private NavMeshAgent nav;
    private Animator anim;
    private List<Vector3> positions;
    private int transformIndex;

    void Awake() {
        nav = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
    }

    public void SetDestinations(Transform[] transforms)
    {
        this.positions = new List<Vector3>();
        for (var i =0; i < transforms.Length; i++)
        {
            this.positions.Add(transforms[i].position);
        }
        this.transformIndex = 0;
    }

    public void DetourTo(Vector3 transform)
    {
        this.positions.Insert(this.transformIndex, transform);
    }

    // Update is called once per frame
    void Update() {
        if (!nav.isActiveAndEnabled) {
            return;
        }
        if (!nav.pathPending && nav.remainingDistance <= nav.stoppingDistance && (!nav.hasPath || nav.velocity.sqrMagnitude == 0f))
        {
            transformIndex++;
        }

        while ((transformIndex < this.positions.Count) && (this.positions[transformIndex] == null))
        {
            transformIndex++;
        }

        if (transformIndex >= positions.Count)
        {
            nav.enabled = false;
            return;
        }

        nav.SetDestination(positions[transformIndex]);
        if (enemy != null && enemy.GetHealth() <= 0f)
        {
            anim.SetTrigger("Dead");
        }
    }

    internal void SetEnemy(Enemy newEnemy)
    {
        enemy = newEnemy;
        enemy.movement = this;
    }
}
