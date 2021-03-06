﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MedLaunch.Classes.Controls.VirtualDevices
{
    public class Ngp : VirtualDeviceBase
    {
        public static DeviceDefinition GamePad(int VirtualPort)
        {
            DeviceDefinition device = new DeviceDefinition();
            device.DeviceName = "NGP GamePad";
            device.CommandStart = "ngp.input.builtin";
            device.ControllerName = "gamepad";
            device.VirtualPort = 0;
            device.MapList = new List<Mapping>
            {
                /* MapList is now autogenerated from mednafen.cfg */
            };
            DeviceDefinition.ParseOptionsFromConfig(device);
            DeviceDefinition.PopulateConfig(device);
            return device;
        }
    }
}
