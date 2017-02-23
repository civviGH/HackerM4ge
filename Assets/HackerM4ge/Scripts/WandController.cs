using UnityEngine;
using System.Collections.Generic;

using TWandAction = Union2<WandAction.Drain, WandAction.Vibrate>;
using System;
using SteamVRHelper;

public class WandController : MonoBehaviour
{
    public GameObject thumbnailPanel;

    private GameObject teleportLine;
    private GameObject highlightedPlatform;

    // buttons
    private const Valve.VR.EVRButtonId triggerButton = Valve.VR.EVRButtonId.k_EButton_SteamVR_Trigger;
    private const Valve.VR.EVRButtonId gripButton = Valve.VR.EVRButtonId.k_EButton_Grip;
    private const Valve.VR.EVRButtonId menuButton = Valve.VR.EVRButtonId.k_EButton_ApplicationMenu;
    private const Valve.VR.EVRButtonId touchpad = Valve.VR.EVRButtonId.k_EButton_SteamVR_Touchpad;

    private const Valve.VR.EVRButtonId touchButtonA = Valve.VR.EVRButtonId.k_EButton_A;
    private const Valve.VR.EVRButtonId touchButtonB = Valve.VR.EVRButtonId.k_EButton_ApplicationMenu;

    private Direction lastFrameTouchpadDirection = Direction.noDirection;

    // controller initalize
    // TODO This should be a method, not a property...
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

        controllerType = Helper.GetControllerType(controller);
        //Debug.Log(string.Format("{0} Controller detected", Enum.GetName(typeof(ControllerType), controllerType)));
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

        Vector3 normalizedDirection = Helper.WandDirection(transform, controllerType);
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

        lastFrameTouchpadDirection = Helper.GetDirectionOfTouchpad(controller);
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
        int newSpellIndex = currentSpellIndex;
        if (SpellSelectPreviousDown())
        {
            newSpellIndex = GetSpellIndexPlus(-1);
        }
        if (SpellSelectNextDown())
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

    private bool SpellSelectNextDown()
    {
        Direction direction = Helper.GetDirectionOfTouchpad(controller);
        switch (controllerType)
        {
            case ControllerType.Oculus:
                return lastFrameTouchpadDirection != direction && direction == Direction.right;
            case ControllerType.Vive:
                return controller.GetPressDown(touchpad) && direction == Direction.right;
            default:
                throw new NotImplementedException();
        }
    }

    private bool SpellSelectPreviousDown()
    {
        Direction direction = Helper.GetDirectionOfTouchpad(controller);
        switch (controllerType)
        {
            case ControllerType.Oculus:
                return lastFrameTouchpadDirection != direction && direction == Direction.left;
            case ControllerType.Vive:
                return controller.GetPressDown(touchpad) && direction == Direction.left;
            default:
                throw new NotImplementedException();
        }
    }

}
