using System;
using UnityEngine;

using TWandAction = Union2<WandAction.Drain, WandAction.Vibrate>;

public class ChaosSpell : Spell
{
    private GameObject potionPrefab;
    private Material material;
    private Camera mainCamera;
    private GameObject potion;
    Vector3 lastWandPosition;
    int currentPositionIndex;

    public ChaosSpell()
    {
        potionPrefab = Resources.Load("ChaosPotion") as GameObject;
        material = Resources.Load("ChaosSpellThumbnailMaterial", typeof(Material)) as Material;
        mainCamera = Camera.main;
        lastWandPosition = new Vector3();
        currentPositionIndex = 0;
    }

    public string GetName()
    {
        return "Chaos";
    }

    public Material GetThumbnail()
    {
        return material;
    }

    TWandAction[] Spell.UpdateSpell(TriggerState triggerState, Vector2 touchpadAxis, Vector3 wandPosition, Vector3 wandDirection)
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
            // TODO Das hier verwendet die Geschwindigkeit der Hand, nicht der Potion (die ja vor der Hand schwebt).
            // Wäre es nicht intuitiver, hier die Potion-Positionen zu verwenden?
            // Außerdem könnte man dann schön aus dem Handgelenk werfen. :)
            potionRigibody.velocity = Time.deltaTime * (wandPosition - lastWandPosition) * 18000f;
            potionRigibody.constraints = RigidbodyConstraints.None;
            float torqueMultiplier = 100f;
            potionRigibody.AddTorque(
                UnityEngine.Random.Range(-torqueMultiplier, torqueMultiplier),
                UnityEngine.Random.Range(-torqueMultiplier, torqueMultiplier),
                UnityEngine.Random.Range(-torqueMultiplier, torqueMultiplier) * 100f);
        }

        TWandAction[] actions = { };
        return actions;
    }

    TWandAction[] Spell.Select()
    {
        TWandAction[] actions = {
            new TWandAction.Case2(new WandAction.Vibrate(500)),
        };
        return actions;
    }


    TWandAction[] Spell.Deselect()
    {
        TWandAction[] actions = { };
        return actions;
    }
}
