using BepInEx;
using BepInEx.Logging;
using GorillaLocomotion.Gameplay;
using GorillaNetworking;
using HarmonyLib;
using KmanMenu.Components;
using KmanMenu.Helpers;
using KmanMenu.Helpers.Helper;
using KmanMenu.Helpers.Notifacations;
using Photon.Pun;
using Photon.Realtime;
using System;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Valve.VR;
using Random = UnityEngine.Random;

namespace KmanMenu
{
    [HarmonyPatch(typeof(GorillaLocomotion.Player), "FixedUpdate", MethodType.Normal)]
    [BepInPlugin("com.Kman.KmanMenuV7", "KmanMenuV7", "7.0.0")]
    public class Plugin : BaseUnityPlugin
    {
        public static ManualLogSource debug;
        public static GameObject Loader = null;
        public static bool KillSwitch
        {
            get
            {
                return false;
            }
        }

        public static bool ghostToggled;

        public static string[] buttons = new string[]
        {
            "Reconnect",
            "Platforms",
            "Fly",
            "Speed Boost",
            "No Tag Freeze",
            "Beacons",
            "Tag Gun",
            "Tag All",
            "ESP",
            "Tracers",
            "Ghost Monkey {RT}",
            "Invis Monkey {RT}",
            "Teleport Gun",
            "Check Point",
            "Magic Monkey",
            "WallWalk {RG/LG}",
            "Projectile Spam {SlingShot}",
            "Projectile Gun {SlingShot}",
            "Projectile Halo {SlingShot}",
            "Projectile Rain {SlingShot}",
            "Piss {RT}",
            "Cum {RT}",
            "Rope Up",
            "Rope Down",
            "Rope Freeze {LAGGY}",
            "Rope To Self",
            "Rope Gun",
            "Splash {RT/LT}",
            "Kick Gun {STUMP} {PRIV}",
            "Kick All {STUMP} {PRIV}",
            "Lag Gun",
            "Lag All",
            "Touch Lag {NW}",
            "Touch Kick {NW}",
            "RGB {STUMP}",
            "Sound Spam {RT} {M}",
            "Mat Spam {M}",
            "Mat Spam Self {M}",
            "Slow All {M}",
            "Vibrate All {M}",
            "Slow Gun {M}",
            "Vibrate Gun {M}",
        };
        static bool _init = false;
        private static int pageSize = 5;
        private static int pageNumber = 0;
        public static GameObject menu = null;
        public static GameObject menubg = null;
        public static GameObject canvasObj = null;
        public static GameObject MenuClick;
        public static int framePressCooldown = 0;
        public static float refreshBoardCooldown;
        static bool AllowProjChange = false;
        static int ProjType = 0;
        static int Projhash = -820530352;
        public static int projgunhash = -820530352;
        public static int projguntype = 0;
        public static int projhalohash = -820530352;
        public static int projhalotype = 0;

        public static float projectiletimeout = 0;

