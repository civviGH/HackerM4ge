using System;
using UnityEngine;

public class ChaosSpell : Spell
{
    private UnityEngine.Object potionPrefab;
    private Material material;
    private Camera mainCamera;
    private GameObject potion;
    Vector3[] lastPositions;
    int currentPositionIndex;

    public ChaosSpell ()
    {
        potionPrefab = Resources.Load("ChaosPotion");
        material = Resources.Load ("ChaosSpellThumbnailMaterial", typeof(Material)) as Material;
        mainCamera = Camera.main;
        lastPositions = new Vector3[10];
        currentPositionIndex = 0;
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
            // check if wand is visible in the camera view
            Plane[] planes = GeometryUtility.CalculateFrustumPlanes(mainCamera);
            Bounds wandBounds = new Bounds(wandPosition, new Vector3(1f, 1f, 1f));
            if (!GeometryUtility.TestPlanesAABB(planes, wandBounds))
            {
                potion = GameObject.Instantiate(potionPrefab) as GameObject;
            }
        }
        else if (triggerState.press && potion != null)
        {
            potion.transform.position = wandPosition + wandDirection;
            lastPositions[currentPositionIndex % 10] = wandPosition;
            currentPositionIndex++;
        }
        else if (triggerState.up && potion != null)
        {
            Vector3 sum = new Vector3(0f, 0f, 0f);
            int Length = Math.Min(currentPositionIndex, 10);
            for (int i=0; i<Length; i++)
            {
                sum += lastPositions[i];
            }
            sum /= Length; // TODO division by zero
            potion.gameObject.GetComponent<Rigidbody>().velocity = wandPosition - sum;
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
