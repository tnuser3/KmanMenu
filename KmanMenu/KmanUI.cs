using BepInEx;
using ExitGames.Client.Photon;
using ExitGames.Client.Photon.StructWrapping;
using GorillaNetworking;
using HarmonyLib;
using KmanMenu.Components;
using KmanMenu.Helpers;
using KmanMenu.Patchers.Misc;
using Newtonsoft.Json;
using Photon.Pun;
using Photon.Realtime;
using PlayFab;
using PlayFab.EventsModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Reflection;
using System.Text.RegularExpressions;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.XR;
using static OVRPlugin;
using static System.Net.Mime.MediaTypeNames;
using Object = UnityEngine.Object;

namespace KmanMenu
{
    internal class hashinfo 
    { 
        public string installedIDs { get; set; }
        public string known { get; set; }
        public string assemblyHash { get; set; }
    }

    internal class KmanUI : MonoBehaviour
    {
        static bool Open = false;
        static bool stopopen = false;
        static float deltaTime;
        static int page = -1;
        static bool Follownearest = false;
        static VRRig closestrig;
        static bool followner = false;
        static bool Raisehands = false;
        static bool ESP = false;
        public GameObject mouseclicker;
        static bool WASD = false;
        internal static Vector3 previousMousePosition;
        static float speed = 10;
        static float grav = -9.81f;
        static bool sesp = false;
        static bool once2 = true;
        static GameObject pointer = null;
        static bool Spammer;
        static bool spammer2;
        static int hashtype = -820530352;
        GameObject tc;
        bool ai;
        public Vector3 LeftPos;
        public Vector3 RightPos;
        GameObject Left;
        GameObject Right;
        float reset;
        int RnumL;
        Vector3 PrevLPos;
        Vector3 PrevRPos;
        GameObject Test1;
        GameObject Test2;
        bool boxesp;
        bool tracers;
        bool hitboxesp;
        float r = 255;
        float g = 255;
        float b = 255;
        float a = 255;
        Color32 Espcolor = new Color32(255, 255, 255, 255);
        string room = "room code";
        static bool hold;
        void OnGUI()
        {
            if (UnityInput.Current.GetKeyDown(KeyCode.RightShift))
            {
                if (stopopen)
                {
                    AssetLoader.Instance.PlayClick();
                    Open = !Open;
                    stopopen = false;
                }
            }
            else
            {
                stopopen = true;
            }
            if (Open)
            {


                GUI.skin.font = Resources.GetBuiltinResource(typeof(Font), "Arial.ttf") as Font;
                deltaTime += (Time.unscaledDeltaTime - deltaTime) * 0.1f;
                GUI.BeginGroup(new Rect(0, 0, Screen.width, 100));

                if (GUI.Button(new Rect(10, 10, 100, 35), "Player"))
                {
                    AssetLoader.Instance.PlayClick();
                    page = 0;
                }
                if (GUI.Button(new Rect(120, 10, 100, 35), "Visual"))
                {
                    AssetLoader.Instance.PlayClick();
                    page = 1;
                }
                if (GUI.Button(new Rect(230, 10, 100, 35), "Room"))
                {
                    AssetLoader.Instance.PlayClick();
                    page = 2;
                }
                if (GUI.Button(new Rect(340, 10, 100, 35), "Server"))
                {
                    AssetLoader.Instance.PlayClick();
                    page = 3;
                }
                if (GUI.Button(new Rect(450, 10, 100, 35), "Fun"))
                {
                    AssetLoader.Instance.PlayClick();
                    page = 4;
                }
                if (GUI.Button(new Rect(560, 10, 100, 35), "Customization"))
                {
                    AssetLoader.Instance.PlayClick();
                    page = 5;
                }
                GUI.EndGroup();
                switch (page)
                {
                    case 0:
                        if (GUI.Button(new Rect(10, 60, 100, 30), "WASD"))
                        {
                            AssetLoader.Instance.PlayClick();
                            WASD = !WASD;
                        }
                        if (GUI.Button(new Rect(120, 60, 100, 30), "AI Walk"))
                        {
                            AssetLoader.Instance.PlayClick();
                            ai = !ai;
                        }
                        GUI.Label(new Rect(60, 100, 100, 20), $"Gravity: {Mathf.Round(grav)}");
                        grav = GUI.HorizontalSlider(new Rect(10, 120, 150, 20), grav, -20, 10);
                        if (grav != -9.81f && GUI.Button(new Rect(170, 115, 75, 20), $"Reset"))
                        {
                            AssetLoader.Instance.PlayClick();
                            grav = -9.81f;
                        }
                        GUI.Label(new Rect(30, 150, 200, 30), $"WASD Speed: {Mathf.Round(speed)}");
                        speed = GUI.HorizontalSlider(new Rect(10, 170, 150, 20), speed, 0, 50);
                        if (speed != 10f && GUI.Button(new Rect(170, 165, 75, 20), $"Reset"))
                        {
                            AssetLoader.Instance.PlayClick();
                            speed = 10f;
                        }
                        break;

                    case 1:

                        if (GUI.Button(new Rect(10, 60, 100, 30), "ESP"))
                        {
                            AssetLoader.Instance.PlayClick();
                            ESP = !ESP;
                        }
                        if (GUI.Button(new Rect(120, 60, 100, 30), "Tracers"))
                        {
                            AssetLoader.Instance.PlayClick();
                            tracers = !tracers;
                        }
                        GUI.Label(new Rect(40, 100, 100, 30), "Esp Color");
                        r = GUI.HorizontalSlider(new Rect(10, 120, 100, 20), r, 0, 255);
                        g = GUI.HorizontalSlider(new Rect(10, 150, 100, 20), g, 0, 255);
                        b = GUI.HorizontalSlider(new Rect(10, 180, 100, 20), b, 0, 255);
                        a = GUI.HorizontalSlider(new Rect(10, 210, 100, 20), a, 0, 255);
                        GUI.Label(new Rect(120, 120, 100, 20), "RED");
                        GUI.Label(new Rect(120, 150, 100, 20), "GREEN");
                        GUI.Label(new Rect(120, 180, 100, 20), "BLUE");
                        GUI.Label(new Rect(120, 210, 100, 20), "ALPHA");
                        Espcolor = new Color32((byte)r, (byte)g, (byte)b, (byte)a);
                        break;

                    case 2:
                        room = GUI.TextField(new Rect(10, 60, 100, 20), room);
                        if (GUI.Button(new Rect(10, 90, 100, 30), "Join Room"))
                        {
                            AssetLoader.Instance.PlayClick();
                            PhotonNetworkController.Instance.AttemptToJoinSpecificRoom(room);
                        }
                        if (GUI.Button(new Rect(120, 90, 100, 30), "Disconnect"))
                        {
                            PhotonNetwork.Disconnect();
                        }
                            for (int i = 0; i < PhotonNetwork.PlayerList.Length; i += 1)
                            {
                                if (PhotonNetwork.PlayerList[i].CustomProperties["mods"] != null)
                                {
                                    //var obj = JsonConvert.DeserializeObject<hashinfo>(PhotonNetwork.PlayerList[i].CustomProperties["mods"].ToString().Replace('[', ' ').Replace('"', char.Parse("'")));
                                    GUI.Label(new Rect(10, 90 + (25 * i), 700, 200), "NAME:" + PhotonNetwork.PlayerList[i].NickName + " | MODS: " + "HAS MODS" + " | USERID: " + PhotonNetwork.PlayerList[i].UserId);
                                }
                                else
                                {
                                    GUI.Label(new Rect(10, 90 + (25 * i), 700, 200),"NAME: " + PhotonNetwork.PlayerList[i].NickName + " | MODS: NONE | USERID: " + PhotonNetwork.PlayerList[i].UserId);
                                }
                            }
                        break;

                    case 3:
                        GUI.skin.label.richText = true;
                        GUI.Label(new Rect(30, 60, 200, 30), "<b>Server Info</b>");
                        GUI.Label(new Rect(10, 80, 400, 300), $"Server: {PhotonNetwork.Server}\nServer Address: {PhotonNetwork.ServerAddress}\nRegion: {PhotonNetwork.CloudRegion}\nUsers Online: {PhotonNetwork.CountOfPlayers}\nPlayers In Room: {PhotonNetwork.CountOfPlayersInRooms}\nNumber Of Rooms: {(PhotonNetwork.CountOfPlayersInRooms/10)+1}\nConnected: {PhotonNetwork.IsConnected}\nPing: {PhotonNetwork.GetPing()}\n");
                        if (GUI.Button(new Rect(10, 250, 120, 25), "Connect To US"))
                        {
                            AssetLoader.Instance.PlayClick();
                            DontReconnect.status = false;
                            PhotonNetwork.Disconnect();
                            PhotonNetwork.ConnectToRegion("us");
                            DontReconnect.status = true;
                        }
                        if (GUI.Button(new Rect(170, 250, 120, 25), "Connect To USW"))
                        {
                            AssetLoader.Instance.PlayClick();
                            DontReconnect.status = false;
                            PhotonNetwork.Disconnect();
                            PhotonNetwork.ConnectToRegion("usw");
                            DontReconnect.status = true;
                        }
                        if (GUI.Button(new Rect(10, 290, 120, 25), "Connect To EU"))
                        {
                            AssetLoader.Instance.PlayClick();
                            DontReconnect.status = false;
                            PhotonNetwork.Disconnect();
                            PhotonNetwork.ConnectToRegion("eu");
                            DontReconnect.status = true;
                        }
                        if (GUI.Button(new Rect(170, 290, 180, 25), "Connect To Default Server"))
                        {
                            AssetLoader.Instance.PlayClick();
                            DontReconnect.status = false;
                            PhotonNetwork.Disconnect();
                            PhotonNetwork.ConnectToBestCloudServer();
                            DontReconnect.status = true;
                        }
                        break;
                }
            }
        }
        GameObject parentbox;
        LineRenderer lineRenderer;
        private float chatgpt;

