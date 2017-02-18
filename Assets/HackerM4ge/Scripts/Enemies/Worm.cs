using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Worm : Enemy {
    float health;
    float speed;
    Transform goal;

    public override void damage(float strength, DamageType type)
    {
        health -= strength;
    }

    public override void knockback(float strength)
    {
        throw new NotImplementedException();
    }

    public override void slowDown(float strength, float duration)
    {
        throw new NotImplementedException();
    }

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
