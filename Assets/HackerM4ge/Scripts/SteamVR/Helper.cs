using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

namespace SteamVRHelper
{
    public enum ControllerType
    {
        Vive,
        Oculus,
    };

    public enum Direction
    {
        noDirection = 0,
        right = 1,
        down = 2,
        left = 3,
        up = 4,
    };


    public class Helper
    {
        public static string GetTrackedDeviceManufacturerString(uint deviceId)
        {
            CVRSystem hmd = SteamVR.instance.hmd;

            var error = ETrackedPropertyError.TrackedProp_Success;
            var capacity = hmd.GetStringTrackedDeviceProperty(deviceId, ETrackedDeviceProperty.Prop_ManufacturerName_String, null, 0, ref error);
            if (capacity > 1)
            {
                var result = new System.Text.StringBuilder((int)capacity);
                hmd.GetStringTrackedDeviceProperty(deviceId, ETrackedDeviceProperty.Prop_ManufacturerName_String, result, capacity, ref error);
                return result.ToString();
            }
            return null;
        }


        public static ControllerType GetControllerType(SteamVR_Controller.Device controller)
        {
            // TODO Das hier ist nicht wirklich future-proof: Derselbe Hersteller könnta ja noch mehr Geräte herstellen. :)
            string manufacturer = GetTrackedDeviceManufacturerString(controller.index);

            switch (manufacturer)
            {
                case "Oculus":
                    return ControllerType.Oculus;
                // TODO Ich weiß nicht, was genau bei der Vive zurück gegeben wird.
                // Default sollte eher ne Exception sein.
                default:
                    return ControllerType.Vive;
            }
        }

        public static Vector3 WandDirection(Transform transform, ControllerType controllerType)
        {
            switch (controllerType)
            {
                case ControllerType.Oculus:
                    return transform.forward - transform.up;
                case ControllerType.Vive:
                    return transform.forward;
                default:
                    throw new NotImplementedException();
            }
        }

        public static Direction GetDirectionOfTouchpad(SteamVR_Controller.Device controller)
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
    }

}