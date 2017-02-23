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
    }

}