using System;
using UnityEngine;

public class ChaosSpell : Spell { 
    private GameObject potionPrefab;
    private Material material;
    private Camera mainCamera;
    private GameObject potion;
    Vector3[] lastPositions;
    int currentPositionIndex;

    public ChaosSpell ()
    {
        potionPrefab = Resources.Load("ChaosPotion") as GameObject;
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
            Debug.Log ("trigger down chaos potion");
            // check if wand is visible in the camera view
            Plane[] planes = GeometryUtility.CalculateFrustumPlanes(mainCamera);
            Bounds wandBounds = new Bounds(wandPosition, new Vector3(0.1f, 0.1f, 0.1f));
            if (!GeometryUtility.TestPlanesAABB(planes, wandBounds))
            {
                Debug.Log ("potion spawn");
                potion = GameObject.Instantiate(potionPrefab) as GameObject;
            }
        }
        else if (triggerState.press && potion != null)
        {
            potion.transform.position = wandPosition + wandDirection;
            lastPositions[currentPositionIndex % 10] = wandPosition;

            for (int i=0; i<Math.Min(currentPositionIndex, 10); i++)
            {
                Debug.Log (lastPositions [i]);
 
            }
            Debug.Log ("-----");
            Debug.Log (currentPositionIndex);
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
            potion.gameObject.GetComponent<Rigidbody>().velocity = (wandPosition - sum) * 10f;
        }
    }

    public void Select() { }
    public void Deselect() { }
}
