using HarmonyLib;
using PlayFab;
using PlayFab.Internal;
using UnityEngine;

namespace KmanMenu.Patchers.Playfab
{
    public class Patch : MonoBehaviour
    {
        void Awake()
        {
            var harm = new Harmony("KmanMenu.Patchers.Playfab");
            harm.PatchAll();
        }
    }

    [HarmonyPatch(typeof(PlayFabHttp), "InitializeScreenTimeTracker")]
    internal class NoInitializeScreenTimeTracker : MonoBehaviour
    {
        private static bool Prefix()
        {
            Debug.Log("NoInitializeScreenTimeTracker");
            return false;
        }
    }

    [HarmonyPatch(typeof(PlayFabDeviceUtil), "GetAdvertIdFromUnity")]
    internal class NoGetAdvertIdFromUnity : MonoBehaviour
    {
        private static bool Prefix()
        {
            Debug.Log("NoGetAdvertIdFromUnity");
            return false;
        }
    }

    [HarmonyPatch(typeof(PlayFabDeviceUtil), "DoAttributeInstall")]
    internal class NoDoAttributeInstall : MonoBehaviour
    {
        private static bool Prefix()
        {
            Debug.Log("NoDoAttributeInstall");
            return false;
        }
    }

    [HarmonyPatch(typeof(PlayFabClientInstanceAPI), "ReportDeviceInfo")]
    internal class NoDeviceInfo2 : MonoBehaviour
    {
        private static bool Prefix()
        {
            Debug.Log("NoDeviceInfo2");
            return false;
        }
    }

    [HarmonyPatch(typeof(PlayFabClientAPI), "ReportDeviceInfo")]
    internal class NoDeviceInfo1 : MonoBehaviour
    {
        private static bool Prefix()
        {
            Debug.Log("NoDeviceInfo1");
            return false;
        }
    }

    [HarmonyPatch(typeof(PlayFabDeviceUtil), "SendDeviceInfoToPlayFab")]
    internal class NoDeviceInfo : MonoBehaviour
    {
        private static bool Prefix()
        {
            Debug.Log("NoDeviceInfo");
            return false;
        }
    }

    [HarmonyPatch(typeof(PlayFabClientAPI), "AttributeInstall")]
    internal class NoAttributeInstall : MonoBehaviour
    {
        private static bool Prefix()
        {
            Debug.Log("NoAttributeInstall");
            return false;
        }
    }
}
