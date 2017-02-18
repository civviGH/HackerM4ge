using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeteorController : MonoBehaviour {

    private Vector3 direction;

    public GameObject explosionSoundObject;
    public GameObject fireSoundObject;
    public GameObject explosionParticleObject;
    public GameObject fireShrapnelObject;

    public MeteorController(){
    }

  public void StartFalling(Vector3 targetPosition){
    this.direction = targetPosition - this.transform.position;
    this.direction.Normalize();
    gameObject.GetComponent<Rigidbody> ().velocity = this.direction * 25f;
  }

  // Use this for initialization
  void Start () {
  }

  // Update is called once per frame
  void Update () {
    
  }

  void OnTriggerEnter(Collider hitObject){
    if (hitObject.GetComponent<Collider> ().gameObject.layer == LayerMask.NameToLayer ("Surfaces")) {
      // fade out fire sound
      fireSoundObject.transform.parent = null;
            fireSoundObject.GetComponent<FireSoundObjectScript> ().FadeOut ();

      // explosionsound
      explosionSoundObject.transform.parent = null;
      explosionSoundObject.GetComponent<AudioSource> ().Play ();

      // explosion particles
            explosionParticleObject.transform.parent = null;
            explosionParticleObject.GetComponent<ParticleSystem> ().Play();

            // shrapnels
            fireShrapnelObject.transform.parent = null;
            fireShrapnelObject.GetComponent<ParticleSystem> ().Play();

      // destroy objects
            Destroy(explosionParticleObject, 1f);
            Destroy (fireShrapnelObject, 1f);
      Destroy (fireSoundObject, 1.5f);
      Destroy (explosionSoundObject, 2f);
      Destroy (gameObject);
    }
  }
}
