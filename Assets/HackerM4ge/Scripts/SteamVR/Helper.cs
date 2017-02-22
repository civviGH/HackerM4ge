using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

namespace SteamVRHelper
{

    public class Helper
    {
        private CVRSystem hmd;

        public Helper()
        {
            hmd = SteamVR.instance.hmd;
        }

        public string GetTrackedDeviceManufacturerString(uint deviceId)
        {
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
    }

}