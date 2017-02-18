using System;
using UnityEngine;

abstract public class Enemy : MonoBehaviour {
    private float health;

    public Enemy() {
        ResetHealth();
    }

    internal float GetHealth()
    {
        return health; // TODO use short syntax instead
    }

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

    void OnTriggerEnter(Collider collider)
    {
        if (collider.gameObject.CompareTag("EnemyDestination"))
        {
            collider.gameObject.GetComponent<AudioSource>().Play();
            Destroy(gameObject);
        }
    }
}
