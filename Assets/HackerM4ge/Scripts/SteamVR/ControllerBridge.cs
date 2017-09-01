using UnityEngine;
using SteamVRHelper;

public class ControllerBridge
{
    private GameObject controllerObject;
    private SteamVR_Controller.Device controllerSteamDevice
    {
        get
        {
            var index = controllerObject.GetComponent<SteamVR_TrackedObject>().index;
            if (index == SteamVR_TrackedObject.EIndex.None)
            {
                return null;
            }
            return SteamVR_Controller.Input((int)index);
        }
    }

    private ControllerType controllerType;

    private const Valve.VR.EVRButtonId triggerButton = Valve.VR.EVRButtonId.k_EButton_SteamVR_Trigger;

    public ControllerBridge(GameObject controllerObject)
    {
        this.controllerObject = controllerObject;
        controllerType = Helper.GetControllerType(controllerSteamDevice);
    }

    public TriggerState GetTriggerState()
    {
        if (controllerSteamDevice == null)
        {
            return new TriggerState(false, false, false);
        }

        return new TriggerState(
            controllerSteamDevice.GetPressUp(triggerButton),
            controllerSteamDevice.GetPressDown(triggerButton),
            controllerSteamDevice.GetPress(triggerButton)
        );
    }

    public Vector2 GetTouchpadAxis()
    {
        return controllerSteamDevice.GetAxis();
    }

    public Vector3 GetPosition()
    {
        return controllerObject.transform.position;
    }

    public Vector3 GetDirection()
    {
        return Helper.WandDirection(controllerObject.transform, controllerType).normalized;
    }

    public Vector3 GetVelocity()
    {
        return controllerSteamDevice.velocity;
    }
}
