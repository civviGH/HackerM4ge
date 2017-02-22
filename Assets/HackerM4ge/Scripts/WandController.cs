using UnityEngine;
using System.Collections.Generic;

using TWandAction = Union2<WandAction.Drain, WandAction.Vibrate>;
using System;

public class WandController : MonoBehaviour
{
    enum Direction
    {
        noDirection = 0,
        right = 1,
        down = 2,
        left = 3,
        up = 4,
    };

    enum ControllerType
    {
        Vive,
        Oculus,
    };

    public AudioSource teleportSound;

    public GameObject thumbnailPanel;

    private GameObject teleportLine;
    private GameObject highlightedPlatform;

    // buttons
    private Valve.VR.EVRButtonId triggerButton = Valve.VR.EVRButtonId.k_EButton_SteamVR_Trigger;
    private Valve.VR.EVRButtonId gripButton = Valve.VR.EVRButtonId.k_EButton_Grip;
    private Valve.VR.EVRButtonId menuButton = Valve.VR.EVRButtonId.k_EButton_ApplicationMenu;
    private Valve.VR.EVRButtonId touchpad = Valve.VR.EVRButtonId.k_EButton_SteamVR_Touchpad;

    // controller initalize
    private SteamVR_Controller.Device controller { get { return SteamVR_Controller.Input((int)trackedObj.index); } }

    private ControllerType controllerType;

    private SteamVR_TrackedObject trackedObj;

    // List of spells
    List<Spell> listOfSpells = new List<Spell>();
    private int currentSpellIndex = 0;

    // Use this for initialization
    void Start()
    {
        trackedObj = GetComponent<SteamVR_TrackedObject>();

        // Add spells to spellList
        listOfSpells.Add(new MeteorSpell());
        listOfSpells.Add(new LasertrapSpell());
        listOfSpells.Add(new ChaosSpell());

        TWandAction[] wandActions = SelectedSpell().Select();

        ExecuteWandActions(wandActions);

        // Put material of spell on thumbnailpanel
        UpdateThumbnail();

        DetectControllerType();
        //Debug.Log(string.Format("{0} Controller detected", Enum.GetName(typeof(ControllerType), controllerType)));
    }

    private void DetectControllerType()
    {
        string manufacturer = (new SteamVRHelper.Helper()).GetTrackedDeviceManufacturerString(controller.index);

        switch(manufacturer) {
            case "Oculus":
                controllerType = ControllerType.Oculus;
                break;
            default:
                controllerType = ControllerType.Vive;
                break;
        }
    }

    private void ExecuteWandActions(TWandAction[] wandActions)
    {
        foreach (TWandAction wandAction in wandActions)
        {
            wandAction.Match(
                drain =>
                {
                    throw new NotImplementedException();
                },
                vibrate =>
                {
                    controller.TriggerHapticPulse(vibrate.microseconds);
                    return Unit.Instance;
                }
            );
        }
    }

    // Update is called once per frame
    void Update()
    {
        SpellSelect();
        ShowTeleportDirection();
        Teleport();

        Vector3 normalizedDirection = WandDirection();
        normalizedDirection.Normalize();
        TWandAction[] wandActions = SelectedSpell().UpdateSpell(
          new TriggerState(
            controller.GetPressUp(triggerButton),
            controller.GetPressDown(triggerButton),
            controller.GetPress(triggerButton)
          ),
          controller.GetAxis(),
          transform.position,
          normalizedDirection
        );
        ExecuteWandActions(wandActions);
    }

    private Spell SelectedSpell()
    {
        return listOfSpells[currentSpellIndex];
    }

    private void SelectSpellByIndex(int newSpellIndex)
    {
        currentSpellIndex = newSpellIndex;
    }

    private int GetSpellIndexPlus(int delta)
    {
        return ((currentSpellIndex + delta) % listOfSpells.Count + listOfSpells.Count) % listOfSpells.Count;
    }

    void SpellSelect()
    {
        Direction direction = GetDirectionOfTouchpad();
        int newSpellIndex = currentSpellIndex;
        if (SpellSelectPreviousDown(direction))
        {
            newSpellIndex = GetSpellIndexPlus(-1);
        }
        if (SpellSelectNextDown(direction))
        {
            newSpellIndex = GetSpellIndexPlus(1);
        }
        if (newSpellIndex != currentSpellIndex)
        {
            TWandAction[] wandActions;
            wandActions = SelectedSpell().Deselect();
            ExecuteWandActions(wandActions);
            SelectSpellByIndex(newSpellIndex);
            wandActions = SelectedSpell().Select();
            ExecuteWandActions(wandActions);
            UpdateThumbnail();
        }
    }

