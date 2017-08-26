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
    private GameObject rightHandPlacedTrapSource;
    private GameObject leftHandPlacedTrapSource;

    private Vector3 rightHandThrowingStartPosition;
    private Vector3 leftHandThrowingStartPosition;
    private float rightHandThrowingStartTime;
    private float leftHandThrowingStartTime;



    private enum ControllerAndTrapSourceState
    {
        Initial = 0,
        Placing = 1,
        Placed = 2,
        Throwing = 3,
        Thrown = 4
    };

    private ControllerAndTrapSourceState leftHandState = ControllerAndTrapSourceState.Initial;
    private ControllerAndTrapSourceState rightHandState = ControllerAndTrapSourceState.Initial;

    /*
     * TODO:
     * - Nach dem Setzen nicht direkt den Laser einschalten, sondern Traps erst noch "werfen"
     * - Trap-Positionen beim Setzen besser sichtbar machen: Strahl in den Himmel z.B.?
     * - Schadenssschema ändern: Gesamtschaden statt Lebensdauer beschränken, dafür mehr DPS machen (evtl. einfach unbegrenzt, zumindest aber genug um normale Gegner zu töten bevor sie durchlaufen können)
     * - Traps erst ab "Placing" sichtbar machen, oder vielleicht erst nur an der Hand sichtbar machen und ab Placing in der Ferne
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
        rightHandTrapSource = InstantiateTrapSource();
        leftHandTrapSource = InstantiateTrapSource();
        leftHandState = ControllerAndTrapSourceState.Initial;
        rightHandState = ControllerAndTrapSourceState.Initial;
    }

    GameObject InstantiateTrapSource()
    {
        GameObject trapSource = Object.Instantiate(laserSourcePrefab);
        Material[] materials = { laserSourceBlueprintMaterial, laserSourceBlueprintMaterial };
        trapSource.GetComponent<MeshRenderer>().materials = materials;

        return trapSource;
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
            ref rightHandTrapSource, ref rightHandPlacedTrapSource, ref rightHandState, ref rightHandThrowingStartPosition, ref rightHandThrowingStartTime
        );
        UpdateControllerStateAndTrap(leftTriggerState, leftTouchpadAxis, leftControllerPosition, leftControllerDirection,
            ref leftHandTrapSource, ref leftHandPlacedTrapSource, ref leftHandState, ref leftHandThrowingStartPosition, ref leftHandThrowingStartTime
        );

        // when both are placed, finish and reset
        if (rightHandState == ControllerAndTrapSourceState.Thrown && leftHandState == ControllerAndTrapSourceState.Thrown)
        {
            MakeTrapSolid(rightHandPlacedTrapSource);
            MakeTrapSolid(leftHandPlacedTrapSource);
            GameObject laser = CreateLaser(rightHandPlacedTrapSource.transform.position, leftHandPlacedTrapSource.transform.position);
            Object.Destroy(rightHandTrapSource);
            Object.Destroy(leftHandTrapSource);
            Object.Destroy(rightHandPlacedTrapSource, lifetime);
            Object.Destroy(leftHandPlacedTrapSource, lifetime);
            Object.Destroy(laser, lifetime);

            Init();
        }

        TWandAction[] actions = { };
        return actions;
    }

    private void UpdateControllerStateAndTrap(TriggerState triggerState, Vector2 touchpadAxis, Vector3? controllerPosition, Vector3? controllerDirection,
        ref GameObject trapSource, ref GameObject placedTrapSource, ref ControllerAndTrapSourceState state, ref Vector3 throwingStartPosition, ref float throwingStartTime)
    {
        if (controllerPosition == null || controllerDirection == null)
        {
            // Controller isn't active
            return;
        }

        switch (state)
        {
            case ControllerAndTrapSourceState.Initial:
                UpdateTrapSourcePosition(ref trapSource, controllerPosition.Value, controllerDirection.Value, false);
                if (triggerState.down)
                {
                    state = ControllerAndTrapSourceState.Placing;
                }
                break;
            case ControllerAndTrapSourceState.Placing:
                UpdateTrapSourcePosition(ref trapSource, controllerPosition.Value, controllerDirection.Value, true);
                if (triggerState.up)
                {
                    if (NextSurfacePosition(controllerPosition.Value, controllerDirection.Value) != null)
                    {
                        state = ControllerAndTrapSourceState.Placed;
                        placedTrapSource = trapSource;
                        trapSource = InstantiateTrapSource();
                    }
                    else
                    {
                        state = ControllerAndTrapSourceState.Initial;
                    }
                }
                break;
            case ControllerAndTrapSourceState.Placed:
                if (triggerState.down)
                {
                    throwingStartPosition = controllerPosition.Value;
                    throwingStartTime = Time.time;
                    state = ControllerAndTrapSourceState.Throwing;
                }
                break;
            case ControllerAndTrapSourceState.Throwing:
                UpdateTrapSourcePosition(ref trapSource, controllerPosition.Value, controllerDirection.Value, false);
                if (triggerState.up)
                {
                    Vector3 direction = controllerPosition.Value - throwingStartPosition;
                    float speed = direction.magnitude / (throwingStartTime - Time.time);

                    state = ControllerAndTrapSourceState.Thrown;
                    MakeTrapSolid(trapSource);
                }
                break;
            case ControllerAndTrapSourceState.Thrown:
                break;
        }
    }

    private void MakeTrapSolid(GameObject trapSource)
    {
        Material[] materials = { laserSourceMaterial, laserSourceMaterial };
        trapSource.GetComponent<MeshRenderer>().materials = materials;
    }

    private void UpdateTrapSourcePosition(ref GameObject trapSource, Vector3 wandPosition, Vector3 wandDirection, bool placeOnSurface)
    {
        Vector3? surfacePosition = NextSurfacePosition(wandPosition, wandDirection);
        if (placeOnSurface && surfacePosition != null)
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
