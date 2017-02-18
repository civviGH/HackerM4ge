using System;
using UnityEngine;

public class ChaosSpell : MonoBehaviour, Spell {
    private Material material;

    public ChaosSpell()
    {
        material = Resources.Load("ChaosSpellThumbnailMaterial", typeof(Material)) as Material; 
    }

    public string GetName()
    {
        return "Chaos";
    }

    public Material GetThumbnail()
    {
        return material;
    }

    public void UpdateSpell(TriggerState triggerState, Vector2 touchpadAxis, Vector3 wandPosition, Vector3 wandDirection)
    {
        Debug.Log(wandPosition);
    }

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
