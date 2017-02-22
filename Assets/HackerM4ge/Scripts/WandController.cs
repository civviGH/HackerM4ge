using UnityEngine;
using System.Collections.Generic;

using TWandAction = Union2<WandAction.Drain, WandAction.Vibrate>;
using System;

public class WandController : MonoBehaviour
{
    const int up = 4;
    const int right = 1;
    const int left = 3;
    const int down = 2;
    const int noDirection = 0;

    public Transform tipOfWand;

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
    private string controllerManufacturer;

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

        controllerManufacturer = (new SteamVRHelper.Helper()).GetTrackedDeviceManufacturerString(controller.index);
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

        Vector3 normalizedDirection = tipOfWand.position - transform.position;
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
        if (controller.GetPressDown(touchpad))
        {
            int direction = GetDirectionOfTouchpad();
            int newSpellIndex = currentSpellIndex;
            if (direction == left)
            {
                newSpellIndex = GetSpellIndexPlus(-1);
            }
            if (direction == right)
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
    }

    void UpdateThumbnail()
    {
        thumbnailPanel.GetComponent<Renderer>().material = SelectedSpell().GetThumbnail();
    }

    void Teleport()
    {
        if (controller.GetPressUp(touchpad))
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
        Ray ray = new Ray(transform.position, tipOfWand.position - transform.position);
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
        int direction = GetDirectionOfTouchpad();
        if (controller.GetPressDown(touchpad) && direction == up)
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
            Vector3 wandDirection = tipOfWand.position - transform.position;

            lineRenderer.SetPosition(0, transform.position);
            lineRenderer.SetPosition(1, transform.position + 20 * wandDirection);
        }

        if (controller.GetPress(touchpad) && teleportLine != null)
        {
            Vector3 wandDirection = tipOfWand.position - transform.position;

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
    private int GetDirectionOfTouchpad()
    {
        Vector2 axes = controller.GetAxis();
        if (axes[0] < -0.7)
            return left;
        if (axes[0] > 0.7)
            return right;
        if (axes[1] < -0.7)
            return down;
        if (axes[1] > 0.7)
            return up;
        return noDirection;
    }

}
