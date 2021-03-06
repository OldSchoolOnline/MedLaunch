﻿using MedLaunch.Classes.Controls.VirtualDevices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using MahApps.Metro.SimpleChildWindow;
using MedLaunch.Classes.Controls;

namespace MedLaunch
{
    /// <summary>
    /// Interaction logic for TestUC.xaml
    /// </summary>
    public partial class SsCtrl : UserControl
    {
        public MainWindow mw { get; set; }

        public SsCtrl()
        {
            InitializeComponent();
            mw = Application.Current.Windows.OfType<MainWindow>().FirstOrDefault();
        }

        private async void btnControlsConfigure_Click(object sender, RoutedEventArgs e)
        {
            // get button name
            Button button = (Button)sender;
            string name = button.Name;

            // remove beginning and end
            name = name.Replace("btn", "").Replace("Configure", "");
            
            // get the relevant combox
            ComboBox cb = (ComboBox)this.FindName("cmb" + name);

            // get the virtual port number
            string selectedString = cb.SelectionBoxItem.ToString();
            int portNum = Convert.ToInt32(selectedString.Replace("Virtual Port ", ""));

            // get mednafen config version
            bool isNewConfig = Classes.VersionChecker.Instance.IsNewConfig;

            IDeviceDefinition dev;

            if (isNewConfig)
            {
                dev = new DeviceDefinition();

                switch (name)
                {
                    case "SsGamepad":
                        dev = Ss.GamePad(portNum);
                        break;
                    case "Ss3DGamepad":
                        dev = Ss.ThreeD(portNum);
                        break;
                    case "SsMission":
                        dev = Ss.Mission(portNum);
                        break;
                    case "SsDualMission":
                        dev = Ss.DMission(portNum);
                        break;
                    case "SsWheel":
                        dev = Ss.Wheel(portNum);
                        break;
                    case "SsGun":
                        dev = Ss.Gun(portNum);
                        break;
                    case "SsMouse":
                        dev = Ss.Mouse(portNum);
                        break;
                    case "SsKeyboardUS":
                        dev = Ss.KeyboardUS(portNum);
                        break;
                    case "SsKeyboardJP":
                        dev = Ss.KeyboardJP(portNum);
                        break;
                    default:
                        return;
                }
            }
            else
            {
                dev = new DeviceDefinitionLegacy();

                switch (name)
                {
                    case "SsGamepad":
                        dev = Ss_Legacy.GamePad(portNum);
                        break;
                    case "Ss3DGamepad":
                        dev = Ss_Legacy.ThreeD(portNum);
                        break;
                    case "SsMission":
                        dev = Ss_Legacy.Mission(portNum);
                        break;
                    case "SsDualMission":
                        dev = Ss_Legacy.DMission(portNum);
                        break;
                    case "SsWheel":
                        dev = Ss_Legacy.Wheel(portNum);
                        break;
                    case "SsGun":
                        dev = Ss_Legacy.Gun(portNum);
                        break;
                    case "SsMouse":
                        dev = Ss_Legacy.Mouse(portNum);
                        break;
                    default:
                        Classes.MessagePopper.PopControllerTargetingIssue();
                        return;
                }
            }            
            
            mw.ControllerDefinition = dev;

            // launch controller configuration window
            if (isNewConfig)
            {
                Grid RootGrid = (Grid)mw.FindName("RootGrid");
                await mw.ShowChildWindowAsync(new ConfigureController()
                {
                    IsModal = true,
                    AllowMove = false,
                    Title = "Controller Configuration",
                    CloseOnOverlay = false,
                    ShowCloseButton = false,
                    CloseByEscape = false
                }, RootGrid);
            }
            else
            {
                Grid RootGrid = (Grid)mw.FindName("RootGrid");
                await mw.ShowChildWindowAsync(new ConfigureControllerLegacy()
                {
                    IsModal = true,
                    AllowMove = false,
                    Title = "Controller Configuration",
                    CloseOnOverlay = false,
                    ShowCloseButton = false,
                    CloseByEscape = false
                }, RootGrid);
            }
        }
    }
}
