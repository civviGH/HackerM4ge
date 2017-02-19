using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightningSpell : MonoBehaviour
{

    private Material thumbnail;

    private GameObject lightningPrefab;

    public LightningSpell () {
        this.thumbnail = Resources.Load ("LightningThumbnailMaterial") as Material;
        this.lightningPrefab = Resources.Load ("LightningSpellPrefabs/LightningtPrefab") as GameObject;
    }
    // Use this for initialization
    void Start ()
    {
		
    }
	
    // Update is called once per frame
    void Update ()
    {
		
    }
}
