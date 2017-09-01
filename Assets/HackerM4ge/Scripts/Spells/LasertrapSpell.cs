using UnityEngine;

using TWandAction = Union2<WandAction.Drain, WandAction.Vibrate>;

class LasertrapSpell : Spell
{
    const float minimalDistance = 0.2f;
    const float maxSpeed = 15f;
    const float lifetime = 20f;
    const float maxThrowDuration = 5f;

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

    private Vector3 rightHandThrowVelocity;
    private Vector3 leftHandThrowVelocity;
    private Vector3 rightHandThrowPosition;
    private Vector3 leftHandThrowPosition;
    private float rightHandThrowTime;
    private float leftHandThrowTime;



    private enum ControllerAndTrapSourceState
    {
        Initial = 0,
        Placing = 1,
        Placed = 2,
        Throwing = 3,
        Thrown = 4,
        Arrived = 5
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

    TWandAction[] Spell.UpdateSpell(ControllerBridge rightController, ControllerBridge leftController)
    {
        TriggerState rightTriggerState = rightController.GetTriggerState();
        Vector2 rightTouchpadAxis = rightController.GetTouchpadAxis();
        Vector3 rightControllerPosition = rightController.GetPosition();
        Vector3 rightControllerDirection = rightController.GetDirection();
        Vector3 rightControllerVelocity = rightController.GetVelocity();

        TriggerState leftTriggerState = leftController != null ? leftController.GetTriggerState() : null;
        Vector2 leftTouchpadAxis = leftController != null ? leftController.GetTouchpadAxis() : Vector2.zero;
        Vector3? leftControllerPosition = leftController != null ? leftController.GetPosition() : null as Vector3?;
        Vector3? leftControllerDirection = leftController != null ? leftController.GetDirection() : null as Vector3?;
        Vector3? leftControllerVelocity = leftController != null ? leftController.GetVelocity() : null as Vector3?;

        UpdateControllerStateAndTrap(rightTriggerState, rightTouchpadAxis, rightControllerPosition, rightControllerDirection,
            ref rightHandTrapSource, ref rightHandPlacedTrapSource, ref rightHandState, ref rightHandThrowingStartPosition, ref rightHandThrowingStartTime,
            ref rightHandThrowVelocity, ref rightHandThrowPosition, ref rightHandThrowTime
        );
        UpdateControllerStateAndTrap(leftTriggerState, leftTouchpadAxis, leftControllerPosition, leftControllerDirection,
            ref leftHandTrapSource, ref leftHandPlacedTrapSource, ref leftHandState, ref leftHandThrowingStartPosition, ref leftHandThrowingStartTime,
            ref leftHandThrowVelocity, ref leftHandThrowPosition, ref leftHandThrowTime
        );

        // when both are placed, finish and reset
        if (rightHandState == ControllerAndTrapSourceState.Arrived && leftHandState == ControllerAndTrapSourceState.Arrived)
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
        ref GameObject trapSource, ref GameObject placedTrapSource, ref ControllerAndTrapSourceState state, ref Vector3 throwingStartPosition, ref float throwingStartTime,
        ref Vector3 throwVelocity, ref Vector3 throwPosition, ref float throwTime)
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
                        trapSource = null;
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
                    trapSource = InstantiateTrapSource();
                    MakeTrapSolid(trapSource);
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
                    float speed = direction.magnitude / (Time.time - throwingStartTime);

                    throwVelocity = direction / (Time.time - throwingStartTime) * 10;
                    throwPosition = controllerPosition.Value;
                    throwTime = Time.time;

                    state = ControllerAndTrapSourceState.Thrown;
                    trapSource.AddComponent<Rigidbody>();
                    trapSource.GetComponent<Rigidbody>().velocity = throwVelocity;
                    trapSource.GetComponent<Rigidbody>().mass = 1;
                }
                break;
            case ControllerAndTrapSourceState.Thrown:
                if (maxThrowDuration < Time.time - throwTime)
                {
                    state = ControllerAndTrapSourceState.Placed;
                    Object.Destroy(trapSource);
                }
                float currentDist = (placedTrapSource.transform.position - trapSource.transform.position).magnitude;
                float origDist = (placedTrapSource.transform.position - throwPosition).magnitude;

                float velocityWeight = currentDist / origDist;
                if (currentDist < 1)
                {
                    state = ControllerAndTrapSourceState.Arrived;
                    Object.Destroy(trapSource);
                }

                if (velocityWeight > 2)
                {
                    state = ControllerAndTrapSourceState.Placed;
                    Object.Destroy(trapSource);
                }
                if (velocityWeight > 1)
                {
                    velocityWeight = 1;
                }
                UpdateFlyingTrapSourceVelocity(ref trapSource, placedTrapSource, throwVelocity, velocityWeight);

                break;
        }
    }

    private static void UpdateFlyingTrapSourceVelocity(
        ref GameObject trapSource, GameObject placedTrapSource, Vector3 throwVelocity, float velocityWeight
    )
    {
        Vector3 targetVelocity = (placedTrapSource.transform.position - trapSource.transform.position).normalized * 10;

        trapSource.GetComponent<Rigidbody>().velocity
                = velocityWeight * throwVelocity
                + (1 - velocityWeight) * targetVelocity;
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
