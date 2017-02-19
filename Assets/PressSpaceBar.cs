using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PressSpaceBar : MonoBehaviour {

    private Material skyboxMaterial;
    public Camera playerCam;


    // Use this for initialization
    void Start () {
        this.skyboxMaterial = gameObject.GetComponent<Skybox> ().material;
    }

	// Update is called once per frame
    void Update () {
        if (Input.GetKeyDown ("space")) {
            RenderSettings.skybox.SetColor ("_Tint", new Color (1f, 0f, 0f, 1f));
            Debug.Log ("rot");
        }
        if (Input.GetKeyDown ("k")) {
            StartCoroutine (normalColor());
        }
        if (Input.GetKeyDown ("u")) {
            Debug.Log ("Cam: " + playerCam);
            Debug.Log ("Color: " + this.playerCam.backgroundColor);
        }
    }


    IEnumerator normalColor(){
        this.skyboxMaterial.SetColor ("_Tint", new Color (1f, 0f, 0f, 1f));
        yield return new WaitForSeconds (0.1f);
        this.skyboxMaterial.SetColor ("_Tint", new Color (1f, 1f, 1f, 1f));
    }
}
