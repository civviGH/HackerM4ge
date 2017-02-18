using UnityEngine;
using System.Collections;

public class MeteorSpell : Spell {

	public Material Thumbnail;

	private GameObject previewSpherePrefab;
	private GameObject previewSphere;

	public MeteorSpell(){
		this.Thumbnail = Resources.Load ("MeteorThumbnailMaterial") as Material;
		this.previewSpherePrefab = Resources.Load ("MeteorSpellPrefabs/MeteorPreviewPrefab") as GameObject;
	}

	public void UpdateSpell(
		TriggerState triggerState, 
		Vector2 touchpadAxis, 
		Vector3 wandPosition,
		Vector3 wandDirection){

		if (triggerState.up)
			RightTriggerUp ();
		if (triggerState.down)
			RightTriggerDown (wandPosition, wandDirection);
		if (triggerState.press)
			RightTriggerPress (wandPosition, wandDirection);
	}

	private void RightTriggerDown(Vector3 wandPosition, Vector3 wandDirection){
		RaycastHit hitObject;
		Ray ray = new Ray (wandPosition, wandDirection);
		if (Physics.Raycast (ray, out hitObject)) {
			this.previewSphere = GameObject.Instantiate(this.previewSpherePrefab, hitObject.point, new Quaternion (0f, 0f, 0f, 0f)) as GameObject;
		}
	}

	private void RightTriggerPress(Vector3 wandPosition, Vector3 wandDirection){
		RaycastHit hitObject;
		LayerMask raycastLayerMask = ~(1 << 2);
		Ray ray = new Ray (wandPosition, wandDirection);
		if (Physics.Raycast (ray, out hitObject, raycastLayerMask)) {
			Debug.Log (hitObject.point);
			this.previewSphere.transform.position = hitObject.point;
		}
	}

	private void RightTriggerUp(){
		UnityEngine.Object.Destroy (this.previewSphere);
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
