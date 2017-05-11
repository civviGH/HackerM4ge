using UnityEngine;
using System.Collections.Generic;

using TWandAction = Union2<WandAction.Drain, WandAction.Vibrate>;
using System;
using SteamVRHelper;

public class WandController : MonoBehaviour
{
    public GameObject thumbnailPanel;
    public GameObject leftControllerObject;

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
    private SteamVR_Controller.Device rightController { get { return SteamVR_Controller.Input((int)trackedObj.index); } }
    private SteamVR_Controller.Device leftController { 
        get {
            var index = leftControllerObject.GetComponent<SteamVR_TrackedObject> ().index;
            if (index == SteamVR_TrackedObject.EIndex.None) {
                return null;
            }
            return SteamVR_Controller.Input((int)index);
        }
    }


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

        controllerType = Helper.GetControllerType(rightController);
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
                    rightController.TriggerHapticPulse(vibrate.microseconds);
                    return Unit.Instance;
                }
            );
        }
    }

    // Update is called once per frame
    void Update()
    {
        SpellSelect();

        TWandAction[] wandActions = GetWandActions();
        ExecuteWandActions(wandActions);

        lastFrameTouchpadDirection = Helper.GetDirectionOfTouchpad(rightController);
    }
        
    private Spell SelectedSpell()
    {
        return listOfSpells[currentSpellIndex];
    }

    private void SelectSpellByIndex(int newSpellIndex)
    {
        if(newSpellIndex == currentSpellIndex)
        {
            return;
        }
        TWandAction[] wandActions;
        wandActions = SelectedSpell().Deselect();
        ExecuteWandActions(wandActions);
        currentSpellIndex = newSpellIndex;
        wandActions = SelectedSpell().Select();
        ExecuteWandActions(wandActions);
        UpdateThumbnail();
    }

    public void SelectSpell(Spell spell)
    {
        int newSpellIndex = listOfSpells.FindIndex(
            spell_ => spell_.GetType() == spell.GetType()
        );

        if(newSpellIndex < 0)
        {
            Debug.LogError("Couldn't find spell " + spell.GetType());
            return;
        }

        SelectSpellByIndex(newSpellIndex);
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
        SelectSpellByIndex(newSpellIndex);
    }

    void UpdateThumbnail()
    {
        thumbnailPanel.GetComponent<Renderer>().material = SelectedSpell().GetThumbnail();
    }

    private bool SpellSelectNextDown()
    {
        Direction direction = Helper.GetDirectionOfTouchpad(rightController);
        switch (controllerType)
        {
            case ControllerType.Oculus:
                return lastFrameTouchpadDirection != direction && direction == Direction.right;
            case ControllerType.Vive:
                return rightController.GetPressDown(touchpad) && direction == Direction.right;
            default:
                throw new NotImplementedException();
        }
    }

    private bool SpellSelectPreviousDown()
    {
        Direction direction = Helper.GetDirectionOfTouchpad(rightController);
        switch (controllerType)
        {
            case ControllerType.Oculus:
                return lastFrameTouchpadDirection != direction && direction == Direction.left;
            case ControllerType.Vive:
                return rightController.GetPressDown(touchpad) && direction == Direction.left;
            default:
                throw new NotImplementedException();
        }
    }

    private TWandAction[] GetWandActions(){

        Vector3 normalizedDirection = Helper.WandDirection(transform, controllerType);
        normalizedDirection.Normalize();

        if (leftController == null) {
            return SelectedSpell().UpdateSpell(
                new TriggerState(
                    rightController.GetPressUp(triggerButton),
                    rightController.GetPressDown(triggerButton),
                    rightController.GetPress(triggerButton)
                ),
                rightController.GetAxis(),
                transform.position,
                normalizedDirection,
                new TriggerState(
                    false,
                    false,
                    false
                ),
                Vector2.zero,
                null,
                null
            );

        } else {
            return SelectedSpell().UpdateSpell(
                new TriggerState(
                    rightController.GetPressUp(triggerButton),
                    rightController.GetPressDown(triggerButton),
                    rightController.GetPress(triggerButton)
                ),
                rightController.GetAxis(),
                transform.position,
                normalizedDirection,
                new TriggerState(
                    leftController.GetPressUp(triggerButton),
                    leftController.GetPressDown(triggerButton),
                    leftController.GetPress(triggerButton)
                ),
                leftController.GetAxis(),
                leftControllerObject.transform.position,
                Helper.WandDirection(leftControllerObject.transform, controllerType).normalized
            );
        }

    }

}
