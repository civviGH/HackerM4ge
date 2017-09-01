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

    private Spell selectedSpell;

    // Use this for initialization
    void Start()
    {
        trackedObj = GetComponent<SteamVR_TrackedObject>();

        controllerType = Helper.GetControllerType(rightController);
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
        TWandAction[] wandActions = GetWandActions();
        ExecuteWandActions(wandActions);

        lastFrameTouchpadDirection = Helper.GetDirectionOfTouchpad(rightController);
    }
        
    private Spell SelectedSpell()
    {
        return selectedSpell;
    }

    public void SelectSpell(Spell spell)
    {
        TWandAction[] wandActions;

        if (SelectedSpell() != null)
        {
            if (SelectedSpell().GetType() == spell.GetType())
            {
                return;
            }

            wandActions = SelectedSpell().Deselect();
            ExecuteWandActions(wandActions);
        }

        selectedSpell = spell;

        wandActions = SelectedSpell().Select();
        ExecuteWandActions(wandActions);
        UpdateThumbnail();
    }

    void UpdateThumbnail()
    {
        thumbnailPanel.GetComponent<Renderer>().material = SelectedSpell().GetThumbnail();
    }

    private TWandAction[] GetWandActions() {
        if (SelectedSpell() == null)
        {
            return new TWandAction[0];
        }
        Vector3 normalizedDirection = Helper.WandDirection(transform, controllerType);
        normalizedDirection.Normalize();

        Vector2 leftControllerAxis = Vector2.zero;
        Vector3? leftControllerPosition = null;
        Vector3? leftControllerDirection = null;

        if (leftController != null) {
            leftControllerAxis = leftController.GetAxis();
            leftControllerPosition = leftControllerObject.transform.position;
            leftControllerDirection = Helper.WandDirection(leftControllerObject.transform, controllerType).normalized;
        }

        ControllerBridge rightControllerBridge = new ControllerBridge(this.gameObject);
        ControllerBridge leftControllerBridge = leftController != null ? new ControllerBridge(leftControllerObject) : null;

        return SelectedSpell().UpdateSpell(
            rightControllerBridge,
            leftControllerBridge
        );
    }

}
