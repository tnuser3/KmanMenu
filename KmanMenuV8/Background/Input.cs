using KmanMenu.Helpers.Notifacations;
using KmanMenu.Helpers;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using Valve.VR;

namespace KmanMenu.Background
{
    internal class Input : MonoBehaviour
    {
        public static bool RightSecondary;
        public static bool RightPrimary;
        public static bool RightTrigger;
        public static bool RightGrip;
        public static Vector2 RightJoystick;
        public static bool RightStickClick;

        public static bool LeftSecondary;
        public static bool LeftPrimary;
        public static bool LeftGrip;
        public static bool LeftTrigger;
        public static Vector2 LeftJoystick;
        public static bool LeftStickClick;

        private static bool CalculateGripState(float grabValue, float grabThreshold)
        {
            return grabValue >= grabThreshold;
        }

        public void Update()
        {
            if (ControllerInputPoller.instance != null)
            {
                var Poller = ControllerInputPoller.instance;
                RightSecondary = Poller.rightControllerPrimaryButton;
                RightPrimary = Poller.rightControllerSecondaryButton;
                RightTrigger = CalculateGripState(Poller.rightControllerIndexFloat, 0.5f);
                RightGrip = CalculateGripState(Poller.rightControllerGripFloat, 0.5f);
                RightJoystick = Poller.rightControllerPrimary2DAxis;
                RightStickClick = SteamVR_Actions.gorillaTag_RightJoystickClick.GetState(SteamVR_Input_Sources.RightHand);

                //------------------------------------------------------------------------

                LeftSecondary = Poller.leftControllerPrimaryButton;
                LeftPrimary = Poller.leftControllerSecondaryButton;
                LeftTrigger = CalculateGripState(Poller.leftControllerIndexFloat, 0.5f);
                LeftGrip = CalculateGripState(Poller.leftControllerGripFloat, 0.5f);
                LeftJoystick = SteamVR_Actions.gorillaTag_LeftJoystick2DAxis.GetAxis(SteamVR_Input_Sources.LeftHand);
                LeftStickClick = SteamVR_Actions.gorillaTag_LeftJoystickClick.GetState(SteamVR_Input_Sources.LeftHand);
            }
        }
    }
}