        public void Tracers()
        {
            foreach (VRRig rig in GorillaParent.instance.vrrigs)
            {
                if (rig != null && !rig.isOfflineVRRig && !rig.isMyPlayer)
                {
                    var gameobject = new GameObject("Line");
                    lineRenderer = gameobject.AddComponent<LineRenderer>();
                    lineRenderer.startColor = Espcolor;
                    lineRenderer.endColor = Color.green;
                    lineRenderer.startWidth = 0.01f;
                    lineRenderer.endWidth = 0.01f;
                    lineRenderer.positionCount = 2;
                    lineRenderer.useWorldSpace = true;
                    lineRenderer.SetPosition(0, GorillaLocomotion.Player.Instance.headCollider.transform.position);
                    lineRenderer.SetPosition(1, rig.transform.position);
                    lineRenderer.material.shader = Shader.Find("GUI/Text Shader");
                    Destroy(lineRenderer, Time.deltaTime);
                }
            }
        }
        public void BoxESP()
        {
            foreach (VRRig rig in GorillaParent.instance.vrrigs)
            {
                GameObject go = new GameObject("box");
                go.transform.position = rig.transform.position;
                GameObject top = GameObject.CreatePrimitive(PrimitiveType.Cube);
                GameObject right = GameObject.CreatePrimitive(PrimitiveType.Cube);
                GameObject left = GameObject.CreatePrimitive(PrimitiveType.Cube);
                GameObject bottom = GameObject.CreatePrimitive(PrimitiveType.Cube);
                Destroy(top.GetComponent<BoxCollider>());
                Destroy(bottom.GetComponent<BoxCollider>());
                Destroy(left.GetComponent<BoxCollider>());
                Destroy(right.GetComponent<BoxCollider>());
                top.transform.SetParent(go.transform);
                top.transform.localPosition = new Vector3(0f, 1f / 2f - 0.02f / 2f, 0f);
                top.transform.localScale = new Vector3(1f, 0.02f, 0.02f);
                bottom.transform.SetParent(go.transform);
                bottom.transform.localPosition = new Vector3(0f, (0f - 1f) / 2f + 0.02f / 2f, 0f);
                bottom.transform.localScale = new Vector3(1f, 0.02f, 0.02f);
                left.transform.SetParent(go.transform);
                left.transform.localPosition = new Vector3((0f - 1f) / 2f + 0.02f / 2f, 0f, 0f);
                left.transform.localScale = new Vector3(0.02f, 1f, 0.02f);
                right.transform.SetParent(go.transform);
                right.transform.localPosition = new Vector3(1f / 2f - 0.02f / 2f, 0f, 0f);
                right.transform.localScale = new Vector3(0.02f, 1f, 0.02f);

                top.GetComponent<Renderer>().material.shader = Shader.Find("GUI/Text Shader");
                bottom.GetComponent<Renderer>().material.shader = Shader.Find("GUI/Text Shader");
                left.GetComponent<Renderer>().material.shader = Shader.Find("GUI/Text Shader");
                right.GetComponent<Renderer>().material.shader = Shader.Find("GUI/Text Shader");

                top.GetComponent<Renderer>().material.color = Espcolor;
                bottom.GetComponent<Renderer>().material.color = Espcolor;
                left.GetComponent<Renderer>().material.color = Espcolor;
                right.GetComponent<Renderer>().material.color = Espcolor;

                go.transform.LookAt(go.transform.position + Camera.main.transform.rotation * Vector3.forward, Camera.main.transform.rotation * Vector3.up);
                Destroy(go, Time.deltaTime);
            }
        }
        public void AdvancedWASD()
        {
            GorillaTagger.Instance.rigidbody.velocity = new Vector3(0, 0.0735f, 0);
            float NSpeed = speed * Time.deltaTime;
            if (UnityInput.Current.GetKey(KeyCode.LeftShift) || UnityInput.Current.GetKey(KeyCode.RightShift))
            {
                NSpeed *= 10f;
            }
            if (UnityInput.Current.GetKey(KeyCode.LeftArrow) || UnityInput.Current.GetKey(KeyCode.A))
            {
                GorillaLocomotion.Player.Instance.transform.position += Camera.main.transform.right * -1f * NSpeed;
            }
            if (UnityInput.Current.GetKey(KeyCode.RightArrow) || UnityInput.Current.GetKey(KeyCode.D))
            {
                GorillaLocomotion.Player.Instance.transform.position += Camera.main.transform.right * NSpeed;
            }
            if (UnityInput.Current.GetKey(KeyCode.UpArrow) || UnityInput.Current.GetKey(KeyCode.W))
            {
                GorillaLocomotion.Player.Instance.transform.position += Camera.main.transform.forward * NSpeed;
            }
            if (UnityInput.Current.GetKey(KeyCode.DownArrow) || UnityInput.Current.GetKey(KeyCode.S))
            {
                GorillaLocomotion.Player.Instance.transform.position += Camera.main.transform.forward * -1f * NSpeed;
            }
            if (UnityInput.Current.GetKey(KeyCode.Space) || UnityInput.Current.GetKey(KeyCode.PageUp))
            {
                GorillaLocomotion.Player.Instance.transform.position += Camera.main.transform.up * NSpeed;
            }
            if (UnityInput.Current.GetKey(KeyCode.LeftControl) || UnityInput.Current.GetKey(KeyCode.PageDown))
            {
                GorillaLocomotion.Player.Instance.transform.position += Camera.main.transform.up * -1f * NSpeed;
            }
            if (UnityInput.Current.GetMouseButton(1))
            {
                Vector3 val = UnityInput.Current.mousePosition - previousMousePosition;
                float num2 = GorillaLocomotion.Player.Instance.transform.localEulerAngles.y + val.x * 0.3f;
                float num3 = GorillaLocomotion.Player.Instance.transform.localEulerAngles.x - val.y * 0.3f;
                GorillaLocomotion.Player.Instance.transform.localEulerAngles = new Vector3(num3, num2, 0f);
            }
            previousMousePosition = UnityInput.Current.mousePosition;

        }
        public void SelfControlledPath()
        {
            GorillaLocomotion.Player.Instance.rightControllerTransform.rotation = Quaternion.Euler(new Vector3(0, 0, 90));
            GorillaLocomotion.Player.Instance.leftControllerTransform.rotation = Quaternion.Euler(new Vector3(0, 0, -90));
            if (UnityInput.Current.GetKey(KeyCode.A))
            {
                GorillaLocomotion.Player.Instance.transform.Rotate(0f, -1f, 0f);
            }
            if (UnityInput.Current.GetKey(KeyCode.D))
            {
                GorillaLocomotion.Player.Instance.transform.Rotate(0f, 1f, 0f);
            }
            if (UnityInput.Current.GetKey(KeyCode.W))
            {
                GorillaLocomotion.Player.Instance.transform.position += (GorillaLocomotion.Player.Instance.bodyCollider.transform.forward * Time.deltaTime) * 1;
            }

            if (!GameObject.Find("Test1"))
            {
                Test1 = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                Test1.name = "Test1";
                Test1.GetComponent<SphereCollider>().enabled = false;
                Test1.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);
                Test1.GetComponent<Renderer>().material.color = Color.red;
            }
            if (!GameObject.Find("Test2"))
            {
                Test2 = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                Test2.name = "Test2";
                Test2.GetComponent<SphereCollider>().enabled = false;
                Test2.GetComponent<Renderer>().material.color = Color.red;
                Test2.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);
            }