        // YES ITS FROM MANGO I GOT ORAL PERMISSION TO USE HIS DRAW FUNCTION IT WAS TAKEN FROM DNSPY BECAUSE HE DIDNT HAVE THE CODE
        #region Draw
        private static void AddButton(float offset, string text)
        {
            GameObject gameObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
            UnityEngine.Object.Destroy(gameObject.GetComponent<Rigidbody>());
            gameObject.GetComponent<BoxCollider>().isTrigger = true;
            gameObject.transform.parent = menu.transform;
            gameObject.transform.rotation = Quaternion.identity;
            gameObject.transform.localScale = new Vector3(0.09f, 0.8f, 0.08f);
            gameObject.transform.localPosition = new Vector3(0.56f, 0f, 0.58f - offset);
            gameObject.AddComponent<BtnCollider>().relatedText = text;
            gameObject.name = text;
            int num = -1;

            for (int i = 0; i < buttons.Length; i++)
            {
                if (text == buttons[i])
                {
                    num = i;
                    break;
                }
            }

            if (buttonsActive[num])
            {
                gameObject.GetComponent<Renderer>().material.color = Color.red * 0.5f;
            }
            else
            {
                GradientColorKey[] colorkeys = new GradientColorKey[4];

                colorkeys[0].color = Color.black;
                colorkeys[0].time = 0f;

                colorkeys[1].color = Color.red * 0.4f;
                colorkeys[1].time = 0.5f;

                colorkeys[2].color = Color.black;
                colorkeys[2].time = 1f;

                var changer = gameObject.AddComponent<bgchanger>();
                Gradient gradient = new Gradient();
                gradient.colorKeys = colorkeys;
                changer.Gradients = gradient;
            }

            GameObject gameObject2 = new GameObject();
            gameObject2.transform.parent = canvasObj.transform;

            Text text2 = gameObject2.AddComponent<Text>();
            text2.font = Resources.GetBuiltinResource(typeof(Font), "Arial.ttf") as Font;
            text2.text = text;
            text2.fontSize = 1;
            text2.fontStyle = FontStyle.Italic;
            text2.alignment = TextAnchor.MiddleCenter;
            text2.resizeTextForBestFit = true;
            text2.resizeTextMinSize = 0;

            RectTransform component = text2.GetComponent<RectTransform>();
            component.localPosition = Vector3.zero;
            component.sizeDelta = new Vector2(0.2f, 0.03f);
            component.localPosition = new Vector3(0.064f, 0f, 0.231f - offset / 2.55f);
            component.rotation = Quaternion.Euler(new Vector3(180f, 90f, 90f));
        }
        public static void Draw()
        {
            menu = GameObject.CreatePrimitive(PrimitiveType.Cube);
            UnityEngine.Object.Destroy(menu.GetComponent<Rigidbody>());
            UnityEngine.Object.Destroy(menu.GetComponent<BoxCollider>());
            UnityEngine.Object.Destroy(menu.GetComponent<Renderer>());
            menu.transform.localScale = new Vector3(0.1f, 0.3f, 0.4f);
            menu.name = "Menu";

            GameObject menubg = GameObject.CreatePrimitive(PrimitiveType.Cube);
            UnityEngine.Object.Destroy(menubg.GetComponent<Rigidbody>());
            UnityEngine.Object.Destroy(menubg.GetComponent<BoxCollider>());
            menubg.transform.parent = menu.transform;
            menubg.transform.rotation = Quaternion.identity;
            menubg.transform.localScale = new Vector3(0.1f, 0.94f, 1f);
            menubg.name = "Menucolor";
            menubg.transform.position = new Vector3(0.054f, 0, 0f);
            menubg.GetComponent<Renderer>().material.color = Color.black;

            GradientColorKey[] colorkeys = new GradientColorKey[4];

            colorkeys[0].color = Color.black;
            colorkeys[0].time = 0f;

            colorkeys[1].color = Color.red * 0.4f;
            colorkeys[1].time = 0.5f;

            colorkeys[2].color = Color.black;
            colorkeys[2].time = 1f;

            var changer = menubg.AddComponent<bgchanger>();
            Gradient gradient = new Gradient();
            gradient.colorKeys = colorkeys;
            changer.Gradients = gradient;

            canvasObj = new GameObject();
            canvasObj.transform.parent = menu.transform;
            canvasObj.name = "canvas";
            Canvas canvas = canvasObj.AddComponent<Canvas>();
            CanvasScaler canvasScaler = canvasObj.AddComponent<CanvasScaler>();
            canvasObj.AddComponent<GraphicRaycaster>();
            canvas.renderMode = RenderMode.WorldSpace;
            canvasScaler.dynamicPixelsPerUnit = 1000f;

            GameObject gameObject2 = new GameObject();

            gameObject2.transform.parent = canvasObj.transform;
            gameObject2.name = "Title";
            Text text = gameObject2.AddComponent<Text>();
            text.font = Resources.GetBuiltinResource(typeof(Font), "Arial.ttf") as Font;
            text.text = "Kman Menu V7";
            text.fontSize = 1;
            text.fontStyle = FontStyle.Italic;
            text.alignment = TextAnchor.MiddleCenter;
            text.resizeTextForBestFit = true;
            text.resizeTextMinSize = 0;
            RectTransform component = text.GetComponent<RectTransform>();
            component.localPosition = Vector3.zero;
            component.sizeDelta = new Vector2(0.28f, 0.05f);
            component.position = new Vector3(0.06f, 0f, 0.162f);
            component.rotation = Quaternion.Euler(new Vector3(180f, 90f, 90f));

            AddPageButtons();
            string[] array2 = buttons.Skip(pageNumber * pageSize).Take(pageSize).ToArray();
            for (int i = 0; i < array2.Length; i++)
            {
                AddButton((float)i * 0.13f + 0.45f, array2[i]);
            }
        }
        private static void AddPageButtons()
        {
            int num = (buttons.Length + pageSize - 1) / pageSize;
            int num2 = pageNumber + 1;
            int num3 = pageNumber - 1;
            if (num2 > num - 1)
            {
                num2 = 0;
            }
            if (num3 < 0)
            {
                num3 = num - 1;
            }
            GameObject gameObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
            UnityEngine.Object.Destroy(gameObject.GetComponent<Rigidbody>());
            gameObject.GetComponent<BoxCollider>().isTrigger = true;
            gameObject.transform.parent = menu.transform;
            gameObject.transform.rotation = Quaternion.identity;
            gameObject.transform.localScale = new Vector3(0.09f, 0.15f, 0.945f);
            gameObject.transform.localPosition = new Vector3(0.56f, 0.5833f, 0f);
            gameObject.AddComponent<BtnCollider>().relatedText = "PreviousPage";
            gameObject.GetComponent<Renderer>().material.SetColor("_Color", Color.black);
            gameObject.name = "back";
            GradientColorKey[] colorkeys = new GradientColorKey[4];

            colorkeys[0].color = Color.black;
            colorkeys[0].time = 0f;

            colorkeys[1].color = Color.red * 0.4f;
            colorkeys[1].time = 0.5f;

            colorkeys[2].color = Color.black;
            colorkeys[2].time = 1f;

            var changer = gameObject.AddComponent<bgchanger>();
            Gradient gradient = new Gradient();
            gradient.colorKeys = colorkeys;
            changer.Gradients = gradient;

            GameObject gameObject2 = new GameObject();
            gameObject2.transform.parent = canvasObj.transform;
            gameObject2.name = "back";
            Text text = gameObject2.AddComponent<Text>();
            text.font = Resources.GetBuiltinResource(typeof(Font), "Arial.ttf") as Font;
            text.text = "<";
            text.fontSize = 1;
            text.alignment = TextAnchor.MiddleCenter;
            text.resizeTextForBestFit = true;
            text.resizeTextMinSize = 0;

            RectTransform component = text.GetComponent<RectTransform>();
            component.localPosition = Vector3.zero;
            component.sizeDelta = new Vector2(0.2f, 0.03f);
            component.localPosition = new Vector3(0.064f, 0.175f, 0.0036f);
            component.rotation = Quaternion.Euler(new Vector3(180f, 90f, 90f));
            component.localScale = new Vector3(1.3f, 1.3f, 1.3f);


            GameObject gameObject3 = GameObject.CreatePrimitive(PrimitiveType.Cube);
            UnityEngine.Object.Destroy(gameObject3.GetComponent<Rigidbody>());
            gameObject3.GetComponent<BoxCollider>().isTrigger = true;
            gameObject3.transform.parent = menu.transform;
            gameObject3.transform.rotation = Quaternion.identity;
            gameObject3.name = "Next";
            gameObject3.transform.localScale = new Vector3(0.09f, 0.15f, 0.945f);
            gameObject3.transform.localPosition = new Vector3(0.56f, -0.5833f, 0f);
            gameObject3.AddComponent<BtnCollider>().relatedText = "NextPage";
            gameObject3.GetComponent<Renderer>().material.SetColor("_Color", Color.black);
            var changer2 = gameObject3.AddComponent<bgchanger>();
            changer2.Gradients = gradient;

            GameObject gameObject4 = new GameObject();
            gameObject4.transform.parent = canvasObj.transform;
            gameObject4.name = "Next";
            Text text2 = gameObject4.AddComponent<Text>();
            text2.font = Resources.GetBuiltinResource(typeof(Font), "Arial.ttf") as Font;
            text2.text = ">";
            text2.fontSize = 1;
            text2.alignment = TextAnchor.MiddleCenter;
            text2.resizeTextForBestFit = true;
            text2.resizeTextMinSize = 0;

            RectTransform component2 = text2.GetComponent<RectTransform>();
            component2.localPosition = Vector3.zero;
            component2.sizeDelta = new Vector2(0.2f, 0.03f);
            component2.localPosition = new Vector3(0.064f, -0.175f, 0.0036f);
            component2.rotation = Quaternion.Euler(new Vector3(180f, 90f, 90f));
            component2.localScale = new Vector3(1.3f, 1.3f, 1.3f);






            GameObject gameObject5 = GameObject.CreatePrimitive(PrimitiveType.Cube);
            UnityEngine.Object.Destroy(gameObject5.GetComponent<Rigidbody>());
            gameObject5.GetComponent<BoxCollider>().isTrigger = true;
            gameObject5.transform.parent = menu.transform;
            gameObject5.transform.rotation = Quaternion.identity;
            gameObject5.name = "LeaveButton";
            gameObject5.transform.localScale = new Vector3(0.09f, 0.8f, 0.08f);
            gameObject5.transform.localPosition = new Vector3(0.56f, -0.0076f, 0.26f);
            gameObject5.AddComponent<BtnCollider>().relatedText = "Cum";
            gameObject5.GetComponent<Renderer>().material.color = Color.red;
            var changer55 = gameObject5.AddComponent<bgchanger>();
            changer55.Gradients = gradient;

            GameObject gameObject6 = new GameObject();
            gameObject6.transform.parent = canvasObj.transform;
            gameObject6.name = "LeaveButton";
            Text text3 = gameObject6.AddComponent<Text>();
            text3.font = Resources.GetBuiltinResource(typeof(Font), "Arial.ttf") as Font;
            text3.text = "Leave";
            text3.fontSize = 1;
            text3.alignment = TextAnchor.MiddleCenter;
            text3.resizeTextForBestFit = true;
            text3.resizeTextMinSize = 0;

            RectTransform component3 = text3.GetComponent<RectTransform>();
            component3.localPosition = Vector3.zero;
            component3.sizeDelta = new Vector2(0.2f, 0.03f);
            component3.localPosition = new Vector3(0.064f, 0, 0.106f);
            component3.rotation = Quaternion.Euler(new Vector3(180f, 90f, 90f));
            component3.localScale = new Vector3(1f, 1f, 1f);
        }
        public static void Toggle(string relatedText)
        {
            int num = (buttons.Length + pageSize - 1) / pageSize;
            if (relatedText == "Cum")
            {
                PhotonNetwork.Disconnect();
                UnityEngine.Object.Destroy(menu);
                menu = null;
                Draw();
                return;
            }
            if (relatedText == "NextPage")
            {
                if (pageNumber < num - 1)
                {
                    pageNumber++;
                }
                else
                {
                    pageNumber = 0;
                }
                UnityEngine.Object.Destroy(menu);
                menu = null;
                Draw();
                return;
            }
            if (relatedText == "PreviousPage")
            {
                if (pageNumber > 0)
                {
                    pageNumber--;
                }
                else
                {
                    pageNumber = num - 1;
                }
                UnityEngine.Object.Destroy(menu);
                menu = null;
                Draw();
                return;
            }
            int num2 = -1;
            for (int i = 0; i < buttons.Length; i++)
            {
                if (relatedText == buttons[i])
                {
                    num2 = i;
                    break;
                }
            }
            buttonsActive[num2] = !buttonsActive[num2];
            UnityEngine.Object.Destroy(menu);
            menu = null;
            Draw();
        }
        #endregion

        public static bool[] buttonsActive = new bool[]
        {
            false, false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,            false, false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,
            false, false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,
            false, false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,
            false, false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,
            false, false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,

        };









