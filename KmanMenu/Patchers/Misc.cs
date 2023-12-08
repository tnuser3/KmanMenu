using GorillaLocomotion;
using GorillaNetworking;
using HarmonyLib;
using KmanMenu.Helpers;
using Photon.Pun;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace KmanMenu.Patchers.Misc
{
    public class Patch : MonoBehaviour
    {
        void Awake()
        {
            var harm = new Harmony("KmanMenu.Patchers.Misc");
            harm.PatchAll();
        }
    }

    [HarmonyPatch(typeof(GorillaGameManager), "LaunchSlingshotProjectile", MethodType.Normal)]
    public class anticrash
    {
        static bool Prefix(Vector3 slingshotLaunchLocation, Vector3 slingshotLaunchVelocity, int projHash, int trailHash, bool forLeftHand, int projectileCount, bool shouldOverrideColor, float colorR, float colorG, float colorB, float colorA, PhotonMessageInfo info)
        {
            if (info.Sender != PhotonNetwork.LocalPlayer)
            {
                if (Vector3.Distance(slingshotLaunchLocation, GorillaLocomotion.Player.Instance.transform.position) > 10)
                {
                    return false;
                }
                if (ObjectPools.instance.GetPoolByHash(projHash).objectToPool.GetComponent<SlingshotProjectileTrail>() != null)
                {
                    return false;
                }
            }
            if (info.Sender == PhotonNetwork.LocalPlayer)
            {
                if (Plugin.buttonsActive[31])
                {
                    return false;
                }
            }
            return true;
        }
    }

    [HarmonyPatch(typeof(GameObject))]
    [HarmonyPatch("CreatePrimitive", MethodType.Normal)]
    internal class GameObjectPatch
    {
        private static void Postfix(GameObject __result)
        {
            __result.GetComponent<Renderer>().material.shader = Shader.Find("GorillaTag/UberShader");
            __result.GetComponent<Renderer>().material.color = Color.black;
        }
    }

    [HarmonyPatch(typeof(Player))]
    [HarmonyPatch("AntiTeleportTechnology", MethodType.Normal)]
    internal class AntiTeleportTechnologyPatch
    {
        private static bool Prefix()
        {
            Player.Instance.teleportThresholdNoVel = int.MaxValue;
            return false;
        }
    }

    [HarmonyPatch(typeof(GorillaNetworkPublicTestsJoin))]
    [HarmonyPatch("GracePeriod", MethodType.Enumerator)]
    class NoGracePeriod
    {
        public static bool Prefix()
        {
            return false;
        }
    }

    [HarmonyPatch(typeof(GorillaNetworkPublicTestJoin2))]
    [HarmonyPatch("LateUpdate", MethodType.Normal)]
    class NoGracePeriod2
    {
        public static bool Prefix()
        {
            return false;
        }
    }
    [HarmonyPatch(typeof(PhotonNetworkController), "ProcessConnectedAndWaitingState", MethodType.Normal)]
    public class DontReconnect
    {
        public static bool status = true;
        public static bool Prefix()
        {
            return status;
        }
    }

}
