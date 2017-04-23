using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeteorController : MonoBehaviour
{

    private Vector3 direction;

    private const float explosionRadius = 5f;

    public GameObject explosionSoundObject;
    public GameObject fireSoundObject;
    public GameObject explosionParticleObject;
    public GameObject fireShrapnelObject;

    private float speed = 1f;

    private Vector3 targetArea;

    public MeteorController ()
    {
    }

    public void DestroyMeteor(){
        Destroy (gameObject);
    }

    public void SetTargetArea(Vector3 targetArea){
        this.targetArea = targetArea;
    }

    public void SetSpeed(float speed){
        this.speed = speed;
    }

    public void StartFalling ()
    {
        this.direction = targetArea - this.transform.position;
        this.direction.Normalize ();
        gameObject.GetComponent<Rigidbody> ().velocity = this.direction * speed;
    }

    // Use this for initialization
    void Start ()
    {
    }

    // Update is called once per frame
    void Update ()
    {
    
    }

    void OnTriggerEnter (Collider hitObject)
    {
        if (hitObject.GetComponent<Collider> ().gameObject.layer == LayerMask.NameToLayer ("Surfaces")) {
            // deal damage
            Collider[] damagedEnemies = Physics.OverlapSphere (
                                            gameObject.transform.position, 
                                            explosionRadius, 
                                            LayerMask.GetMask ("Damagable"));

            foreach (Collider damagedEnemy in damagedEnemies){
                damagedEnemy.transform.gameObject.GetComponent<Enemy>().damage(100f, Enemy.DamageType.Fire);
            }

            // fade out fire sound
            fireSoundObject.transform.parent = null;
            fireSoundObject.GetComponent<FireSoundObjectScript> ().FadeOut ();

            // explosionsound
            explosionSoundObject.transform.parent = null;
            explosionSoundObject.GetComponent<AudioSource> ().Play ();

            // explosion particles
            explosionParticleObject.transform.parent = null;
            explosionParticleObject.GetComponent<ParticleSystem> ().Play ();

            // shrapnels
            fireShrapnelObject.transform.parent = null;
            fireShrapnelObject.GetComponent<ParticleSystem> ().Play ();

            // destroy objects
            Destroy (explosionParticleObject, 1f);
            Destroy (fireShrapnelObject, 1f);
            Destroy (fireSoundObject, 1.5f);
            Destroy (explosionSoundObject, 2f);
            Destroy (gameObject);
        }
    }
}