    void UpdateThumbnail()
    {
        thumbnailPanel.GetComponent<Renderer>().material = SelectedSpell().GetThumbnail();
    }

    void Teleport()
    {
        if (TeleportButtonUp())
        {
            if (teleportLine != null)
            {
                GameObject nextPlatform = GetNextPlatform();
                if (nextPlatform != null)
                {
                    CameraComponent cameraComponent = transform.parent.gameObject.GetComponent<CameraComponent>();
                    transform.parent.gameObject.transform.position += nextPlatform.transform.position - cameraComponent.currentPlatformTransform.position;
                    cameraComponent.currentPlatformTransform = nextPlatform.transform;
                    teleportSound.Play();
                }
            }
            unhighlightPlatform();
            Destroy(teleportLine);
            teleportLine = null;
        }
    }

    GameObject GetNextPlatform()
    {
        RaycastHit hitObject;
        Ray ray = new Ray(transform.position, WandDirection());
        if (Physics.Raycast(ray, out hitObject))
        {
            if (hitObject.collider.transform.parent != null)
            {
                GameObject nextPlatform = hitObject.collider.transform.parent.gameObject;
                if (hitObject.collider.isTrigger && nextPlatform.GetComponent<PlatformComponent>() != null)
                {
                    return nextPlatform;
                }
            }
        }
        return null;
    }

    void ShowTeleportDirection()
    {
        if (TeleportButtonDown())
        {
            teleportLine = new GameObject();
            teleportLine.transform.position = transform.position;
            teleportLine.transform.SetParent(transform);
            teleportLine.AddComponent<LineRenderer>();
            LineRenderer lineRenderer = teleportLine.GetComponent<LineRenderer>();
            lineRenderer.material = new Material(Shader.Find("Particles/Alpha Blended Premultiply"));
            lineRenderer.startColor = Color.black;
            lineRenderer.endColor = Color.blue;
            lineRenderer.startWidth = 0.01f;
            lineRenderer.endWidth = 0.01f;
            Vector3 wandDirection = WandDirection();

            lineRenderer.SetPosition(0, transform.position);
            lineRenderer.SetPosition(1, transform.position + 20 * wandDirection);
        }

        if (TeleportButtonPressed() && teleportLine != null)
        {
            Vector3 wandDirection = WandDirection();

            LineRenderer lineRenderer = teleportLine.GetComponent<LineRenderer>();
            lineRenderer.SetPosition(0, transform.position);
            lineRenderer.SetPosition(1, transform.position + 20 * wandDirection);

            GameObject nextPlatform = GetNextPlatform();
            if (nextPlatform != null)
            {
                highlightPlatform(nextPlatform);
            }
            else
            {
                unhighlightPlatform();
            }
        }
    }

    void highlightPlatform(GameObject nextPlatform)
    {
        unhighlightPlatform();
        nextPlatform.GetComponent<Renderer>().material.SetColor("_EmissionColor", Color.green * 0.5f);
        highlightedPlatform = nextPlatform;
    }

    void unhighlightPlatform()
    {
        if (highlightedPlatform != null)
        {
            highlightedPlatform.GetComponent<Renderer>().material.SetColor("_EmissionColor", Color.black);
            highlightedPlatform = null;
        }
    }

    // Returns direction of the touchpad when pressed, Tobi(s füße) stinkt.
    private Direction GetDirectionOfTouchpad()
    {
        Vector2 axes = controller.GetAxis();
        if (axes[0] < -0.7)
            return Direction.left;
        if (axes[0] > 0.7)
            return Direction.right;
        if (axes[1] < -0.7)
            return Direction.down;
        if (axes[1] > 0.7)
            return Direction.up;
        return Direction.noDirection;
    }

    private Vector3 WandDirection()
    {
        switch(controllerType)
        {
            case ControllerType.Oculus:
                return transform.forward - transform.up;
                break;
            case ControllerType.Vive:
                return transform.forward;
                break;
            default:
                throw new NotImplementedException();
        }
    }

    bool TeleportButtonPressed()
    {
        return controller.GetPress(touchpad);
    }

    private bool TeleportButtonUp()
    {
        return controller.GetPressUp(touchpad);
    }

    bool TeleportButtonDown()
    {
        Direction direction = GetDirectionOfTouchpad();
        return controller.GetPressDown(touchpad) && direction == Direction.up;
    }

    private bool SpellSelectNextDown(Direction direction)
    {
        return controller.GetPressDown(touchpad) && direction == Direction.right;
    }

    private bool SpellSelectPreviousDown(Direction direction)
    {
        return controller.GetPressDown(touchpad) && direction == Direction.left;
    }

}
