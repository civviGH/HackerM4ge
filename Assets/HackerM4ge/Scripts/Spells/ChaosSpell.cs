using System;
using UnityEngine;

public class ChaosSpell : Spell { 
    private GameObject potionPrefab;
    private Material material;
    private Camera mainCamera;
    private GameObject potion;
    Vector3[] lastPositions;
    Vector3 lastPosition;
    int currentPositionIndex;

    public ChaosSpell ()
    {
        potionPrefab = Resources.Load("ChaosPotion") as GameObject;
        material = Resources.Load ("ChaosSpellThumbnailMaterial", typeof(Material)) as Material;
        mainCamera = Camera.main;
        lastPositions = new Vector3[10];
        lastPosition = new Vector3();
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
            Debug.Log("trigger down chaos potion");
            // check if wand is visible in the camera view
            Plane[] planes = GeometryUtility.CalculateFrustumPlanes(mainCamera);
            Bounds wandBounds = new Bounds(wandPosition, new Vector3(0.1f, 0.1f, 0.1f));
            if (!GeometryUtility.TestPlanesAABB(planes, wandBounds))
            {
                Debug.Log("potion spawn");
                potion = GameObject.Instantiate(potionPrefab) as GameObject;
            }
        }
        else if (triggerState.press && potion != null)
        {
            potion.transform.position = wandPosition + wandDirection;
            lastPositions[currentPositionIndex % 10] = wandPosition;
            lastPosition = wandPosition;
            currentPositionIndex++;
        }
        else if (triggerState.up && potion != null)
        {
            /* Vector3 sum = new Vector3(0f, 0f, 0f);
            int Length = Math.Min(currentPositionIndex, 10);
            for (int i=0; i<Length; i++)
            {
                sum += lastPositions[i];
            }
            sum /= Length; // TODO division by zero
            */
            Rigidbody potionRigibody = potion.gameObject.GetComponent<Rigidbody> ();
            potionRigibody.velocity = Time.deltaTime * (wandPosition - lastPosition) * 18000f;
            float torqueMultiplier = 100f;
            potionRigibody.AddTorque (
                UnityEngine.Random.Range (-torqueMultiplier, torqueMultiplier), 
                UnityEngine.Random.Range (-torqueMultiplier, torqueMultiplier), 
                UnityEngine.Random.Range (-torqueMultiplier, torqueMultiplier) * 100f );
        }
    }

    public void Select() { }
    public void Deselect() { }
}
