using System;
using UnityEngine;
using BepInEx;
using HarmonyLib;
using KmanMenu.Components;
using KmanMenu.Helpers.Notifacations;
using KmanMenu.Helpers;

namespace KmanMenu
{
    [HarmonyPatch(typeof(GorillaLocomotion.Player), "FixedUpdate", MethodType.Normal)]
    [BepInPlugin("com.kman.kmanmenuv8", "kmanmenu", "8.0.0")]
    public class Main : BaseUnityPlugin
    {
        public void Update()
        {
            if (!GameObject.Find("KmanMenu"))
            {
                GameObject loader = new GameObject("KmanMenu");
                loader.AddComponent<Main>();
                loader.AddComponent<Notif>();
                loader.AddComponent<KmanMenu.Background.Input>();
                loader.AddComponent<AssetLoader>();
                loader.AddComponent<RPCFlush>();
                loader.AddComponent<KmanMenu.Patchers.GorillaNotPatchers.Patch>();
                loader.AddComponent<KmanMenu.Patchers.Playfab.Patch>();
                loader.AddComponent<KmanMenu.Patchers.Misc.Patch>();
                loader.AddComponent<KmanMenu.Patchers.VRRigPatchers.Patch>();
            }
        }
    }
}
