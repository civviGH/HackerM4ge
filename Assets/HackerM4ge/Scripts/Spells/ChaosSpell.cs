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

    TWandAction[] Spell.UpdateSpell(TriggerState rightTriggerState, Vector2 rightTouchpadAxis, Vector3 rightControllerPosition, Vector3 rightControllerDirection,
        TriggerState leftTriggerState, Vector2 leftTouchpadAxis, Vector3? leftControllerPosition, Vector3? leftControllerDirection)
    {
        if (rightTriggerState.down)
        {
            // check if wand is visible in the camera view
            Plane[] planes = GeometryUtility.CalculateFrustumPlanes(mainCamera);
            Bounds wandBounds = new Bounds(rightControllerPosition, new Vector3(0.1f, 0.1f, 0.1f));
            if (!GeometryUtility.TestPlanesAABB(planes, wandBounds))
            {
                potion = GameObject.Instantiate(potionPrefab, rightControllerPosition, new Quaternion(0f, 0f, 0f, 0f)) as GameObject;
            } 
            else
            {
                potion = null;
            }
        }
        else if (rightTriggerState.press && potion != null)
        {
            potion.transform.position = rightControllerPosition + (rightControllerDirection * 0.1f);
            lastWandPosition = rightControllerPosition;
        }
        else if (rightTriggerState.up && potion != null)
        {
            Rigidbody potionRigibody = potion.gameObject.GetComponent<Rigidbody> ();
            // TODO Das hier verwendet die Geschwindigkeit der Hand, nicht der Potion (die ja vor der Hand schwebt).
            // Wäre es nicht intuitiver, hier die Potion-Positionen zu verwenden?
            // Außerdem könnte man dann schön aus dem Handgelenk werfen. :)
            potionRigibody.velocity = Time.deltaTime * (rightControllerPosition - lastWandPosition) * 18000f;
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
