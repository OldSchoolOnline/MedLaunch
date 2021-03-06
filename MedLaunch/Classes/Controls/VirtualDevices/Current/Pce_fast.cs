﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MedLaunch.Classes.Controls.VirtualDevices
{
    public class Pce_fast : VirtualDeviceBase
    {
        public static DeviceDefinition GamePad(int VirtualPort)
        {
            DeviceDefinition device = new DeviceDefinition();
            device.DeviceName = "PCE (fast) GamePad";
            device.ControllerName = "gamepad";
            device.CommandStart = "pce_fast.input.port" + VirtualPort;
            device.VirtualPort = VirtualPort;
            device.MapList = new List<Mapping>
            {
                /* MapList is now autogenerated from mednafen.cfg */
            };

            device.CustomOptions = new List<NonControlMapping>
            {
                new NonControlMapping { Description = "Default position for 'MODE' switch",
                    MednafenCommand = device.CommandStart + ".gamepad.mode_select.defpos",
                    MinValue = 2, MaxValue = 6,
                    Values = new List<string>
                    {
                        "2", "6"
                    },
                    ContType = ContrType.COMBO,
                 ConvType = ConvertionType.STRING},
            };

            DeviceDefinition.ParseOptionsFromConfig(device);
            DeviceDefinition.PopulateConfig(device);
            return device;
        }

        public static DeviceDefinition Mouse(int VirtualPort)
        {
            DeviceDefinition device = new DeviceDefinition();
            device.DeviceName = "PCE (fast) Mouse";
            device.ControllerName = "mouse";
            device.CommandStart = "pce_fast.input.port" + VirtualPort;
            device.VirtualPort = VirtualPort;
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
