using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthManagerScript : MonoBehaviour {

    private Material skyboxMaterial;
    private Color originalSkyboxColor = new Color(0.73f, 0.73f, 0.73f, 1f);
    private List<GameObject> homeNetworks = new List<GameObject>();

    // Use this for initialization
    void Start () {
        // main camera skybox
        this.skyboxMaterial = GameObject.Find ("Camera (eye)").GetComponent<Skybox>().material;
        skyboxMaterial.SetColor("_Tint", this.originalSkyboxColor);
    }
	
    // Update is called once per frame
    void Update () {
        
    }

    public void RegisterNetwork(GameObject network){
        this.homeNetworks.Add (network);
    }

    public void LeaveNetwork(GameObject network){
        this.homeNetworks.Remove (network);
        if (this.homeNetworks.Count == 0) {
            Debug.Log ("GAMEOVER BITCHES");
        }
    }

    public void DamageFlash(){
        StartCoroutine (SkyboxColorFlash ());
    }

    IEnumerator SkyboxColorFlash(){
        this.skyboxMaterial.SetColor ("_Tint", new Color (1f, 0f, 0f, 1f));
        yield return new WaitForSeconds (0.1f);
        this.skyboxMaterial.SetColor ("_Tint", this.originalSkyboxColor);
    }
}
