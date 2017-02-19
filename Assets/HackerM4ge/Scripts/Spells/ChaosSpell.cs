using System;
using UnityEngine;

public class ChaosSpell : Spell { 
    private GameObject potionPrefab;
    private Material material;
    private Camera mainCamera;
    private GameObject potion;
    Vector3 lastWandPosition;
    int currentPositionIndex;

    public ChaosSpell ()
    {
        potionPrefab = Resources.Load("ChaosPotion") as GameObject;
        material = Resources.Load ("ChaosSpellThumbnailMaterial", typeof(Material)) as Material;
        mainCamera = Camera.main;
        lastWandPosition = new Vector3();
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
            Bounds wandBounds = new Bounds(wandPosition, new Vector3(0.1f, 0.1f, 0.1f));
            if (!GeometryUtility.TestPlanesAABB(planes, wandBounds))
            {
                potion = GameObject.Instantiate(potionPrefab) as GameObject;
            } 
            else
            {
                potion = null;
            }
        }
        else if (triggerState.press && potion != null)
        {
            potion.transform.position = wandPosition + wandDirection;
            lastWandPosition = wandPosition;
        }
        else if (triggerState.up && potion != null)
        {
            Rigidbody potionRigibody = potion.gameObject.GetComponent<Rigidbody> ();
            potionRigibody.velocity = Time.deltaTime * (wandPosition - lastWandPosition) * 18000f;
            float torqueMultiplier = 100f;
            potionRigibody.AddTorque (
                UnityEngine.Random.Range (-torqueMultiplier, torqueMultiplier), 
                UnityEngine.Random.Range (-torqueMultiplier, torqueMultiplier), 
                UnityEngine.Random.Range (-torqueMultiplier, torqueMultiplier) * 100f );
        }
    }

    Union2<WandAction.Drain, WandAction.Vibrate> Spell.Select()
    {
        return new Union2<WandAction.Drain, WandAction.Vibrate>.Case2(new WandAction.Vibrate(500));
    }


    void Spell.Deselect() { }
}
