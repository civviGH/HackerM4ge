using UnityEngine;

using TWandAction = Union2<WandAction.Drain, WandAction.Vibrate>;

class LasertrapSpell : Spell
{
    const float minimalDistance = 0.2f;
    const float maxSpeed = 15f;
    const float lifetime = 20f;

    private GameObject laserSourcePrefab;
    private Material laserBeamHazardMaterial;
    private LayerMask surfaceLayer;
    private Material laserSourceBlueprintMaterial;
    private Material laserSourceMaterial;

    private GameObject rightHandTrapSource;
    private GameObject leftHandTrapSource;

    private bool rightHandTrapSourcePlacing;
    private bool leftHandTrapSourcePlacing;
    private bool rightHandTrapSourcePlaced;
    private bool leftHandTrapSourcePlaced;
    private bool rightHandTrapSourceThrowing;
    private bool leftHandTrapSourceThrowing;
    private bool rightHandTrapSourceThrown;
    private bool leftHandTrapSourceThrown;

    /*
     * TODO:
     * - Nach dem Setzen nicht direkt den Laser einschalten, sondern Traps erst noch "werfen"
     * - Trap-Positionen beim Setzen besser sichtbar machen: Strahl in den Himmel z.B.?
     * - Schadenssschema ändern: Gesamtschaden statt Lebensdauer beschränken, dafür mehr DPS machen (evtl. einfach unbegrenzt, zumindest aber genug um normale Gegner zu töten bevor sie durchlaufen können)
     * - Traps erst ab "Placing" sichtbar machen, oder vielleicht erst nur an der Hand sichtbar machen und ab Placing in der Ferne
     */

    /*
     * TODO:
     * MonoBehaviour entfernen. Dafür muss auf Instantiate() Aufrufe verzichtet werden.
     */

    public LasertrapSpell()
    {
        // TODO Ressourcen in der GUI setzen. Dafür müssen die Spells von vorne herein an irgendein GameObject...
        laserSourcePrefab = Resources.Load<GameObject>("LaserTrapSpellPrefabs/LaserSourcePrefab");
        laserSourceMaterial = laserSourcePrefab.GetComponent<MeshRenderer>().sharedMaterials[0];
        laserSourceBlueprintMaterial = Resources.Load<Material>("LaserTrapSpellPrefabs/Materials/laserSourceBlueprintMaterial");
        laserSourceBlueprintMaterial = Resources.Load<Material>("LaserTrapSpellPrefabs/Materials/laserSourceBlueprintMaterial");
        laserBeamHazardMaterial = Resources.Load<Material>("LaserTrapSpellPrefabs/LaserBeamHazard");
        surfaceLayer = LayerMask.GetMask("Surfaces");
    }

    private void Init()
    {
        rightHandTrapSource = Object.Instantiate(laserSourcePrefab);
        leftHandTrapSource = Object.Instantiate(laserSourcePrefab);
        Material[] materials = { laserSourceBlueprintMaterial, laserSourceBlueprintMaterial };
        rightHandTrapSource.GetComponent<MeshRenderer>().materials = materials;
        leftHandTrapSource.GetComponent<MeshRenderer>().materials = materials;
        rightHandTrapSourcePlacing = false;
        leftHandTrapSourcePlacing = false;
        rightHandTrapSourcePlaced = false;
        leftHandTrapSourcePlaced = false;
        rightHandTrapSourceThrowing = false;
        leftHandTrapSourceThrowing = false;
        rightHandTrapSourceThrown = false;
        leftHandTrapSourceThrown = false;
    }

    string Spell.GetName()
    {
        return GetType().Name;
    }

    Material Spell.GetThumbnail()
    {
        return laserBeamHazardMaterial;
    }

    TWandAction[] Spell.UpdateSpell(TriggerState rightTriggerState, Vector2 rightTouchpadAxis, Vector3 rightControllerPosition, Vector3 rightControllerDirection,
        TriggerState leftTriggerState, Vector2 leftTouchpadAxis, Vector3? leftControllerPosition, Vector3? leftControllerDirection)
    {
        UpdateControllerStateAndTrap(rightTriggerState, rightTouchpadAxis, rightControllerPosition, rightControllerDirection,
            ref rightHandTrapSourcePlacing, ref rightHandTrapSourcePlaced, ref rightHandTrapSourceThrowing, ref rightHandTrapSourceThrown, ref rightHandTrapSource);
        UpdateControllerStateAndTrap(leftTriggerState, leftTouchpadAxis, leftControllerPosition, leftControllerDirection,
            ref leftHandTrapSourcePlacing, ref leftHandTrapSourcePlaced, ref leftHandTrapSourceThrowing, ref leftHandTrapSourceThrown, ref leftHandTrapSource
        );

        // when both are placed, finish and reset
        if (rightHandTrapSourceThrown && leftHandTrapSourceThrown)
        {
            GameObject laser = CreateLaser(rightHandTrapSource.transform.position, leftHandTrapSource.transform.position);
            Object.Destroy(rightHandTrapSource, lifetime);
            Object.Destroy(leftHandTrapSource, lifetime);
            Object.Destroy(laser, lifetime);

            Init();
        }

        TWandAction[] actions = { };
        return actions;
    }

