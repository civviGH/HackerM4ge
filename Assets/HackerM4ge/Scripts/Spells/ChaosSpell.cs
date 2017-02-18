using System;
using UnityEngine;

public class ChaosSpell : Spell
{
    private Material material;

    private Camera mainCamera;
    public ChaosSpell ()
    {
        material = Resources.Load ("ChaosSpellThumbnailMaterial", typeof(Material)) as Material;
        mainCamera = Camera.main;
    }

    public string GetName ()
    {
        return "Chaos";
    }

    public Material GetThumbnail ()
    {
        return material;
    }

    public void UpdateSpell (TriggerState triggerState, Vector2 touchpadAxis, Vector3 wandPosition, Vector3 wandDirection)
    {
        if (triggerState.down)
        {
            Plane[] planes = GeometryUtility.CalculateFrustumPlanes(mainCamera);
            Bounds wandBounds = new Bounds(wandPosition, new Vector3(1, 1, 1));
            if (GeometryUtility.TestPlanesAABB(planes, wandBounds))
            {
                Debug.Log("Test");
            }
        }
    }

    // Use this for initialization
    void Start ()
    {
		
    }
	
    // Update is called once per frame
    void Update ()
    {

    }

    public void Select() { }
    public void Deselect() { }
}
