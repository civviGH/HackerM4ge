using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeteorController : MonoBehaviour {

	private Vector3 startPosition;
	private Vector3 targetPosition;

	private float speed;
	private float startTime;
	private float wayToTravel;

	public MeteorController(){
	}

	public void StartFalling(Vector3 targetPosition, float speed = 2f){
		this.targetPosition = targetPosition;
		this.speed = speed;
	}

	// Use this for initialization
	void Start () {
		this.startTime = Time.time;
		this.startPosition = this.transform.position;
		this.wayToTravel = Vector3.Distance (this.transform.position, this.targetPosition);
	}
	
	// Update is called once per frame
	void Update () {
		float distCovered = (Time.time - this.startTime) * this.speed;
		float fracJourney = distCovered / this.wayToTravel;
		this.transform.position = Vector3.Lerp (this.startPosition, targetPosition, fracJourney);
	}

	void OnTriggerEnter(){
		Destroy (gameObject, 0.1f);
	}
}
