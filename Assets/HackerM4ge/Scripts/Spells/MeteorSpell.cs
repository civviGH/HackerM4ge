using UnityEngine;
using System.Collections;

public class MeteorSpell : Spell {

	public Material Thumbnail;

	private GameObject previewSpherePrefab;
	private GameObject previewSphere;
	private GameObject meteorPrefab;
	private LayerMask raycastLayerMask;

	public MeteorSpell(){
		this.Thumbnail = Resources.Load ("MeteorThumbnailMaterial") as Material;
		this.previewSpherePrefab = Resources.Load ("MeteorSpellPrefabs/MeteorPreviewPrefab") as GameObject;
		this.raycastLayerMask = LayerMask.GetMask ("Surfaces");
		this.meteorPrefab = Resources.Load ("MeteorSpellPrefabs/Meteor2") as GameObject;
	}

	public void UpdateSpell(
		TriggerState triggerState, 
		Vector2 touchpadAxis, 
		Vector3 wandPosition,
		Vector3 wandDirection){

		if (triggerState.press)
			RightTriggerPress (wandPosition, wandDirection);
		if (triggerState.up)
			RightTriggerUp ();
	}

	private void RightTriggerDown(){
	}

	private void RightTriggerPress(Vector3 wandPosition, Vector3 wandDirection){
		RaycastHit hitObject;
		Ray ray = new Ray (wandPosition, wandDirection);
		if (Physics.Raycast (ray, out hitObject, Mathf.Infinity, this.raycastLayerMask)) {
			if (this.previewSphere)
				this.previewSphere.transform.position = hitObject.point;
			else {
				this.previewSphere = GameObject.Instantiate (this.previewSpherePrefab, hitObject.point, new Quaternion ());
			}
		} else {
			if (this.previewSphere)
				UnityEngine.Object.Destroy (this.previewSphere);
		}
	}

	private void RightTriggerUp(){
		if (this.previewSphere) {
			SpawnMeteorToPosition (previewSphere.transform.position);	
			UnityEngine.Object.Destroy (this.previewSphere);
		}
	}

	private void SpawnMeteorToPosition(Vector3 targetArea){
		// instantiate meteor
		GameObject actualMeteor = GameObject.Instantiate (
			                          this.meteorPrefab, targetArea + new Vector3 (
				                          Random.Range (-5f, 5f),
				                          20,
				                          Random.Range (-5f, 5f)),
			                          new Quaternion ());
		actualMeteor.GetComponent<MeteorController> ().StartFalling (targetArea, 20f);
		actualMeteor.GetComponent<AudioSource> ().Play ();
	}
	
	public Material GetThumbnail(){
		return this.Thumbnail;
	}

	public string GetName(){
		return "Meteor";
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