    private void UpdateControllerStateAndTrap(TriggerState triggerState, Vector2 touchpadAxis, Vector3? controllerPosition, Vector3? controllerDirection,
        ref bool trapSourcePlacing, ref bool trapSourcePlaced, ref bool trapSourceThrowing, ref bool trapSourceThrown, ref GameObject trapSource
        )
    {
        if(controllerPosition == null || controllerDirection == null)
        {
            // Controller isn't active
            return;
        }

        if (!trapSourcePlaced)
        {
            UpdateTrapSource(ref trapSource, touchpadAxis, controllerPosition.Value, controllerDirection.Value);
        }

        if (!trapSourcePlacing)
        {
            if (triggerState.down && !trapSourcePlaced)
            {
                trapSourcePlacing = true;
            }
        }

        if (trapSourcePlacing && !trapSourcePlaced)
        {
            if (triggerState.up)
            {
                if (NextSurfacePosition(controllerPosition.Value, controllerDirection.Value) != null)
                {
                    trapSourcePlaced = true;
                }
                else
                {
                    trapSourcePlacing = false;
                }
            }
        }

        if (trapSourcePlaced && !trapSourceThrowing)
        {
            if (triggerState.down)
            {
                trapSourceThrowing = true;
            }
        }

        if (trapSourceThrowing && !trapSourceThrown)
        {
            if (triggerState.up)
            {
                trapSourceThrown = true;
                Material[] materials = { laserSourceMaterial, laserSourceMaterial };
                trapSource.GetComponent<MeshRenderer>().materials = materials;
            }
        }
    }

    private void UpdateTrapSource(ref GameObject trapSource, Vector2 touchpadAxis, Vector3 wandPosition, Vector3 wandDirection)
    {
        UpdateTrapSourcePosition(ref trapSource, wandPosition, wandDirection);
    }

    private void UpdateTrapSourcePosition(ref GameObject trapSource, Vector3 wandPosition, Vector3 wandDirection)
    {
        Vector3? surfacePosition = NextSurfacePosition(wandPosition, wandDirection);
        if (surfacePosition != null)
        {
            trapSource.transform.position = surfacePosition.Value + Vector3.up * 0.3f;
        }
        else
        {
            trapSource.transform.position = wandPosition + wandDirection * minimalDistance;
        }
    }

    private Vector3? NextSurfacePosition(Vector3 wandPosition, Vector3 wandDirection)
    {
        RaycastHit hitObject;
        Ray ray = new Ray(wandPosition, wandDirection);

        if (!Physics.Raycast(ray, out hitObject, Mathf.Infinity, surfaceLayer))
        {
            return null;
        }

        return wandPosition + wandDirection * hitObject.distance;
    }

    private static GameObject CreateLaser(Vector3 from, Vector3 to)
    {
        GameObject laser = new GameObject();
        laser.AddComponent<LineRenderer>();
        laser.GetComponent<LineRenderer>().numPositions = 2;
        laser.GetComponent<LineRenderer>().SetPosition(0, from);
        laser.GetComponent<LineRenderer>().SetPosition(1, to);
        laser.GetComponent<LineRenderer>().startWidth = 0.01f;
        laser.GetComponent<LineRenderer>().endWidth = 0.01f;
        laser.GetComponent<LineRenderer>().startColor = Color.red;
        laser.GetComponent<LineRenderer>().endColor = Color.red;
        laser.GetComponent<LineRenderer>().material = new Material(Shader.Find("Particles/Alpha Blended Premultiply"));

        laser.AddComponent<LaserRay>();
        laser.GetComponent<LaserRay>().from = from;
        laser.GetComponent<LaserRay>().to = to;

        return laser;
    }

    TWandAction[] Spell.Select()
    {
        Init();
        TWandAction[] actions = {
            new Union2<WandAction.Drain, WandAction.Vibrate>.Case2(new WandAction.Vibrate(500)),
        };
        return actions;
    }

    TWandAction[] Spell.Deselect()
    {
        Object.Destroy(rightHandTrapSource);
        Object.Destroy(leftHandTrapSource);
        rightHandTrapSource = null;
        leftHandTrapSource = null;

        TWandAction[] actions = { };
        return actions;
    }
}
