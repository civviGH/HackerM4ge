using System;
using UnityEngine;

abstract public class Enemy : MonoBehaviour {
    private float health;
    protected int networkDamage;

    public Enemy() {
        ResetHealth();
    }

    internal float GetHealth()
    {
        return health; // TODO use short syntax instead
    }

    private float speed;

    public enum DamageType { Fire, Water, Earth, Wind, Physical, Electricity, Love, Light };

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
        Debug.Log ("Damage: " + strength + " | Leben: " + health); 
        if (health <= 0f) {
            Debug.Log ("ich wär gestorben.");
            Destroy (gameObject);
        }
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
            collider.gameObject.GetComponent<HomeNetworkScript> ().Damage (networkDamage);
            Destroy(gameObject);
        }
    }
}