        void Awake()
        {
            if (!GameObject.Find("KmanV7Loader"))
            {
                debug = BepInEx.Logging.Logger.CreateLogSource("KMAN MENU");
                debug.Log(BepInEx.Logging.LogLevel.Info, "KMAN MENU LOADED!");
                var Harmony = new Harmony("com.Kman.KmanMenuV7");
                Harmony.PatchAll();
                debug.Log(LogLevel.Info, "Patched Harmony: " + Harmony.Id);
                Loader = new GameObject("KmanV7Loader");
                Loader.AddComponent<Plugin>();
                Loader.AddComponent<KmanUI>();
                Loader.AddComponent<Notif>();
                Loader.AddComponent<Helper>();
                Loader.AddComponent<Input>();
                Loader.AddComponent<AssetLoader>();
                Loader.AddComponent<RPCFlush>();
                var patchers = new GameObject("kmanpatchers");
                patchers.transform.SetParent(Loader.transform);
                patchers.AddComponent<Patchers.GorillaNotPatchers.Patch>();
                patchers.AddComponent<Patchers.Playfab.Patch>();
                patchers.AddComponent<Patchers.Misc.Patch>();
                patchers.AddComponent<Patchers.VRRigPatchers.Patch>();
                DontDestroyOnLoad(Loader);
                _init = true;

            }
        }


