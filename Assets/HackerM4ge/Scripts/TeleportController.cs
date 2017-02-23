using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using SteamVRHelper;

public class TeleportController : MonoBehaviour
{
    private SteamVR_TrackedObject trackedObj;
    private ControllerType controllerType;

    void Start()
    {
        trackedObj = GetComponent<SteamVR_TrackedObject>();

        controllerType = Helper.GetControllerType(Controller());
    }

    void Update()
    {

    }

    private SteamVR_Controller.Device Controller()
    {
        return SteamVR_Controller.Input((int)trackedObj.index);
    }
}
