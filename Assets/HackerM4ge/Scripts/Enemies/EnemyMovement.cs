using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;

public class EnemyMovement : MonoBehaviour {
    private Enemy enemy;
    private NavMeshAgent nav;
    private Animator anim;
    private List<Vector3> transforms;
    private int transformIndex;

    void Awake() {
        nav = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
    }

    public void SetDestinations(Transform[] transforms)
    {
        this.transforms = new List<Vector3>();
        for (var i =0; i < transforms.Length; i++)
        {
            this.transforms.Add(transforms[i].position);
        }
        this.transformIndex = 0;
    }

    public void DetourTo(Vector3 transform)
    {
        this.transforms.Insert(this.transformIndex, transform);
    }

    // Update is called once per frame
    void Update() {
        while ((transformIndex < this.transforms.Count) && (this.transforms[transformIndex] == null))
        {
            transformIndex++;
        }
        if (transformIndex >= transforms.Count)
        {
            nav.enabled = false;
            return;
        }
        nav.SetDestination(transforms[transformIndex]);

        if (nav.enabled && enemy != null && enemy.GetHealth() <= 0f)
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