        public static void Prefix()
        {

            try
            {
                if (PhotonNetwork.IsConnectedAndReady)
                {
                    if (!_init)
                    {
                        debug.LogInfo("Appended Network Checkers!");
                        PhotonNetwork.NetworkingClient.EventReceived += Helper.instance.PlatformNetwork;
                        _init = true;
                    }
                }
                if (Input.LeftPrimary)
                {
                    if (menu == null)
                    {
                        Draw();
                        if (MenuClick == null)
                        {
                            MenuClick = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                            MenuClick.transform.SetParent(GorillaLocomotion.Player.Instance.rightControllerTransform);
                            MenuClick.name = "MenuClicker";
                            MenuClick.transform.localPosition = new Vector3(0f, -0.1f, 0f);
                            MenuClick.transform.localScale = new Vector3(0.01f, 0.01f, 0.01f);
                        }
                    }
                    menu.transform.position = GorillaLocomotion.Player.Instance.leftControllerTransform.position;
                    menu.transform.rotation = GorillaLocomotion.Player.Instance.leftControllerTransform.rotation;
                }
                else
                {
                    if (menu != null)
                    {
                        Destroy(menu);
                        menu = null;
                        Destroy(MenuClick);
                        MenuClick = null;
                    }
                }

                if (GorillaTagger.Instance.offlineVRRig != null && GorillaTagger.Instance.offlineVRRig.enabled)
                {
                    if (GorillaGameManager.instance != null)
                    {
                        GorillaTagger.Instance.offlineVRRig.ChangeMaterialLocal(GorillaGameManager.instance.MyMatIndex(PhotonNetwork.LocalPlayer));
                    }
                    else
                    {
                        GorillaTagger.Instance.offlineVRRig.ChangeMaterialLocal(0);
                    }
                }

                if (buttonsActive[0])
                {
                    PhotonNetwork.ConnectUsingSettings();
                    PhotonNetworkController.Instance.AttemptToJoinSpecificRoom(Helper.oldRoom);
                }
                if (buttonsActive[1])
                {
                    Helper.instance.Platforms();
                }
                if (buttonsActive[2])
                {
                    if (Input.RightPrimary)
                    {
                        GorillaLocomotion.Player.Instance.transform.position += (GorillaLocomotion.Player.Instance.headCollider.transform.forward * Time.deltaTime) * 15;
                        GorillaLocomotion.Player.Instance.GetComponent<Rigidbody>().velocity = Vector3.zero;
                    }
                }
                if (buttonsActive[3])
                {
                    if (GorillaGameManager.instance != null && GorillaGameManager.instance.fastJumpLimit != 9999)
                    {
                        GorillaGameManager.instance.fastJumpLimit = 9999;
                        GorillaGameManager.instance.fastJumpMultiplier = 9999;
                    }
                    GorillaLocomotion.Player.Instance.jumpMultiplier = 1.3f;
                    GorillaLocomotion.Player.Instance.maxJumpSpeed = 11;
                }
                if (buttonsActive[4])
                {
                    GorillaLocomotion.Player.Instance.disableMovement = false;
                }
                if (buttonsActive[5])
                {
                    if (PhotonNetwork.CurrentRoom != null)
                    {
                        foreach (VRRig rig in GorillaParent.instance.vrrigs)
                        {
                            if (!rig.isOfflineVRRig && !rig.isMyPlayer)
                            {
                                GameObject beacon = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
                                GameObject.Destroy(beacon.GetComponent<BoxCollider>());
                                GameObject.Destroy(beacon.GetComponent<Rigidbody>());
                                GameObject.Destroy(beacon.GetComponent<CapsuleCollider>());
                                beacon.transform.rotation = Quaternion.identity;
                                beacon.transform.localScale = new Vector3(0.04f, 200f, 0.04f);
                                beacon.transform.position = rig.transform.position;
                                beacon.GetComponent<MeshRenderer>().material = rig.mainSkin.material;
                                GameObject.Destroy(beacon, Time.deltaTime);
                            }
                        }
                    }
                    else
                    {
                        Notif.SendNotification("Please Join a Room!");
                        buttonsActive[5] = false;
                        Destroy(menu);
                        menu = null;
                        Draw();

                    }
                }
                if (buttonsActive[6])
                {
                    Helper.instance.TagGun();
                }
                if (buttonsActive[7])
                {
                    Helper.instance.TagAll();
                }
                if (buttonsActive[8])
                {
                    if (PhotonNetwork.CurrentRoom != null)
                    {
                        foreach (VRRig vrrig in GorillaParent.instance.vrrigs)
                        {
                            if (!vrrig.isOfflineVRRig && !vrrig.isMyPlayer && vrrig.mainSkin.material.name.Contains("fected"))
                            {
                                vrrig.mainSkin.material.shader = Shader.Find("GUI/Text Shader");
                                vrrig.mainSkin.material.color = new Color(9f, 0f, 0f, 0.15f);
                            }
                            else if (!vrrig.isOfflineVRRig && !vrrig.isMyPlayer)
                            {
                                vrrig.mainSkin.material.shader = Shader.Find("GUI/Text Shader");
                                vrrig.mainSkin.material.color = new Color(0f, 9f, 0f, 0.15f);
                            }
                        }
                        StopEsp = false;
                    }
                    else
                    {
                        Notif.SendNotification("Please Join a Room!");
                        buttonsActive[8] = false;
                        Destroy(menu);
                        menu = null;
                        Draw();
                    }
                }
                else
                {
                    if (!StopEsp)
                    {
                        foreach (VRRig vrrig in GorillaParent.instance.vrrigs)
                        {
                            if (!vrrig.isOfflineVRRig && !vrrig.isMyPlayer)
                            {
                                vrrig.ChangeMaterialLocal(vrrig.currentMatIndex);
                            }
                        }
                        StopEsp = true;
                    }
                }
                if (buttonsActive[9])
                {
                    if (PhotonNetwork.CurrentRoom != null)
                    {
                        foreach (VRRig rig in GorillaParent.instance.vrrigs)
                        {
                            if (!rig.isOfflineVRRig && !rig.isMyPlayer)
                            {
                                GameObject lines = new GameObject("Line");
                                LineRenderer lr = lines.AddComponent<LineRenderer>();
                                var color = Color.green;
                                lr.startColor = color;
                                lr.endColor = color;
                                lr.startWidth = 0.01f;
                                lr.endWidth = 0.01f;
                                lr.positionCount = 2;
                                lr.useWorldSpace = true;
                                lr.SetPosition(0, GorillaLocomotion.Player.Instance.rightControllerTransform.position);
                                lr.SetPosition(1, rig.transform.position);
                                lr.material.shader = Shader.Find("GUI/Text Shader");
                                Destroy(lr, Time.deltaTime);
                                Destroy(lines, Time.deltaTime);
                            }
                        }
                    }
                    else
                    {
                        Notif.SendNotification("Please Join a Room!");
                        buttonsActive[9] = false;
                        Destroy(menu);
                        menu = null;
                        Draw();
                    }
                }
                if (buttonsActive[10])
                {
                    if (Input.RightTrigger)
                    {
                        if (!ghostToggled && GorillaTagger.Instance.offlineVRRig.enabled)
                        {
                            if (GorillaTagger.Instance.offlineVRRig.enabled)
                            {
                                Patchers.VRRigPatchers.OnDisable.Prefix(GorillaTagger.Instance.offlineVRRig);
                            }
                            GorillaTagger.Instance.offlineVRRig.enabled = false;
                            ghostToggled = true;
                        }
                        else
                        {
                            if (!ghostToggled && !GorillaTagger.Instance.offlineVRRig.enabled)
                            {
                                GorillaTagger.Instance.offlineVRRig.enabled = true;
                                ghostToggled = true;
                            }
                        }
                    }
                    else
                    {
                        ghostToggled = false;
                    }
                }
                if (buttonsActive[11])
                {
                    if (Input.RightTrigger)
                    {
                        if (!ghostToggled && GorillaTagger.Instance.offlineVRRig.enabled)
                        {
                            if (GorillaTagger.Instance.offlineVRRig.enabled)
                            {
                                Patchers.VRRigPatchers.OnDisable.Prefix(GorillaTagger.Instance.offlineVRRig);
                            }
                            GorillaTagger.Instance.offlineVRRig.enabled = false;
                            GorillaTagger.Instance.offlineVRRig.transform.position = new Vector3(999, 999, 999);
                            ghostToggled = true;
                        }
                        else
                        {
                            if (!ghostToggled && !GorillaTagger.Instance.offlineVRRig.enabled)
                            {
                                GorillaTagger.Instance.offlineVRRig.enabled = true;
                                ghostToggled = true;
                            }
                        }
                    }
                    else
                    {
                        ghostToggled = false;
                    }
                }
                if (buttonsActive[12])
                {
                    Helper.instance.ProcessTeleportGun();
                }
                if (buttonsActive[13])
                {
                    Helper.instance.ProcessCheckPoint();
                }
                if (buttonsActive[14])
                {
                    Helper.instance.MagicMonkey();
                }
                if (buttonsActive[15])
                {
                    RaycastHit Left;
                    RaycastHit Right;
                    Physics.Raycast(GorillaLocomotion.Player.Instance.rightControllerTransform.position, -GorillaLocomotion.Player.Instance.rightControllerTransform.right, out Left, 100f, int.MaxValue);
                    Physics.Raycast(GorillaLocomotion.Player.Instance.leftControllerTransform.position, GorillaLocomotion.Player.Instance.leftControllerTransform.right, out Right, 100f, int.MaxValue);

                    if (Input.RightGrip)
                    {
                        if (Left.distance < Right.distance)
                        {
                            if (Left.distance < 1)
                            {
                                Vector3 gravityDirection = (Left.point - GorillaLocomotion.Player.Instance.rightControllerTransform.position).normalized;
                                Physics.gravity = gravityDirection * 9.81f;
                            }
                            else
                            {
                                Physics.gravity = new Vector3(0, -9.81f, 0);
                            }
                        }
                        if (Left.distance == Right.distance)
                        {
                            Physics.gravity = new Vector3(0, -9.81f, 0);
                        }
                    }
                    else
                    {
                        Physics.gravity = new Vector3(0, -9.81f, 0);
                    }
                    if (Input.LeftGrip)
                    {
                        if (Left.distance > Right.distance)
                        {
                            if (Right.distance < 1)
                            {
                                Vector3 gravityDirection = (Right.point - GorillaLocomotion.Player.Instance.leftControllerTransform.position).normalized;
                                Physics.gravity = gravityDirection * 9.81f;
                            }
                            else
                            {
                                Physics.gravity = new Vector3(0, -9.81f, 0);
                            }
                        }
                        if (Left.distance == Right.distance)
                        {
                            Physics.gravity = new Vector3(0, -9.81f, 0);
                        }
                    }
                    else
                    {
                        Physics.gravity = new Vector3(0, -9.81f, 0);
                    }
                }
                
                if (buttonsActive[16])
                {
                    if (Input.RightPrimary)
                    {
                        if (AllowProjChange)
                        {
                            AllowProjChange = false;
                            ProjType++;
                            if (ProjType == 0)
                            {
                                buttons[16] = "Projectile Spam {SlingShot}";
                                Projhash = -820530352;
                                Notif.SendNotification("Changed Projectile: Slingshot");
                            }
                            if (ProjType == 1)
                            {
                                buttons[16] = "Projectile Spam {Waterballoon}";
                                Projhash = -1674517839;
                                Notif.SendNotification("Changed Projectile: Waterballoon");
                            }
                            if (ProjType == 2)
                            {
                                buttons[16] = "Projectile Spam {SnowBall}";
                                Projhash = -675036877;
                                Notif.SendNotification("Changed Projectile: Snowball");
                            }
                            if (ProjType == 3)
                            {
                                buttons[16] = "Projectile Spam {DeadShot}";
                                Projhash = 693334698;
                                Notif.SendNotification("Changed Projectile: DeadShot");
                            }
                            if (ProjType == 4)
                            {
                                buttons[16] = "Projectile Spam {Cloud}";
                                Projhash = 1511318966;
                                Notif.SendNotification("Changed Projectile: Cloud");
                            }
                            if (ProjType == 5)
                            {
                                buttons[16] = "Projectile Spam {Cupid}";
                                Projhash = 825718363;
                                Notif.SendNotification("Changed Projectile: Cupid");
                            }
                            if (ProjType == 6)
                            {
                                buttons[16] = "Projectile Spam {Ice}";
                                Projhash = -1671677000;
                                Notif.SendNotification("Changed Projectile: Ice");
                            }
                            if (ProjType == 7)
                            {
                                buttons[16] = "Projectile Spam {Elf}";
                                Projhash = 1705139863;
                                Notif.SendNotification("Changed Projectile: Elf");
                            }
                            if (ProjType == 8)
                            {
                                buttons[16] = "Projectile Spam {Rock}";
                                Projhash = PoolUtils.GameObjHashCode(GameObject.Find("Environment Objects/PersistentObjects_Prefab/GlobalObjectPools/LavaRockProjectile(Clone)"));
                                Notif.SendNotification("Changed Projectile: Rock");
                            }
                            if (ProjType == 9)
                            {
                                buttons[16] = "Projectile Spam {Spider}";
                                Projhash = -790645151;
                                Notif.SendNotification("Changed Projectile: Spider");
                            }
                            if (ProjType == 10)
                            {
                                buttons[16] = "Projectile Spam {Coal}";
                                Projhash = -1433634409;
                                Notif.SendNotification("Changed Projectile: Coal");
                            }
                            if (ProjType == 11)
                            {
                                buttons[16] = "Projectile Spam {CandyCane}";
                                Projhash = 2061412059;
                                Notif.SendNotification("Changed Projectile: CandyCane");
                            }
                            if (ProjType == 12)
                            {
                                int[] hash = new int[]
                                {
                                    -666337545,
                                    -160604350,
                                    -1433633837,
                                };
                                buttons[16] = "Projectile Spam {Present}";
                                Projhash = UnityEngine.Random.Range(0, hash.Length);
                                Notif.SendNotification("Changed Projectile: Present");
                            }
                            if (ProjType >= 13)
                            {
                                ProjType = 0;
                                buttons[16] = "Projectile Spam {SlingShot}";
                                Projhash = -820530352;
                                Notif.SendNotification("Changed Projectile: Slingshot");
                            }

                            Destroy(menu);
                            menu = null;
                            Draw();

                        }
                    }
                    else
                    {
                        AllowProjChange = true;
                    }
                    if (Input.RightGrip)
                    {
                        if (GorillaGameManager.instance != null && Time.time > projectiletimeout + 0.002f)
                        {
                            if (GorillaLocomotion.Player.Instance.rightControllerTransform.gameObject.GetComponent<VelocityTracker>() == null)
                            {
                                GorillaLocomotion.Player.Instance.rightControllerTransform.gameObject.AddComponent<VelocityTracker>();
                            }
                            GorillaGameManager.instance.photonView.RPC("LaunchSlingshotProjectile", RpcTarget.Others, new object[]
                            {
                             GorillaLocomotion.Player.Instance.rightControllerTransform.position,
                             GorillaLocomotion.Player.Instance.rightControllerTransform.gameObject.GetComponent<VelocityTracker>().velocity,
                             Projhash,
                             -1,
                             true,
                             GorillaGameManager.instance.IncrementLocalPlayerProjectileCount(),
                             false,
                             1f,
                             1f,
                             1f,
                             1f,
                            });
                            GameObject gameObject = ObjectPools.instance.Instantiate(Projhash);
                            SlingshotProjectile component = gameObject.GetComponent<SlingshotProjectile>();
                            component.Launch(GorillaLocomotion.Player.Instance.rightControllerTransform.position, GorillaLocomotion.Player.Instance.rightControllerTransform.gameObject.GetComponent<VelocityTracker>().velocity, PhotonNetwork.LocalPlayer, blueTeam: false, orangeTeam: false, GorillaGameManager.instance.IncrementLocalPlayerProjectileCount(), Mathf.Abs(GorillaTagger.Instance.offlineVRRig.slingshot.projectilePrefab.transform.lossyScale.x), false);
                            projectiletimeout = Time.time;
                        }
                    }

                }
                if (buttonsActive[17])
                {
                    if (Input.RightPrimary)
                    {
                        if (AllowProjChange)
                        {
                            projguntype++;
                            if (projguntype == 0)
                            {
                                buttons[17] = "Projectile Gun {SlingShot}";
                                projgunhash = -820530352;
                                Notif.SendNotification("Changed Projectile: Slingshot");
                            }
                            if (projguntype == 1)
                            {
                                buttons[17] = "Projectile Gun {Waterballoon}";
                                projgunhash = -1674517839;
                                Notif.SendNotification("Changed Projectile: Waterballoon");
                            }
                            if (projguntype == 2)
                            {
                                buttons[17] = "Projectile Gun {SnowBall}";
                                projgunhash = -675036877;
                                Notif.SendNotification("Changed Projectile: Snowball");
                            }
                            if (projguntype == 3)
                            {
                                buttons[17] = "Projectile Gun {DeadShot}";
                                projgunhash = 693334698;
                                Notif.SendNotification("Changed Projectile: DeadShot");
                            }
                            if (projguntype == 4)
                            {
                                buttons[17] = "Projectile Gun {Cloud}";
                                projgunhash = 1511318966;
                                Notif.SendNotification("Changed Projectile: Cloud");
                            }
                            if (projguntype == 5)
                            {
                                buttons[17] = "Projectile Gun {Cupid}";
                                projgunhash = 825718363;
                                Notif.SendNotification("Changed Projectile: Cupid");
                            }
                            if (projguntype == 6)
                            {
                                buttons[17] = "Projectile Gun {Ice}";
                                projgunhash = -1671677000;
                                Notif.SendNotification("Changed Projectile: Ice");
                            }
                            if (projguntype == 7)
                            {
                                buttons[17] = "Projectile Gun {Elf}";
                                projgunhash = 1705139863;
                                Notif.SendNotification("Changed Projectile: Elf");
                            }
                            if (projguntype == 8)
                            {
                                buttons[17] = "Projectile Gun {Rock}";
                                projgunhash = PoolUtils.GameObjHashCode(GameObject.Find("Environment Objects/PersistentObjects_Prefab/GlobalObjectPools/LavaRockProjectile(Clone)"));
                                Notif.SendNotification("Changed Projectile: Rock");
                            }
                            if (projguntype == 9)
                            {
                                buttons[17] = "Projectile Gun {Spider}";
                                projgunhash = -790645151;
                                Notif.SendNotification("Changed Projectile: Spider");
                            }
                            if (projguntype == 10)
                            {
                                buttons[17] = "Projectile Spam {Coal}";
                                projgunhash = -1433634409;
                                Notif.SendNotification("Changed Projectile: Coal");
                            }
                            if (projguntype == 11)
                            {
                                buttons[17] = "Projectile Spam {CandyCane}";
                                projgunhash = 2061412059;
                                Notif.SendNotification("Changed Projectile: CandyCane");
                            }
                            if (projguntype == 12)
                            {
                                int[] hash = new int[]
                                {
                                    -666337545,
                                    -160604350,
                                    -1433633837,
                                };
                                buttons[17] = "Projectile Spam {Present}";
                                projgunhash = UnityEngine.Random.Range(0, hash.Length);
                                Notif.SendNotification("Changed Projectile: Present");
                            }
                            if (projguntype >= 13)
                            {
                                projguntype = 0;
                                buttons[17] = "Projectile Spam {SlingShot}";
                                projgunhash = -820530352;
                                Notif.SendNotification("Changed Projectile: Slingshot");
                            }

                            Destroy(menu);
                            menu = null;
                            Draw();
                            AllowProjChange = false;
                        }
                    }
                    else
                    {
                        AllowProjChange = true;
                    }
                    if (Input.RightGrip)
                    {
                        RaycastHit raycastHit;
                        Physics.Raycast(GorillaTagger.Instance.rightHandTriggerCollider.transform.position, GorillaTagger.Instance.rightHandTriggerCollider.transform.up, out raycastHit);
                        Vector3 position = GorillaTagger.Instance.rightHandTriggerCollider.transform.position;
                        Vector3 point = raycastHit.point;
                        Vector3 vector = (point - position).normalized;
                        float d = 50f;
                        vector *= d;
                        if (GorillaGameManager.instance != null && Time.time > projectiletimeout + 0.002f)
                        {
                            GorillaGameManager.instance.photonView.RPC("LaunchSlingshotProjectile", RpcTarget.Others, new object[]
                            {
                             position,
                             vector,
                             projgunhash,
                             -1,
                             true,
                             GorillaGameManager.instance.IncrementLocalPlayerProjectileCount(),
                             false,
                             1f,
                             1f,
                             1f,
                             1f,
                            });
                            GameObject gameObject = ObjectPools.instance.Instantiate(projgunhash);
                            SlingshotProjectile component = gameObject.GetComponent<SlingshotProjectile>();
                            component.Launch(position, vector, PhotonNetwork.LocalPlayer, blueTeam: false, orangeTeam: false, GorillaGameManager.instance.IncrementLocalPlayerProjectileCount(), Mathf.Abs(GorillaTagger.Instance.offlineVRRig.slingshot.projectilePrefab.transform.lossyScale.x), false);
                            projectiletimeout = Time.time;
                        }
                    }

                }
                if (buttonsActive[18])
                {
                    if (Input.RightPrimary)
                    {
                        if (AllowProjChange)
                        {
                            projhalotype++;
                            if (projhalotype == 0)
                            {
                                buttons[18] = "Projectile Halo {SlingShot}";
                                projhalohash = -820530352;
                                Notif.SendNotification("Changed Projectile: Slingshot");
                            }
                            if (projhalotype == 1)
                            {
                                buttons[18] = "Projectile Halo {Waterballoon}";
                                projhalohash = -1674517839;
                                Notif.SendNotification("Changed Projectile: Waterballoon");
                            }
                            if (projhalotype == 2)
                            {
                                buttons[18] = "Projectile Halo {SnowBall}";
                                projhalohash = -675036877;
                                Notif.SendNotification("Changed Projectile: Snowball");
                            }
                            if (projhalotype == 3)
                            {
                                buttons[18] = "Projectile Halo {DeadShot}";
                                projhalohash = 693334698;
                                Notif.SendNotification("Changed Projectile: DeadShot");
                            }
                            if (projhalotype == 4)
                            {
                                buttons[18] = "Projectile Halo {Cloud}";
                                projhalohash = 1511318966;
                                Notif.SendNotification("Changed Projectile: Cloud");
                            }
                            if (projhalotype == 5)
                            {
                                buttons[18] = "Projectile Halo {Cupid}";
                                projhalohash = 825718363;
                                Notif.SendNotification("Changed Projectile: Cupid");
                            }
                            if (projhalotype == 6)
                            {
                                buttons[18] = "Projectile Halo {Ice}";
                                projhalohash = -1671677000;
                                Notif.SendNotification("Changed Projectile: Ice");
                            }
                            if (projhalotype == 7)
                            {
                                buttons[18] = "Projectile Halo {Elf}";
                                projhalohash = 1705139863;
                                Notif.SendNotification("Changed Projectile: Elf");
                            }
                            if (projhalotype == 8)
                            {
                                buttons[18] = "Projectile Halo {Rock}";
                                projhalohash = PoolUtils.GameObjHashCode(GameObject.Find("Environment Objects/PersistentObjects_Prefab/GlobalObjectPools/LavaRockProjectile(Clone)"));
                                Notif.SendNotification("Changed Projectile: Rock");
                            }
                            if (projhalotype == 9)
                            {
                                buttons[18] = "Projectile Halo {Spider}";
                                projhalohash = -790645151;
                                Notif.SendNotification("Changed Projectile: Spider");
                            }
                            if (projhalotype == 10)
                            {
                                buttons[18] = "Projectile Spam {Coal}";
                                projhalohash = -1433634409;
                                Notif.SendNotification("Changed Projectile: Coal");
                            }
                            if (projhalotype == 11)
                            {
                                buttons[18] = "Projectile Spam {CandyCane}";
                                projhalohash = 2061412059;
                                Notif.SendNotification("Changed Projectile: CandyCane");
                            }
                            if (projhalotype == 12)
                            {
                                int[] hash = new int[]
                                {
                                    -666337545,
                                    -160604350,
                                    -1433633837,
                                };
                                buttons[18] = "Projectile Spam {Present}";
                                projhalohash = UnityEngine.Random.Range(0, hash.Length);
                                Notif.SendNotification("Changed Projectile: Present");
                            }
                            if (projhalotype >= 13)
                            {
                                projhalotype = 0;
                                buttons[18] = "Projectile Spam {SlingShot}";
                                projhalohash = -820530352;
                                Notif.SendNotification("Changed Projectile: Slingshot");
                            }

                            Destroy(menu);
                            menu = null;
                            Draw();
                            AllowProjChange = false;
                        }
                    }
                    else
                    {
                        AllowProjChange = true;
                    }
                    if (Input.RightGrip)
                    {
                        if (GorillaGameManager.instance != null && Time.time > projectiletimeout + 0.002f)
                        {
                            chatgpt += 21 * Time.deltaTime;
                            float x = GorillaTagger.Instance.offlineVRRig.headConstraint.transform.position.x + 0.5f * Mathf.Cos(chatgpt);
                            float y = GorillaTagger.Instance.offlineVRRig.headConstraint.transform.position.y + 0.5f;
                            float z = GorillaTagger.Instance.offlineVRRig.headConstraint.transform.position.z + 0.5f * Mathf.Sin(chatgpt);
                            GorillaGameManager.instance.photonView.RPC("LaunchSlingshotProjectile", RpcTarget.Others, new object[]
                            {
                             new Vector3(x, y, z),
                             new Vector3(0,0,0),
                             projhalohash,
                             -1,
                             true,
                             GorillaGameManager.instance.IncrementLocalPlayerProjectileCount(),
                             false,
                             1f,
                             1f,
                             1f,
                             1f,
                            });
                            GameObject gameObject = ObjectPools.instance.Instantiate(projhalohash);
                            SlingshotProjectile component = gameObject.GetComponent<SlingshotProjectile>();
                            component.Launch(new Vector3(x, y, z), new Vector3(0, 0, 0), PhotonNetwork.LocalPlayer, blueTeam: false, orangeTeam: false, GorillaGameManager.instance.IncrementLocalPlayerProjectileCount(), Mathf.Abs(GorillaTagger.Instance.offlineVRRig.slingshot.projectilePrefab.transform.lossyScale.x), false);
                            projectiletimeout = Time.time;
                        }
                    }

                }
                if (buttonsActive[19])
                {
                    if (Input.RightPrimary)
                    {
                        if (AllowProjChange)
                        {
                            ProjType++;
                            if (ProjType == 0)
                            {
                                buttons[19] = "Projectile Rain {SlingShot}";
                                Projhash = -820530352;
                                Notif.SendNotification("Changed Projectile: Slingshot");
                            }
                            if (ProjType == 1)
                            {
                                buttons[19] = "Projectile Rain {Waterballoon}";
                                Projhash = -1674517839;
                                Notif.SendNotification("Changed Projectile: Waterballoon");
                            }
                            if (ProjType == 2)
                            {
                                buttons[19] = "Projectile Rain {SnowBall}";
                                Projhash = -675036877;
                                Notif.SendNotification("Changed Projectile: Snowball");
                            }
                            if (ProjType == 3)
                            {
                                buttons[19] = "Projectile Rain {DeadShot}";
                                Projhash = 693334698;
                                Notif.SendNotification("Changed Projectile: DeadShot");
                            }
                            if (ProjType == 4)
                            {
                                buttons[19] = "Projectile Rain {Cloud}";
                                Projhash = 1511318966;
                                Notif.SendNotification("Changed Projectile: Cloud");
                            }
                            if (ProjType == 5)
                            {
                                buttons[19] = "Projectile Rain {Cupid}";
                                Projhash = 825718363;
                                Notif.SendNotification("Changed Projectile: Cupid");
                            }
                            if (ProjType == 6)
                            {
                                buttons[19] = "Projectile Rain {Ice}";
                                Projhash = -1671677000;
                                Notif.SendNotification("Changed Projectile: Ice");
                            }
                            if (ProjType == 7)
                            {
                                buttons[19] = "Projectile Rain {Elf}";
                                Projhash = 1705139863;
                                Notif.SendNotification("Changed Projectile: Elf");
                            }
                            if (ProjType == 8)
                            {
                                buttons[19] = "Projectile Rain {Rock}";
                                Projhash = PoolUtils.GameObjHashCode(GameObject.Find("Environment Objects/PersistentObjects_Prefab/GlobalObjectPools/LavaRockProjectile(Clone)"));
                                Notif.SendNotification("Changed Projectile: Rock");
                            }
                            if (ProjType == 9)
                            {
                                buttons[19] = "Projectile Rain {Spider}";
                                Projhash = -790645151;
                                Notif.SendNotification("Changed Projectile: Spider");
                            }
                            if (ProjType == 10)
                            {
                                buttons[19] = "Projectile Spam {Coal}";
                                Projhash = -1433634409;
                                Notif.SendNotification("Changed Projectile: Coal");
                            }
                            if (ProjType == 11)
                            {
                                buttons[19] = "Projectile Spam {CandyCane}";
                                Projhash = 2061412059;
                                Notif.SendNotification("Changed Projectile: CandyCane");
                            }
                            if (ProjType == 12)
                            {
                                int[] hash = new int[]
                                {
                                    -666337545,
                                    -160604350,
                                    -1433633837,
                                };
                                buttons[19] = "Projectile Spam {Present}";
                                Projhash = UnityEngine.Random.Range(0, hash.Length);
                                Notif.SendNotification("Changed Projectile: Present");
                            }
                            if (ProjType >= 13)
                            {
                                ProjType = 0;
                                buttons[19] = "Projectile Spam {SlingShot}";
                                Projhash = -820530352;
                                Notif.SendNotification("Changed Projectile: Slingshot");
                            }

                            Destroy(menu);
                            menu = null;
                            Draw();
                            AllowProjChange = false;
                        }
                    }
                    else
                    {
                        AllowProjChange = true;
                    }
                    if (Input.RightGrip)
                    {
                        if (GorillaGameManager.instance != null && Time.time > projectiletimeout + 0.002f)
                        {
                            GorillaGameManager.instance.photonView.RPC("LaunchSlingshotProjectile", RpcTarget.Others, new object[]
                            {
                             GorillaTagger.Instance.offlineVRRig.transform.position + new Vector3(Random.Range(-5f, 5f), 5f, Random.Range(-5f, 5f)),
                             new Vector3(0,0,0),
                             Projhash,
                             -1,
                             true,
                             GorillaGameManager.instance.IncrementLocalPlayerProjectileCount(),
                             false,
                             1f,
                             1f,
                             1f,
                             1f,
                            });
                            GameObject gameObject = ObjectPools.instance.Instantiate(projgunhash);
                            SlingshotProjectile component = gameObject.GetComponent<SlingshotProjectile>();
                            component.Launch(GorillaTagger.Instance.offlineVRRig.transform.position + new Vector3(Random.Range(-5f, 5f), 5f, Random.Range(-5f, 5f)), Vector3.zero, PhotonNetwork.LocalPlayer, blueTeam: false, orangeTeam: false, GorillaGameManager.instance.IncrementLocalPlayerProjectileCount(), Mathf.Abs(GorillaTagger.Instance.offlineVRRig.slingshot.projectilePrefab.transform.lossyScale.x), false);
                            projectiletimeout = Time.time;
                        }
                    }

                }
                if (buttonsActive[20])
                {
                    if (Input.RightTrigger && Time.time > projectiletimeout + 0.01)
                    {
                        projectiletimeout = Time.time;
                        GorillaGameManager.instance.photonView.RPC("LaunchSlingshotProjectile", RpcTarget.Others, new object[]
                        {
                            GorillaLocomotion.Player.Instance.bodyCollider.transform.position + new Vector3(0f, -0.1f, 0f),
                            GorillaLocomotion.Player.Instance.bodyCollider.transform.forward * Time.deltaTime * 500f,
                            -820530352,
                            -1,
                            true,
                            GorillaGameManager.instance.IncrementLocalPlayerProjectileCount(),
                            true,
                            255f,
                            105f,
                            0f,
                            1f
                        });
                        GameObject gameObject = ObjectPools.instance.Instantiate(-820530352);
                        SlingshotProjectile component = gameObject.GetComponent<SlingshotProjectile>();
                        component.Launch(GorillaLocomotion.Player.Instance.bodyCollider.transform.position + new Vector3(0f, -0.1f, 0f), GorillaLocomotion.Player.Instance.bodyCollider.transform.forward * Time.deltaTime * 500f, PhotonNetwork.LocalPlayer, blueTeam: false, orangeTeam: false, GorillaGameManager.instance.IncrementLocalPlayerProjectileCount(), Mathf.Abs(GorillaTagger.Instance.offlineVRRig.slingshot.projectilePrefab.transform.lossyScale.x), false);

                    }
                }
                if (buttonsActive[21])
                {
                    if (Input.RightTrigger && Time.time > projectiletimeout + 0.01)
                    {
                        projectiletimeout = Time.time;
                        GorillaGameManager.instance.photonView.RPC("LaunchSlingshotProjectile", RpcTarget.Others, new object[]
                        {
                            GorillaLocomotion.Player.Instance.bodyCollider.transform.position + new Vector3(0f, -0.1f, 0f),
                            GorillaLocomotion.Player.Instance.bodyCollider.transform.forward * Time.deltaTime * 500f,
                            -820530352,
                            -1,
                            true,
                            GorillaGameManager.instance.IncrementLocalPlayerProjectileCount(),
                            true,
                            45f,
                            25f,
                            22f,
                            1f
                        });
                        GameObject gameObject = ObjectPools.instance.Instantiate(-820530352);
                        SlingshotProjectile component = gameObject.GetComponent<SlingshotProjectile>();
                        component.Launch(GorillaLocomotion.Player.Instance.bodyCollider.transform.position + new Vector3(0f, -0.1f, 0f), GorillaLocomotion.Player.Instance.bodyCollider.transform.forward * Time.deltaTime * 500f, PhotonNetwork.LocalPlayer, blueTeam: false, orangeTeam: false, GorillaGameManager.instance.IncrementLocalPlayerProjectileCount(), Mathf.Abs(GorillaTagger.Instance.offlineVRRig.slingshot.projectilePrefab.transform.lossyScale.x), false);

                    }
                }
                if (buttonsActive[22])
                {
                    if (Input.RightTrigger)
                    {
                        if (Time.time > ropetimeout + 0.2f)
                        {
                            ropetimeout = Time.time;
                            foreach (GorillaRopeSwing rope in FindObjectsOfType<GorillaRopeSwing>())
                            {
                                rope.photonView.RPC("SetVelocity", RpcTarget.All, new object[]
                                {
                                    4,
                                    new Vector3(0, 100,0),
                                    true,
                                    null
                                });
                            }
                        }
                    }
                }
                if (buttonsActive[23])
                {
                    if (Input.RightTrigger)
                    {
                        if (Time.time > ropetimeout + 0.2f)
                        {
                            ropetimeout = Time.time;
                            foreach (GorillaRopeSwing rope in FindObjectsOfType<GorillaRopeSwing>())
                            {
                                rope.photonView.RPC("SetVelocity", RpcTarget.All, new object[]
                                {
                                    4,
                                    new Vector3(0, -100,0),
                                    true,
                                    null
                                });
                            }

                        }
                    }
                }
                if (buttonsActive[24])
                {
                    if (Input.RightTrigger)
                    {
                        if (Time.time > ropetimeout + 0.02f)
                        {
                            ropetimeout = Time.time;
                            foreach (GorillaRopeSwing rope in FindObjectsOfType<GorillaRopeSwing>())
                            {
                                rope.photonView.RPC("SetVelocity", RpcTarget.All, new object[]
                                {
                                    4,
                                    new Vector3(0, 0,0),
                                    true,
                                    null
                                });
                            }

                        }
                    }
                }
                if (buttonsActive[25])
                {
                    if (Time.time > ropetimeout + 0.2f)
                    {
                        ropetimeout = Time.time;
                        foreach (GorillaRopeSwing rope in FindObjectsOfType<GorillaRopeSwing>())
                        {

                            Vector3 targetPosition = GorillaLocomotion.Player.Instance.transform.position;
                            Vector3 ropeToCursor = targetPosition - rope.transform.position;
                            float distanceToCursor = ropeToCursor.magnitude;
                            float speed = 9999;
                            float t = Mathf.Clamp01(speed / distanceToCursor);

                            Vector3 newPosition = rope.transform.position + ropeToCursor.normalized * distanceToCursor * t;
                            Vector3 velocity = (newPosition - rope.transform.position).normalized * speed;
                            rope.photonView.RPC("SetVelocity", RpcTarget.All, new object[]
                            {
                                    4,
                                    velocity,
                                    true,
                                    null
                            });
                        }

                    }
                }
                if (buttonsActive[26])
                {
                    RaycastHit raycastHit;
                    Physics.Raycast(GorillaTagger.Instance.rightHandTriggerCollider.transform.position, GorillaTagger.Instance.rightHandTriggerCollider.transform.up, out raycastHit);
                    Vector3 position = GorillaTagger.Instance.rightHandTriggerCollider.transform.position;
                    Vector3 point = raycastHit.point;
                    Vector3 vector = (point - position).normalized;
                    float d = 50f;
                    vector *= d;
                    if (Input.RightGrip)
                    {
                        if (Time.time > ropetimeout + 0.2f)
                        {
                            ropetimeout = Time.time;
                            foreach (GorillaRopeSwing rope in FindObjectsOfType<GorillaRopeSwing>())
                            {
                                rope.photonView.RPC("SetVelocity", RpcTarget.All, new object[]
                                {
                                    4,
                                    vector,
                                    true,
                                    null
                                });
                            }

                        }
                    }
                }
                if (buttonsActive[27])
                {
                    if (Time.time > splashtimeout + 0.5f)
                    {
                        if (Input.RightTrigger)
                        {
                            splashtimeout = Time.time;
                            GorillaTagger.Instance.myVRRig.RPC("PlaySplashEffect", RpcTarget.All, new object[]
                            {
                                GorillaTagger.Instance.offlineVRRig.rightHandTransform.position,
                                GorillaTagger.Instance.offlineVRRig.rightHandTransform.rotation,
                                20f,
                                8f,
                                false,
                                true
                            });
                        }

                        if (Input.LeftTrigger)
                        {
                            splashtimeout = Time.time;
                            GorillaTagger.Instance.myVRRig.RPC("PlaySplashEffect", RpcTarget.All, new object[]
                            {
                                GorillaTagger.Instance.offlineVRRig.leftHandTransform.position,
                                GorillaTagger.Instance.offlineVRRig.leftHandTransform.rotation,
                                20f,
                                8f,
                                false,
                                true
                            });
                        }
                    }
                }
                if (buttonsActive[28])
                {
                    Helper.instance.KickGun();
                }
                if (buttonsActive[29])
                {
                    if (!haskicked)
                    {
                        haskicked = true;
                        foreach (Player player in PhotonNetwork.PlayerListOthers)
                        {
                            if (GorillaComputer.instance.friendJoinCollider.playerIDsCurrentlyTouching.Contains(player.UserId) && GorillaComputer.instance.friendJoinCollider.playerIDsCurrentlyTouching.Contains(PhotonNetwork.LocalPlayer.UserId))
                            {
                                GorillaGameManager.instance.photonView.RPC("JoinPubWithFriends", player, PhotonNetworkController.Instance.shuffler, PhotonNetworkController.Instance.keyStr);
                            }
                        }
                    }
                }
                if (buttonsActive[30])
                {
                    Helper.instance.LagGun();
                }
                if (buttonsActive[31])
                {
                    if (GorillaGameManager.instance != null)
                    {
                        GorillaGameManager.instance.photonView.RPC("LaunchSlingshotProjectile", RpcTarget.All, new object[]
                        {
                             GorillaLocomotion.Player.Instance.transform.position - new Vector3(0, 1,0),
                             Vector3.zero,
                             PoolUtils.GameObjHashCode(GameObject.Find("Environment Objects/PersistentObjects_Prefab/GlobalObjectPools/LavaRockProjectile(Clone)")),
                             -1,
                             true,
                             GorillaGameManager.instance.IncrementLocalPlayerProjectileCount(),
                             false,
                             1f,
                             1f,
                             1f,
                             1f,
                        });
                        GorillaGameManager.instance.photonView.RPC("LaunchSlingshotProjectile", RpcTarget.All, new object[]
{
                             GorillaLocomotion.Player.Instance.transform.position - new Vector3(0, 1,0),
                             Vector3.zero,
                             PoolUtils.GameObjHashCode(GameObject.Find("Environment Objects/PersistentObjects_Prefab/GlobalObjectPools/LavaRockProjectile(Clone)")),
                             -1,
                             true,
                             GorillaGameManager.instance.IncrementLocalPlayerProjectileCount(),
                             false,
                             1f,
                             1f,
                             1f,
                             1f,
});
                    }

                }
                if (buttonsActive[32])
                {
                }
                if (buttonsActive[33])
                {
                }
                if (buttonsActive[34])
                {
                    GradientColorKey[] colorkeys = new GradientColorKey[7];
                    colorkeys[0].color = Color.red;
                    colorkeys[0].time = 0f;
                    colorkeys[1].color = Color.yellow;
                    colorkeys[1].time = 0.2f;
                    colorkeys[2].color = Color.green;
                    colorkeys[2].time = 0.3f;
                    colorkeys[3].color = Color.cyan;
                    colorkeys[3].time = 0.5f;
                    colorkeys[4].color = Color.blue;
                    colorkeys[4].time = 0.6f;
                    colorkeys[5].color = Color.magenta;
                    colorkeys[5].time = 0.8f;
                    colorkeys[6].color = Color.red;
                    colorkeys[6].time = 1f;
                    Gradient gradient = new Gradient();
                    gradient.colorKeys = colorkeys;
                    float t = Mathf.PingPong(Time.time / 2f, 1f);
                    var colortochange = gradient.Evaluate(t);

                    if (GorillaComputer.instance.friendJoinCollider.playerIDsCurrentlyTouching.Contains(PhotonNetwork.LocalPlayer.UserId))
                    {
                        GorillaTagger.Instance.myVRRig.RPC("InitializeNoobMaterial", RpcTarget.All, colortochange.r, colortochange.g, colortochange.b, true);
                    }
                }
                if (buttonsActive[35])
                {
                    if (Input.RightTrigger && PhotonNetwork.IsMasterClient && Time.time > sstimeout + 0.08)
                    {
                        sstimeout = Time.time;
                        GorillaTagger.Instance.myVRRig.RPC("PlayTagSound", RpcTarget.All, Random.Range(0, 20), 1000);
                    }
                }
                if (buttonsActive[36])
                {
                    foreach (Photon.Realtime.Player p in PhotonNetwork.PlayerList)
                    {
                        GorillaLocomotion.Player.Instance.disableMovement = false;
                        GorillaGameManager.instance.OnPlayerLeftRoom(p);
                        GorillaNot.instance.OnPlayerLeftRoom(p);
                        GorillaLocomotion.Player.Instance.disableMovement = false;
                    }
                }
                if (buttonsActive[37])
                {
                    if (!doonce)
                    {
                        Helper.instance.StartMatSelf();
                        doonce = true;
                    }
                }
                else
                {
                    doonce = false;
                }
                if (buttonsActive[38])
                {
                    if (PhotonNetwork.LocalPlayer.IsMasterClient)
                    {
                        foreach (Player p in PhotonNetwork.PlayerListOthers)
                        {
                            GorillaGameManager.instance.FindVRRigForPlayer(p).RPC("SetTaggedTime", p);
                        }
                    }
                }
                if (buttonsActive[39])
                {
                    if (PhotonNetwork.LocalPlayer.IsMasterClient)
                    {
                        foreach (Player p in PhotonNetwork.PlayerListOthers)
                        {
                            GorillaGameManager.instance.FindVRRigForPlayer(p).RPC("SetJoinTaggedTime", p);
                        }
                    }
                }
                if (buttonsActive[40])
                {
                    Helper.instance.SlowGun();
                }
                if (buttonsActive[41])
                {
                    Helper.instance.VibrateGun();
                }







            }
            catch (Exception ex)
            {
                debug.Log(LogLevel.Error, ex.ToString());
            }
        }
        static float splashtimeout;
        static bool doonce = false;
        static float sstimeout;
        static float ropetimeout;
        static bool StopEsp;
        static float chatgpt;
        static bool haskicked = false;
        static float boardcooldown;
    }
    internal class BtnCollider : MonoBehaviour
    {
        void OnTriggerEnter(Collider collider)
        {
            if (Time.frameCount >= Plugin.framePressCooldown + 10 && collider.gameObject.name == "MenuClicker")
            {
                AssetLoader.Instance.PlayClick();
                GorillaTagger.Instance.StartVibration(false, GorillaTagger.Instance.tagHapticStrength / 2, GorillaTagger.Instance.tagHapticDuration / 2);
                Plugin.Toggle(this.relatedText);
                Notif.ClearPastNotifications(100);
                Plugin.framePressCooldown = Time.frameCount;
            }
        }
        public string relatedText;
    }
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
    public class textchanger : MonoBehaviour
    {
        Text texttochange;
        public Gradient Gradients;

        public void Start()
        {
            texttochange = gameObject.GetComponent<Text>();
        }

        public void Update()
        {
            if (texttochange != null)
            {
                float t = Mathf.PingPong(Time.time / 2f, 1f);
                texttochange.color = Gradients.Evaluate(t); // from: https://gamedev.stackexchange.com/questions/98740/how-to-color-lerp-between-multiple-colors i didnt know how to do it
            }
        }
    }
    public class bgchanger : MonoBehaviour
    {
        Renderer renderer;
        public Gradient Gradients;

        public void Start()
        {
            renderer = gameObject.GetComponent<Renderer>();
        }

        public void Update()
        {
            if (renderer != null)
            {
                float t = Mathf.PingPong(Time.time / 2f, 1f);
                renderer.material.color = Gradients.Evaluate(t);
            }
        }
    }

}
