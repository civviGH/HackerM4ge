using System.Collections;
using System.Collections.Generic;
using UnityEngine;

abstract public class Enemy : MonoBehaviour {
    private float health;
    private float speed;

    public enum DamageType { Fire, Water, Earth, Wind, Physical, Electricity, Love};

    public void Knockback(float strength) {
        // TODO
    }
    public void SlowDown(float strength, float duration)
    {
        speed *= strength;
        Invoke("ResetSpeed", duration);
    }

    public void damage(float strength, DamageType type)
    {
        health -= strength;
    }
   
    // Use this for initialization
    void Start () {
        ResetSpeed();
        ResetHealth();
	}
	
	// Update is called once per frame
	void Update () {

    }

    private void ResetSpeed()
    {
        speed = 1;
    }

    private void ResetHealth()
    {
        health = 80;
    }
}
