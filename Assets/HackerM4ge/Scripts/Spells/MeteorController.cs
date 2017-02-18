using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeteorController : MonoBehaviour {

  //private Vector3 startPosition;
  //private Vector3 targetPosition;
  private Vector3 direction;

  public GameObject explosionSoundObject;

  public MeteorController(){
  }

  public void StartFalling(Vector3 targetPosition){
    this.direction = targetPosition - this.transform.position;
    this.direction.Normalize();
    gameObject.GetComponent<Rigidbody> ().velocity = this.direction * 25f;
    Debug.Log (this.direction * 10f);
    Debug.Log (gameObject);
  }

  // Use this for initialization
  void Start () {
    /*this.startTime = Time.time;
    this.startPosition = this.transform.position;
    this.wayToTravel = Vector3.Distance (this.transform.position, this.targetPosition);
*/
  }

  // Update is called once per frame
  void Update () {
    
  }

  void OnTriggerEnter(){
    explosionSoundObject.transform.parent = null;
    explosionSoundObject.GetComponent<AudioSource> ().Play ();
    Destroy(explosionSoundObject, 3f);
    Destroy (gameObject, 0.1f);
  }
}