            if (Left == null)
            {
                Left = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                Left.GetComponent<SphereCollider>().enabled = false;
                Left.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);
            }
            if (Right == null)
            {
                Right = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                Right.GetComponent<SphereCollider>().enabled = false;
                Right.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);
            }
            Physics.Raycast(Left.transform.position, -Left.transform.up, out RaycastHit Leftray, 5f, GorillaLocomotion.Player.Instance.locomotionEnabledLayers);
            Physics.Raycast(Right.transform.position, -Right.transform.up, out RaycastHit Rightray, 5f, GorillaLocomotion.Player.Instance.locomotionEnabledLayers);
            Left.transform.position = GorillaLocomotion.Player.Instance.headCollider.transform.position + GorillaLocomotion.Player.Instance.headCollider.transform.forward * 0f + GorillaLocomotion.Player.Instance.headCollider.transform.up * -0.1f + GorillaLocomotion.Player.Instance.headCollider.transform.right * 0.5f;
            Right.transform.position = GorillaLocomotion.Player.Instance.headCollider.transform.position + GorillaLocomotion.Player.Instance.headCollider.transform.forward * 0f + GorillaLocomotion.Player.Instance.headCollider.transform.up * -0.1f + GorillaLocomotion.Player.Instance.headCollider.transform.right * -0.5f;

            if (Rightray.point != null)
            {
                Test2.transform.position = Rightray.point;
            }
            if (Leftray.point != null)
            {
                Test1.transform.position = Leftray.point;
            }
            if (Time.time > reset + 0.65f)
            {
                if (RnumL >= 2)
                {
                    RnumL = 0;
                }
                else
                {
                    RnumL++;
                }
                if (RnumL == 2)
                {
                    RnumL = 0;
                }
                Debug.Log(RnumL);
                reset = Time.time;
                PrevLPos = Leftray.point;
                PrevRPos = Rightray.point;
                //LeftPos = GenerateRandomPointInFrontOfObject(GorillaLocomotion.Player.Instance.gameObject, Leftray);
            }
            else
            {
                if (RnumL == 0)
                {
                    GorillaLocomotion.Player.Instance.rightControllerTransform.position = Leftray.point;
                    GorillaLocomotion.Player.Instance.leftControllerTransform.position = PrevRPos;
                }
                if (RnumL == 1)
                {
                    GorillaLocomotion.Player.Instance.rightControllerTransform.position = PrevLPos;
                    GorillaLocomotion.Player.Instance.leftControllerTransform.position = Rightray.point;
                }
            }
        }
        void Update()
        {
            if (hold)
            {
                foreach (GorillaTagScripts.DecorativeItem d in Resources.FindObjectsOfTypeAll<GorillaTagScripts.DecorativeItem>())
                {
                    d.OnGrab(null, null);
                    chatgpt += 0.1f * Time.deltaTime;
                    float x = GorillaTagger.Instance.offlineVRRig.headConstraint.transform.position.x + 0.5f * Mathf.Cos(chatgpt);
                    float y = GorillaLocomotion.Player.Instance.headCollider.transform.position.y + 0f;
                    float z = GorillaTagger.Instance.offlineVRRig.headConstraint.transform.position.z + 0.5f * Mathf.Sin(chatgpt);
                    d.transform.position = new Vector3(x, y, z);
                }
            }
            if (Open)
            {
                tc = GorillaTagger.Instance.rightHandTriggerCollider;
                if (mouseclicker == null)
                {
                    mouseclicker = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                    mouseclicker.transform.localScale = tc.transform.localScale / 15f;
                    mouseclicker.layer = LayerMask.NameToLayer("TransparentFX");
                    Destroy(mouseclicker.GetComponent<SphereCollider>());
                }
                mouseclicker.GetComponent<Renderer>().enabled = false;
                Vector2 mousePos = Mouse.current.position.ReadValue();
                Ray ray = GorillaTagger.Instance.thirdPersonCamera.GetComponentInChildren<Camera>().ScreenPointToRay(mousePos);
                if (Physics.Raycast(ray, out RaycastHit hit, 500f, GorillaLocomotion.Player.Instance.locomotionEnabledLayers))
                {
                    mouseclicker.transform.position = hit.point;
                }

                if (UnityInput.Current.GetMouseButton(0))
                {
                    tc.GetComponent<TransformFollow>().enabled = !tc.GetComponent<TransformFollow>().enabled;
                    mouseclicker.GetComponent<Renderer>().material.color = tc.GetComponent<TransformFollow>().enabled ? Color.green : Color.white;
                    tc.transform.position = hit.point;
                }

            }
            Physics.gravity = new Vector3(0, grav, 0);

            if (WASD)
            {
                AdvancedWASD();
            }
            if (ai)
            {
                SelfControlledPath();
            }
            if (Spammer)
            {

                Ray ray = GameObject.Find("Shoulder Camera").GetComponent<Camera>().ScreenPointToRay(UnityInput.Current.mousePosition);

                RaycastHit raycastHit;
                Physics.Raycast(ray.origin, ray.direction, out raycastHit);
                if (pointer == null)
                {
                    pointer = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                    Object.Destroy(pointer.GetComponent<Rigidbody>());
                    Object.Destroy(pointer.GetComponent<SphereCollider>());
                    pointer.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);
                }
                pointer.transform.position = raycastHit.point;
                if (UnityInput.Current.GetMouseButton(0))
                {
                    Vector3 startPosition = GorillaTagger.Instance.offlineVRRig.transform.position;
                    Vector3 targetPosition = raycastHit.point;
                    Vector3 directionToTarget = (targetPosition - startPosition).normalized;
                    float strength = 140f;
                    directionToTarget *= strength;
                    int num2 = GorillaGameManager.instance.IncrementLocalPlayerProjectileCount();
                    GorillaGameManager.instance.photonView.RPC("LaunchSlingshotProjectile", RpcTarget.All, new object[]
                    {
                             startPosition,
                             directionToTarget,
                             hashtype,
                             -1,
                             true,
                             num2,
                             false,
                             1f,
                             1f,
                             1f,
                             1f,
                    });


                }

            }
            if (spammer2)
            {
                Ray ray = GameObject.Find("Shoulder Camera").GetComponent<Camera>().ScreenPointToRay(UnityInput.Current.mousePosition);

                RaycastHit raycastHit;
                Physics.Raycast(ray.origin, ray.direction, out raycastHit);
                if (pointer == null)
                {
                    pointer = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                    Object.Destroy(pointer.GetComponent<Rigidbody>());
                    Object.Destroy(pointer.GetComponent<SphereCollider>());
                    pointer.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);
                }
                pointer.transform.position = raycastHit.point;
                if (UnityInput.Current.GetMouseButton(0))
                {
                    Vector3 startPosition = GorillaTagger.Instance.offlineVRRig.transform.position;
                    Vector3 targetPosition = raycastHit.point;
                    Vector3 directionToTarget = (targetPosition - startPosition).normalized;
                    float strength = 140f;
                    directionToTarget *= strength;
                    float random1 = UnityEngine.Random.Range(0f, 1f);
                    float random2 = UnityEngine.Random.Range(0f, 1f);
                    float random3 = UnityEngine.Random.Range(0f, 1f);
                    Type photon = typeof(PhotonNetwork);
                    MethodInfo ExecuteRpc = photon.GetMethod("ExecuteRpc", BindingFlags.Static | BindingFlags.NonPublic);
                    MethodInfo RaiseEventInternal = photon.GetMethod("RaiseEventInternal", BindingFlags.Static | BindingFlags.NonPublic);
                    RaiseEventOptions RaiseEventOptionsInternal = new RaiseEventOptions
                    {
                        Receivers = ReceiverGroup.All
                    };
                    Hashtable tabel = new Hashtable();
                    tabel.Add(0, GorillaGameManager.instance.photonView.ViewID);
                    tabel.Add(2, (int)PhotonNetwork.Time);
                    tabel.Add(3, "LaunchSlingshotProjectile");
                    tabel.Add(4,
                    new object[]
                        {
                             startPosition,
                             directionToTarget,
                             -675036877,
                             -1,
                             true,
                             GorillaGameManager.instance.IncrementLocalPlayerProjectileCount(),
                            true,
                            random1,
                            random2,
                             random3,
                             1f,
                        });
                    RaiseEventInternal.Invoke(photon, new object[]
                    {
                    (byte)200,
                    tabel,
                    RaiseEventOptionsInternal,
                    SendOptions.SendReliable
                    });
                    ExecuteRpc.Invoke(photon, new object[] { tabel, PhotonNetwork.LocalPlayer });
                }
            }
            if (ESP)
            {
                foreach (VRRig rig in GorillaParent.instance.vrrigs)
                {
                    if (rig != null && !rig.isOfflineVRRig && !rig.isMyPlayer)
                    {
                        rig.mainSkin.material.shader = Shader.Find("GUI/Text Shader");
                        rig.mainSkin.material.color = Espcolor;
                        sesp = false;
                    }
                }
            }
            else
            {
                if (!sesp)
                {
                    if (!sesp)
                    {
                        foreach (VRRig vrrig in GorillaParent.instance.vrrigs)
                        {
                            if (!vrrig.isOfflineVRRig && !vrrig.isMyPlayer)
                            {
                                vrrig.ChangeMaterialLocal(vrrig.currentMatIndex);
                            }
                        }
                        sesp = true;
                    }
                    sesp = true;
                }
            }
            if (boxesp)
            {
                BoxESP();
            }
            if (tracers)
            {
                Tracers();
            }
            if (Raisehands)
            {
                GorillaLocomotion.Player.Instance.rightControllerTransform.transform.position += new Vector3(0, 10, 0);
                GorillaLocomotion.Player.Instance.leftControllerTransform.transform.position += new Vector3(0, 10, 0);
            }
            if (followner)
            {
                foreach (VRRig rig in GorillaParent.instance.vrrigs)
                {
                    if (!rig.isMyPlayer && rig.playerText.text != PhotonNetwork.LocalPlayer.NickName && !rig.isOfflineVRRig)
                    {
                        if (closestrig == null)
                        {
                            closestrig = rig;
                        }
                        if (Vector3.Distance(GorillaLocomotion.Player.Instance.transform.position, closestrig.transform.position) > Vector3.Distance(GorillaLocomotion.Player.Instance.transform.position, rig.transform.position))
                        {
                            closestrig = rig;
                        }
                    }
                    GorillaLocomotion.Player.Instance.rightControllerTransform.transform.position = closestrig.transform.position;
                    GorillaLocomotion.Player.Instance.leftControllerTransform.transform.position = closestrig.transform.position;
                }
            }
            if (Follownearest)
            {
                if (once2)
                {
                    MeshCollider[] array = Resources.FindObjectsOfTypeAll<MeshCollider>();
                    if (array != null)
                    {
                        foreach (MeshCollider collider in array)
                        {
                            if (!collider.gameObject.name.Contains("pit"))
                            {
                                if (collider.enabled == true)
                                {
                                    collider.enabled = false;
                                }
                                else
                                {
                                    collider.enabled = true;
                                }
                            }
                        }
                    }
                    once2 = false;
                }
                foreach (VRRig rig in GorillaParent.instance.vrrigs)
                {
                    if (!rig.isMyPlayer && rig.playerText.text != PhotonNetwork.LocalPlayer.NickName && !rig.isOfflineVRRig)
                    {
                        if (closestrig == null)
                        {
                            closestrig = rig;
                        }
                        if (Vector3.Distance(GorillaLocomotion.Player.Instance.transform.position, closestrig.transform.position) > Vector3.Distance(GorillaLocomotion.Player.Instance.transform.position, rig.transform.position))
                        {
                            closestrig = rig;
                        }
                    }
                    GorillaLocomotion.Player.Instance.bodyCollider.attachedRigidbody.velocity = new Vector3(0, 0, 0);
                    GorillaLocomotion.Player.Instance.transform.LookAt(closestrig.transform.position);
                    GorillaLocomotion.Player.Instance.transform.position = Vector3.Lerp(GorillaLocomotion.Player.Instance.transform.position, closestrig.transform.position, 0.1f * Time.deltaTime);
                    GorillaLocomotion.Player.Instance.rightControllerTransform.transform.position = closestrig.transform.position;
                    GorillaLocomotion.Player.Instance.leftControllerTransform.transform.position = closestrig.transform.position;
                }
            }
            else
            {
                if (!once2)
                {
                    MeshCollider[] array = Resources.FindObjectsOfTypeAll<MeshCollider>();
                    if (array != null)
                    {
                        foreach (MeshCollider collider in array)
                        {
                            if (!collider.gameObject.name.Contains("pit"))
                            {
                                if (collider.enabled == true)
                                {
                                    collider.enabled = false;
                                }
                                else
                                {
                                    collider.enabled = true;
                                }
                            }
                        }
                    }
                    once2 = true;
                }
            }
        }

    }
}
