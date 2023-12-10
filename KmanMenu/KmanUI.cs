using BepInEx;
using ExitGames.Client.Photon;
using GorillaNetworking;
using KmanMenu.Helpers;
using KmanMenu.Helpers.Helper;
using KmanMenu.Patchers.Misc;
using Photon.Pun;
using Photon.Realtime;
using System;
using System.Reflection;
using UnityEngine;
using UnityEngine.InputSystem;
using Object = UnityEngine.Object;

namespace KmanMenu
{
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
        public static float speed = 10;
        static float grav = -9.81f;
        static bool sesp = false;
        static bool once2 = true;
        static GameObject pointer = null;
        static bool Spammer;
        static bool spammer2;
        static int hashtype = -820530352;
        GameObject tc;
        bool ai;
        bool boxesp;
        bool tracers;
        float r = 255;
        float g = 255;
        float b = 255;
        float a = 255;
        public static Color32 Espcolor = new Color32(255, 255, 255, 255);
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
                /*
                if (GUI.Button(new Rect(450, 10, 100, 35), "Fun"))
                {
                    AssetLoader.Instance.PlayClick();
                    page = 4;
                }
                if (GUI.Button(new Rect(560, 10, 100, 35), "Customization"))
                {
                    AssetLoader.Instance.PlayClick();
                    page = 5;
                }*/
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
                        if (GUI.Button(new Rect(230, 60, 100, 30), "Box ESP"))
                        {
                            AssetLoader.Instance.PlayClick();
                            boxesp = !boxesp;
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
                        GUILayout.Space(90);
                        for (int i = 0; i < PhotonNetwork.PlayerList.Length; i += 1)
                        {
                            if (PhotonNetwork.PlayerList[i].CustomProperties["mods"] != null)
                            {
                                //var obj = JsonConvert.DeserializeObject<hashinfo>(PhotonNetwork.PlayerList[i].CustomProperties["mods"].ToString().Replace('[', ' ').Replace('"', char.Parse("'")));
                                GUILayout.Label("NAME:" + PhotonNetwork.PlayerList[i].NickName + " | MODS: " + PhotonNetwork.PlayerList[i].CustomProperties["mods"].ToString() + " | USERID: " + PhotonNetwork.PlayerList[i].UserId);
                            }
                            else
                            {
                                GUILayout.Label("NAME: " + PhotonNetwork.PlayerList[i].NickName + " | MODS: NONE | USERID: " + PhotonNetwork.PlayerList[i].UserId);
                            }
                        }
                        break;

                    case 3:
                        GUI.skin.label.richText = true;
                        GUI.Label(new Rect(30, 60, 200, 30), "<b>Server Info</b>");
                        GUI.Label(new Rect(10, 80, 400, 300), $"Server: {PhotonNetwork.Server}\nServer Address: {PhotonNetwork.ServerAddress}\nRegion: {PhotonNetwork.CloudRegion}\nUsers Online: {PhotonNetwork.CountOfPlayers}\nPlayers In Room: {PhotonNetwork.CountOfPlayersInRooms}\nNumber Of Rooms: {(PhotonNetwork.CountOfPlayersInRooms / 10) + 1}\nConnected: {PhotonNetwork.IsConnected}\nPing: {PhotonNetwork.GetPing()}\n");
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

        void Update()
        {
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
                Helper.instance.AdvancedWASD();
            }
            if (ai)
            {
                Helper.instance.SelfControlledPath();
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
                Helper.instance.BoxESP();
            }
            if (tracers)
            {
                Helper.instance.Tracers();
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
