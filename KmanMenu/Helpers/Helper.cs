using Photon.Pun;
using Photon.Realtime;
using KmanMenu.Helpers.Notifacations;
using UnityEngine;
using GorillaNetworking;
using System.Collections;
using HarmonyLib;
using ExitGames.Client.Photon;
using BepInEx;

namespace KmanMenu.Helpers.Helper
{
    internal class Helper : MonoBehaviourPunCallbacks
    {
        GameObject Left;
        GameObject LeftPlat;
        GameObject Right;
        GameObject RightPlat;
        GameObject Test1;
        GameObject Test2;
        GameObject pointer;

        GameObject[] LeftPlat_Networked = new GameObject[9999];
        GameObject[] RightPlat_Networked = new GameObject[9999];

        GorillaBattleManager GorillaBattleManager;
        GorillaHuntManager GorillaHuntManager;
        GorillaTagManager GorillaTagManager;

        LineRenderer lineRenderer;

        VRRig KickPlayer;
        VRRig SlowingRig;
        VRRig Tagger;
        VRRig VibratingRig;
        VRRig lagrig;

        Vector3 PrevLPos;
        Vector3 PrevRPos;
        Vector3 previousMousePosition;
        Vector3 scale = new Vector3(0.0125f, 0.28f, 0.3825f);

        bool IsTaggedSelf;
        bool[] istagged = new bool[100000];

        float TagAura;
        float kicktimer = -2;
        float reset;

        int RnumL;

        private static Helper _Instance;
        public static Helper instance { get { return _Instance; } }
        public static string oldRoom;

        public Vector3 GetMiddle(Vector3 vector)
        {
            return new Vector3(vector.x / 2f, vector.y / 2f, vector.z / 2f);
        }

        public PhotonView GetPhotonViewFromRig(VRRig rig)
        {
            PhotonView info = Traverse.Create(rig).Field("photonView").GetValue<PhotonView>();
            if (info != null)
            {
                return info;
            }

            return null;
        }

        public void Awake()
        {
            _Instance = this;
        }

        public override void OnJoinedRoom()
        {
            base.OnJoinedRoom();
            oldRoom = PhotonNetwork.CurrentRoom.Name;

        }

        public override void OnLeftRoom()
        {
            base.OnLeftRoom();
            Notif.SendNotification("You have Left Room: " + oldRoom);
            oldRoom = string.Empty;
        }

        public override void OnPlayerEnteredRoom(Player newPlayer)
        {
            base.OnPlayerEnteredRoom(newPlayer);
            Notif.SendNotification(newPlayer.NickName + " Has Joined Room: " + oldRoom);

        }

        public override void OnPlayerLeftRoom(Player otherPlayer)
        {
            base.OnPlayerLeftRoom(otherPlayer);
            Notif.SendNotification(otherPlayer.NickName + " Has Left Room: " + oldRoom);
        }

        public void StartMatAll()
        {
            base.StartCoroutine(this.MatAll());
        }

        public void StartMatSelf()
        {
            base.StartCoroutine(this.MatSpamSelf());
        }

        public void ProcessTeleportGun()
        {
            if (Input.RightGrip && !Input.RightTrigger)
            {
                RaycastHit raycastHit;
                if (Physics.Raycast(GorillaLocomotion.Player.Instance.rightControllerTransform.position - GorillaLocomotion.Player.Instance.rightControllerTransform.up, -GorillaLocomotion.Player.Instance.rightControllerTransform.up, out raycastHit, 500, GorillaLocomotion.Player.Instance.locomotionEnabledLayers) && pointer == null)
                {
                    pointer = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                    UnityEngine.Object.Destroy(pointer.GetComponent<Rigidbody>());
                    UnityEngine.Object.Destroy(pointer.GetComponent<SphereCollider>());
                    pointer.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);
                    pointer.GetComponent<Renderer>().material.color = Color.green;
                }
                pointer.transform.position = raycastHit.point;
                return;
            }
            if (Input.RightTrigger && Input.RightGrip)
            {
                pointer.GetComponent<Renderer>().material.color = Color.red;
                GorillaLocomotion.Player.Instance.bodyCollider.attachedRigidbody.velocity = Vector3.zero;
                GorillaLocomotion.Player.Instance.transform.position = pointer.transform.position;
            }
            if (!Input.RightTrigger)
            {
                pointer.GetComponent<Renderer>().material.color = Color.green;
            }
            if (!Input.RightGrip)
            {
                if (pointer != null)
                {
                    UnityEngine.GameObject.Destroy(pointer);
                }
            }
        }

        public void ProcessCheckPoint()
        {
            if (Input.RightGrip)
            {
                if (pointer == null)
                {
                    pointer = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                    UnityEngine.Object.Destroy(pointer.GetComponent<Rigidbody>());
                    UnityEngine.Object.Destroy(pointer.GetComponent<SphereCollider>());
                    pointer.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);
                }
                pointer.transform.position = GorillaLocomotion.Player.Instance.rightControllerTransform.position;
            }
            if (!Input.RightGrip && Input.RightTrigger)
            {
                pointer.GetComponent<Renderer>().material.color = Color.green;
                GorillaLocomotion.Player.Instance.transform.position = pointer.transform.position;
            }
            if (!Input.RightTrigger)
            {
                pointer.GetComponent<Renderer>().material.color = Color.red;
            }
        }

        public void MagicMonkey()
        {
            if (Input.RightGrip && !Input.RightTrigger)
            {
                RaycastHit raycastHit;
                if (Physics.Raycast(GorillaLocomotion.Player.Instance.rightControllerTransform.position - GorillaLocomotion.Player.Instance.rightControllerTransform.up, -GorillaLocomotion.Player.Instance.rightControllerTransform.up, out raycastHit, 500, GorillaLocomotion.Player.Instance.locomotionEnabledLayers) && pointer == null)
                {
                    pointer.GetComponent<Renderer>().material.color = Color.red;
                    pointer = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                    UnityEngine.Object.Destroy(pointer.GetComponent<Rigidbody>());
                    UnityEngine.Object.Destroy(pointer.GetComponent<SphereCollider>());
                    pointer.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);
                }
                pointer.transform.position = raycastHit.point;
                
                return;
            }
            if (Input.RightTrigger && Input.RightGrip)
            {
                pointer.GetComponent<Renderer>().material.color = Color.green;
                GorillaLocomotion.Player.Instance.GetComponent<Rigidbody>().isKinematic = true;
                float step = 32f * Time.deltaTime;
                GorillaLocomotion.Player.Instance.transform.position = Vector3.MoveTowards(Camera.main.transform.position, pointer.transform.position, step);
                GorillaLocomotion.Player.Instance.GetComponent<Rigidbody>().isKinematic = false;
                return;
            }
            if (!Input.RightTrigger)
            {
                pointer.GetComponent<Renderer>().material.color = Color.red;
            }
            if (!Input.RightGrip)
            {
                UnityEngine.GameObject.Destroy(pointer);
            }
        }

        public void PlatformNetwork(EventData data)
        {
            if (data.Code == 110)
            {
                object[] customshit = (object[])data.CustomData;
                RightPlat_Networked[data.Sender] = GameObject.CreatePrimitive(PrimitiveType.Cube);
                RightPlat_Networked[data.Sender].transform.position = (Vector3)customshit[0];
                RightPlat_Networked[data.Sender].transform.rotation = (Quaternion)customshit[1];
                RightPlat_Networked[data.Sender].transform.localScale = (Vector3)customshit[2];
                RightPlat_Networked[data.Sender].GetComponent<BoxCollider>().enabled = false;
            }
            if (data.Code == 120)
            {
                object[] customshit = (object[])data.CustomData;
                LeftPlat_Networked[data.Sender] = GameObject.CreatePrimitive(PrimitiveType.Cube);
                LeftPlat_Networked[data.Sender].transform.position = (Vector3)customshit[0];
                LeftPlat_Networked[data.Sender].transform.rotation = (Quaternion)customshit[1];
                LeftPlat_Networked[data.Sender].transform.localScale = (Vector3)customshit[2];
                LeftPlat_Networked[data.Sender].GetComponent<BoxCollider>().enabled = false;
            }
            if (data.Code == 110)
            {
                Destroy(RightPlat_Networked[data.Sender]);
                RightPlat_Networked[data.Sender] = null;
            }
            if (data.Code == 121)
            {
                Destroy(LeftPlat_Networked[data.Sender]);
                LeftPlat_Networked[data.Sender] = null;
            }
        }

        public void Platforms()
        {

            RaiseEventOptions safs = new RaiseEventOptions
            {
                Receivers = ReceiverGroup.Others
            };
            if (Input.RightGrip)
            {
                if (RightPlat == null)
                {
                        RightPlat = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    RightPlat.transform.position = new Vector3(0f, -0.0175f, 0f) + GorillaLocomotion.Player.Instance.rightControllerTransform.position;
                    RightPlat.transform.rotation = GorillaLocomotion.Player.Instance.rightControllerTransform.rotation;
                    RightPlat.transform.localScale = scale;

                        RightPlat.GetComponent<Renderer>().material.color = Color.yellow;
                    PhotonNetwork.RaiseEvent(110, new object[] { RightPlat.transform.position, RightPlat.transform.rotation, scale }, safs, SendOptions.SendReliable);
                }
            }
            else
            {
                if (RightPlat != null)
                {
                    PhotonNetwork.RaiseEvent(111, null, safs, SendOptions.SendReliable);
                    Destroy(RightPlat);
                    RightPlat = null;
                }
            }
            if (Input.LeftGrip)
            {
                if (LeftPlat == null)
                {
                    LeftPlat = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    LeftPlat.transform.localScale = scale;
                    LeftPlat.transform.position = new Vector3(0f, -0.0175f, 0f) + GorillaLocomotion.Player.Instance.leftControllerTransform.position;
                    LeftPlat.transform.rotation = GorillaLocomotion.Player.Instance.leftControllerTransform.rotation;
                        LeftPlat.GetComponent<Renderer>().material.color = Color.red;
                    PhotonNetwork.RaiseEvent(120, new object[] { LeftPlat.transform.position, LeftPlat.transform.rotation, scale }, safs, SendOptions.SendReliable);
                }
            }
            else
            {
                if (LeftPlat != null)
                {
                    PhotonNetwork.RaiseEvent(121, null, safs, SendOptions.SendReliable);
                    Destroy(LeftPlat);
                    LeftPlat = null;
                }
            }
        }

        public void SlowGun()
        {
            if (Input.RightGrip)
            {
                RaycastHit raycastHit;
                if (SlowingRig == null)
                {
                    if (Physics.Raycast(GorillaLocomotion.Player.Instance.rightControllerTransform.position - GorillaLocomotion.Player.Instance.rightControllerTransform.up, -GorillaLocomotion.Player.Instance.rightControllerTransform.up, out raycastHit) && pointer == null)
                    {
                        pointer = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                        UnityEngine.Object.Destroy(pointer.GetComponent<Rigidbody>());
                        UnityEngine.Object.Destroy(pointer.GetComponent<SphereCollider>());
                        pointer.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);
                        pointer.GetComponent<Renderer>().material.color = new Color(1, 0, 0, 0.3f);
                        pointer.GetComponent<Renderer>().material.shader = Shader.Find("GUI/Text Shader");
                    }
                    pointer.transform.position = raycastHit.point;
                    if (Input.RightTrigger)
                    {
                        if (raycastHit.collider.GetComponentInParent<VRRig>())
                        {
                            SlowingRig = raycastHit.collider.GetComponentInParent<VRRig>();
                        }
                    }
                }
                if (Input.RightTrigger && SlowingRig != null)
                {
                    pointer.transform.position = SlowingRig.transform.position;
                    PhotonView Photonview = GetPhotonViewFromRig(SlowingRig);
                    Photon.Realtime.Player owner = Photonview.Owner;
                    if (Photonview != null || owner != null)
                    {
                        if (PhotonNetwork.LocalPlayer.IsMasterClient)
                        {
                            Photonview.RPC("SetTaggedTime", owner);
                        }
                        pointer.GetComponent<Renderer>().material.color = new Color(0, 1, 0, 0.3f);
                    }

                }
                else
                {
                    SlowingRig = null;
                    pointer.GetComponent<Renderer>().material.color = new Color(1, 0, 0, 0.3f);
                }
                return;
            }
            UnityEngine.GameObject.Destroy(pointer);
        }

        public void VibrateGun()
        {
            if (Input.RightGrip)
            {
                RaycastHit raycastHit;
                if (VibratingRig == null)
                {
                    if (Physics.Raycast(GorillaLocomotion.Player.Instance.rightControllerTransform.position - GorillaLocomotion.Player.Instance.rightControllerTransform.up, -GorillaLocomotion.Player.Instance.rightControllerTransform.up, out raycastHit) && pointer == null)
                    {
                        pointer = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                        UnityEngine.Object.Destroy(pointer.GetComponent<Rigidbody>());
                        UnityEngine.Object.Destroy(pointer.GetComponent<SphereCollider>());
                        pointer.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);
                        pointer.GetComponent<Renderer>().material.color = new Color(1, 0, 0, 0.3f);
                        pointer.GetComponent<Renderer>().material.shader = Shader.Find("GUI/Text Shader");
                    }
                    pointer.transform.position = raycastHit.point;
                    if (Input.RightTrigger)
                    {
                        if (raycastHit.collider.GetComponentInParent<VRRig>())
                        {
                            VibratingRig = raycastHit.collider.GetComponentInParent<VRRig>();
                        }
                    }
                }
                if (Input.RightTrigger && VibratingRig != null)
                {
                    pointer.transform.position = VibratingRig.transform.position;
                    PhotonView Photonview = GetPhotonViewFromRig(VibratingRig);
                    Photon.Realtime.Player owner = Photonview.Owner;
                    if (Photonview != null || owner != null)
                    {
                        if (PhotonNetwork.LocalPlayer.IsMasterClient)
                        {
                            Photonview.RPC("SetJoinTaggedTime", owner);
                        }
                        pointer.GetComponent<Renderer>().material.color = new Color(0, 1, 0, 0.3f);
                    }

                }
                else
                {
                    VibratingRig = null;
                    pointer.GetComponent<Renderer>().material.color = new Color(1, 0, 0, 0.3f);
                }
                return;
            }
            UnityEngine.GameObject.Destroy(pointer);
        }
        
        public void KickGun()
        {
            if (Input.RightGrip)
            {
                RaycastHit raycastHit;
                if (KickPlayer == null)
                {
                    if (Physics.Raycast(GorillaLocomotion.Player.Instance.rightControllerTransform.position - GorillaLocomotion.Player.Instance.rightControllerTransform.up, -GorillaLocomotion.Player.Instance.rightControllerTransform.up, out raycastHit) && pointer == null)
                    {
                        pointer = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                        UnityEngine.Object.Destroy(pointer.GetComponent<Rigidbody>());
                        UnityEngine.Object.Destroy(pointer.GetComponent<SphereCollider>());
                        pointer.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);
                        pointer.GetComponent<Renderer>().material.color = new Color(1, 0, 0, 0.3f);
                        pointer.GetComponent<Renderer>().material.shader = Shader.Find("GUI/Text Shader");
                    }
                    pointer.transform.position = raycastHit.point;
                    if (Input.RightTrigger)
                    {
                        if (raycastHit.collider.GetComponentInParent<VRRig>())
                        {
                            KickPlayer = raycastHit.collider.GetComponentInParent<VRRig>();
                        }
                    }
                }
                if (Input.RightTrigger && KickPlayer != null)
                {
                    pointer.GetComponent<Renderer>().material.color = new Color(0, 1, 0, 0.3f);
                    pointer.transform.position = KickPlayer.transform.position;
                    Player owner = GetPhotonViewFromRig(KickPlayer).Owner;
                    if (owner != null && Time.time > kicktimer + 1)
                    {
                        if (GorillaComputer.instance.friendJoinCollider.playerIDsCurrentlyTouching.Contains(PhotonNetwork.LocalPlayer.UserId) && GorillaComputer.instance.friendJoinCollider.playerIDsCurrentlyTouching.Contains(owner.UserId))
                        {
                            PhotonNetworkController.Instance.shuffler = UnityEngine.Random.Range(0, 99999999).ToString().PadLeft(8, '0');
                            PhotonNetworkController.Instance.keyStr = UnityEngine.Random.Range(0, 99999999).ToString().PadLeft(8, '0');
                            GorillaGameManager.instance.photonView.RPC("JoinPubWithFriends", owner, PhotonNetworkController.Instance.shuffler, PhotonNetworkController.Instance.keyStr);
                            kicktimer = Time.time;
                        }
                    }
                }
                else
                {
                    KickPlayer = null;
                    pointer.GetComponent<Renderer>().material.color = new Color(1, 0, 0, 0.3f);
                }
                return;
            }
            UnityEngine.GameObject.Destroy(pointer);
        }

        public void LagGun()
        {
            if (Input.RightGrip)
            {
                RaycastHit raycastHit;
                if (lagrig == null)
                {
                    if (Physics.Raycast(GorillaLocomotion.Player.Instance.rightControllerTransform.position - GorillaLocomotion.Player.Instance.rightControllerTransform.up, -GorillaLocomotion.Player.Instance.rightControllerTransform.up, out raycastHit) && pointer == null)
                    {
                        pointer = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                        UnityEngine.Object.Destroy(pointer.GetComponent<Rigidbody>());
                        UnityEngine.Object.Destroy(pointer.GetComponent<SphereCollider>());
                        pointer.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);
                        pointer.GetComponent<Renderer>().material.color = new Color(1, 0, 0, 0.3f);
                        pointer.GetComponent<Renderer>().material.shader = Shader.Find("GUI/Text Shader");
                    }
                    pointer.transform.position = raycastHit.point;
                    if (Input.RightTrigger)
                    {
                        if (raycastHit.collider.GetComponentInParent<VRRig>())
                        {
                            lagrig = raycastHit.collider.GetComponentInParent<VRRig>();
                        }
                    }
                }
                if (Input.RightTrigger && lagrig != null)
                {
                    pointer.GetComponent<Renderer>().material.color = new Color(0, 1, 0, 0.3f);
                    pointer.transform.position = lagrig.transform.position;
                    Player owner = GetPhotonViewFromRig(lagrig).Owner;
                    if (owner != null)
                    {
                        if (GorillaGameManager.instance != null)
                        {
                            GorillaGameManager.instance.photonView.RPC("LaunchSlingshotProjectile", owner, new object[]
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
                            GorillaGameManager.instance.photonView.RPC("LaunchSlingshotProjectile", owner, new object[]
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
                }
                else
                {
                    lagrig = null;
                    pointer.GetComponent<Renderer>().material.color = new Color(1, 0, 0, 0.3f);
                }
                return;
            }
            Destroy(pointer);
        }

        public void InvisGun()
        {
            if (Input.RightGrip)
            {
                RaycastHit raycastHit;
                if (Physics.Raycast(GorillaLocomotion.Player.Instance.rightControllerTransform.position - GorillaLocomotion.Player.Instance.rightControllerTransform.up, -GorillaLocomotion.Player.Instance.rightControllerTransform.up, out raycastHit) && pointer == null)
                {
                    pointer = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                    UnityEngine.Object.Destroy(pointer.GetComponent<Rigidbody>());
                    UnityEngine.Object.Destroy(pointer.GetComponent<SphereCollider>());
                    pointer.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
                    pointer.GetComponent<Renderer>().material.color = new Color(1, 0, 0, 0.07f);
                }
                pointer.transform.position = raycastHit.point;
                if (Input.RightTrigger)
                {
                    var owner = GetPhotonViewFromRig(raycastHit.collider.GetComponentInParent<VRRig>()).Owner;
                    PhotonNetwork.CurrentRoom.StorePlayer(owner);
                    PhotonNetwork.CurrentRoom.Players.Remove(owner.ActorNumber);
                    PhotonNetwork.OpRemoveCompleteCacheOfPlayer(owner.ActorNumber);
                    pointer.GetComponent<Renderer>().material.color = new Color(0, 1, 0, 0.07f);
                }
                else
                {
                    pointer.GetComponent<Renderer>().material.color = new Color(1, 0, 0, 0.07f);
                }
                return;
            }
            UnityEngine.GameObject.Destroy(pointer);
        }

        private IEnumerator MatAll()
        {
            while (Plugin.buttonsActive[43] == true)
            {
                if (GorillaGameManager.instance != null)
                {
                    if (GorillaGameManager.instance.GameMode().Contains("INFECTION"))
                    {
                        if (GorillaTagManager == null)
                        {
                            GorillaTagManager = GorillaGameManager.instance.gameObject.GetComponent<GorillaTagManager>();
                        }
                    }
                    else if (GorillaGameManager.instance.GameMode().Contains("HUNT"))
                    {
                        if (this.GorillaHuntManager == null)
                        {
                            this.GorillaHuntManager = GorillaGameManager.instance.gameObject.GetComponent<GorillaHuntManager>();
                        }
                    }
                    else if (GorillaGameManager.instance.GameMode().Contains("BATTLE"))
                    {
                        if (GorillaBattleManager == null)
                        {
                            GorillaBattleManager = GorillaGameManager.instance.gameObject.GetComponent<GorillaBattleManager>();
                        }
                    }
                }
                if (GorillaTagManager != null)
                {
                    if (GorillaTagManager.isCurrentlyTag)
                    {
                        foreach (Player owner in PhotonNetwork.PlayerList)
                        {
                            if (GorillaTagManager.currentIt == owner)
                            {
                                GorillaTagManager.currentIt = null;
                                yield return new WaitForSeconds(0.05f);
                            }
                            else
                            {
                                GorillaTagManager.currentIt = owner;
                                yield return new WaitForSeconds(0.05f);
                            }
                        }
                    }
                    else
                    {
                        foreach (Player owner in PhotonNetwork.PlayerList)
                        {
                            if (GorillaTagManager.currentInfected.Contains(owner))
                            {
                                istagged[owner.ActorNumber] = true;
                            }
                            else
                            {
                                istagged[owner.ActorNumber] = false;
                            }
                            if (istagged[owner.ActorNumber])
                            {
                                GorillaTagManager.currentInfected.Remove(owner);
                                GorillaTagManager.UpdateState();
                                yield return new WaitForSeconds(0.05f);
                            }
                            else
                            {
                                GorillaTagManager.currentInfected.Add(owner);
                                GorillaTagManager.UpdateState();
                                yield return new WaitForSeconds(0.05f);
                            }
                        }
                    }
                }
                if (this.GorillaHuntManager != null)
                {
                    foreach (Player owner in PhotonNetwork.PlayerList)
                    {
                        if (this.GorillaHuntManager.currentHunted.Contains(owner))
                        {
                            istagged[owner.ActorNumber] = true;
                        }
                        else
                        {
                            istagged[owner.ActorNumber] = false;
                        }
                        if (istagged[owner.ActorNumber])
                        {
                            this.GorillaHuntManager.currentHunted.Add(owner);
                            this.GorillaHuntManager.UpdateHuntState();
                            yield return new WaitForSeconds(0.1f);
                        }
                        else
                        {
                            this.GorillaHuntManager.currentHunted.Remove(owner);
                            this.GorillaHuntManager.UpdateHuntState();
                            yield return new WaitForSeconds(0.1f);
                        }
                    }
                }
                if (GorillaBattleManager != null)
                {
                    foreach (Player owner in PhotonNetwork.PlayerList)
                    {
                        if (GorillaBattleManager.playerLives[owner.ActorNumber] == 3)
                        {
                            istagged[owner.ActorNumber] = true;
                        }
                        else
                        {
                            istagged[owner.ActorNumber] = false;
                        }
                        if (istagged[owner.ActorNumber])
                        {
                            GorillaBattleManager.playerLives[owner.ActorNumber] = 0;
                            yield return new WaitForSeconds(0.05f);
                        }
                        else
                        {
                            GorillaBattleManager.playerLives[owner.ActorNumber] = 3;
                            yield return new WaitForSeconds(0.05f);
                        }
                    }
                }
            }
            yield break;
        }
        
        private IEnumerator MatSpamSelf()
        {
            while (Plugin.buttonsActive[37] == true)
            {
                Photon.Realtime.Player owner = PhotonNetwork.LocalPlayer;
                if (GorillaGameManager.instance != null)
                {
                    if (GorillaGameManager.instance.GameMode().Contains("INFECTION"))
                    {
                        if (GorillaTagManager == null)
                        {
                            GorillaTagManager = GorillaGameManager.instance.gameObject.GetComponent<GorillaTagManager>();
                        }
                    }
                    else if (GorillaGameManager.instance.GameMode().Contains("HUNT"))
                    {
                        if (this.GorillaHuntManager == null)
                        {
                            this.GorillaHuntManager = GorillaGameManager.instance.gameObject.GetComponent<GorillaHuntManager>();
                        }
                    }
                    else if (GorillaGameManager.instance.GameMode().Contains("BATTLE"))
                    {
                        if (GorillaBattleManager == null)
                        {
                            GorillaBattleManager = GorillaGameManager.instance.gameObject.GetComponent<GorillaBattleManager>();
                        }
                    }
                }
                if (GorillaTagManager != null)
                {
                    if (GorillaTagManager.isCurrentlyTag)
                    {
                        if (GorillaTagManager.currentIt == owner)
                        {
                            GorillaTagManager.currentIt = null;
                            yield return new WaitForSeconds(0.1f);
                        }
                        else
                        {
                            GorillaTagManager.currentIt = owner;
                            yield return new WaitForSeconds(0.1f);
                        }
                    }
                    else
                    {
                        if (GorillaTagManager.currentInfected.Contains(owner))
                        {
                            IsTaggedSelf = true;
                        }
                        else
                        {
                            IsTaggedSelf = false;
                        }
                        if (IsTaggedSelf)
                        {
                            GorillaTagManager.currentInfected.Remove(owner);
                            GorillaTagManager.UpdateState();
                            yield return new WaitForSeconds(0.1f);
                        }
                        else
                        {
                            GorillaTagManager.currentInfected.Add(owner);
                            GorillaTagManager.UpdateState();
                            yield return new WaitForSeconds(0.1f);
                        }
                    }
                }
                if (this.GorillaHuntManager != null)
                {
                    if (this.GorillaHuntManager.currentHunted.Contains(owner))
                    {
                        IsTaggedSelf = true;
                    }
                    else
                    {
                        IsTaggedSelf = false;
                    }
                    if (IsTaggedSelf)
                    {
                        this.GorillaHuntManager.currentHunted.Add(owner);
                        this.GorillaHuntManager.UpdateHuntState();
                        yield return new WaitForSeconds(0.1f);
                    }
                    else
                    {
                        this.GorillaHuntManager.currentHunted.Remove(owner);
                        this.GorillaHuntManager.UpdateHuntState();
                        yield return new WaitForSeconds(0.1f);
                    }
                }
                if (GorillaBattleManager != null)
                {
                    if (GorillaBattleManager.playerLives[owner.ActorNumber] == 3)
                    {
                        IsTaggedSelf = true;
                    }
                    else
                    {
                        IsTaggedSelf = false;
                    }
                    if (IsTaggedSelf)
                    {
                        GorillaBattleManager.playerLives[owner.ActorNumber] = 0;
                        yield return new WaitForSeconds(0.1f);
                    }
                    else
                    {
                        GorillaBattleManager.playerLives[owner.ActorNumber] = 3;
                        yield return new WaitForSeconds(0.1f);
                    }
                }
            }
            yield break;
        }

        public void ProcessTagAura(Photon.Realtime.Player pl)
        {
            if (Time.time > TagAura + 0.1)
            {
                float distance = Vector3.Distance(GorillaTagger.Instance.offlineVRRig.transform.position, GorillaGameManager.instance.FindPlayerVRRig(pl).transform.position);
                if (distance < GorillaGameManager.instance.tagDistanceThreshold)
                {
                    PhotonView.Get(GorillaGameManager.instance.GetComponent<GorillaGameManager>()).RPC("ReportTagRPC", RpcTarget.MasterClient, new object[]
                    {
                                pl
                    });
                }

                TagAura = Time.time;
            }
        }

        public void TagAll()
        {
            if (GorillaGameManager.instance != null)
            {
                if (GorillaGameManager.instance.GameMode().Contains("INFECTION"))
                {
                    if (GorillaTagManager == null)
                    {
                        GorillaTagManager = GorillaGameManager.instance.gameObject.GetComponent<GorillaTagManager>();
                    }
                }
                else if (GorillaGameManager.instance.GameMode().Contains("HUNT"))
                {
                    if (this.GorillaHuntManager == null)
                    {
                        this.GorillaHuntManager = GorillaGameManager.instance.gameObject.GetComponent<GorillaHuntManager>();
                    }
                }
                else if (GorillaGameManager.instance.GameMode().Contains("BATTLE"))
                {
                    if (GorillaBattleManager == null)
                    {
                        GorillaBattleManager = GorillaGameManager.instance.gameObject.GetComponent<GorillaBattleManager>();
                    }
                }
            }
            foreach (Photon.Realtime.Player p in PhotonNetwork.PlayerListOthers)
            {
                if (!GorillaTagManager.currentInfected.Contains(p))
                {
                    GorillaTagger.Instance.offlineVRRig.transform.position = GorillaGameManager.instance.FindPlayerVRRig(p).transform.position;
                    ProcessTagAura(p);
                    return;
                }
            }
        }

        public void TagGun()
        {
            if (Input.RightGrip)
            {
                RaycastHit raycastHit;
                if (Tagger == null)
                {
                    if (Physics.Raycast(GorillaLocomotion.Player.Instance.rightControllerTransform.position - GorillaLocomotion.Player.Instance.rightControllerTransform.up, -GorillaLocomotion.Player.Instance.rightControllerTransform.up, out raycastHit) && pointer == null)
                    {
                        pointer = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                        UnityEngine.Object.Destroy(pointer.GetComponent<Rigidbody>());
                        UnityEngine.Object.Destroy(pointer.GetComponent<SphereCollider>());
                        pointer.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);
                        pointer.GetComponent<Renderer>().material.color = new Color(1, 0, 0, 0.15f);
                        pointer.GetComponent<Renderer>().material.shader = Shader.Find("GUI/Text Shader");
                    }
                    pointer.transform.position = raycastHit.point;
                    if (Input.RightTrigger)
                    {
                        if (raycastHit.collider.GetComponentInParent<VRRig>())
                        {
                            Tagger = raycastHit.collider.GetComponentInParent<VRRig>();
                        }
                    }
                }
                if (Input.RightTrigger && Tagger != null)
                {
                    pointer.transform.position = Tagger.transform.position;
                    pointer.GetComponent<Renderer>().material.color = new Color(0, 1, 0, 0.15f);
                    if (GorillaGameManager.instance.GameMode().Contains("INFECTION"))
                    {
                        GorillaTagger.Instance.offlineVRRig.transform.position = Tagger.transform.position;
                        ProcessTagAura(GetPhotonViewFromRig(Tagger).Owner);
                    }
                    return;
                }
                else
                {
                    pointer.GetComponent<Renderer>().material.color = new Color(1, 0, 0, 0.15f);
                    GorillaTagger.Instance.offlineVRRig.enabled = true;
                    Tagger = null;
                }
                return;
            }
        }

        public void Tracers()
        {
            foreach (VRRig rig in GorillaParent.instance.vrrigs)
            {
                if (rig != null && !rig.isOfflineVRRig && !rig.isMyPlayer)
                {
                    var gameobject = new GameObject("Line");
                    lineRenderer = gameobject.AddComponent<LineRenderer>();
                    lineRenderer.startColor = KmanUI.Espcolor;
                    lineRenderer.endColor = KmanUI.Espcolor;
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
                if (rig != null && !rig.isOfflineVRRig && !rig.isMyPlayer)
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

                    top.GetComponent<Renderer>().material.color = KmanUI.Espcolor;
                    bottom.GetComponent<Renderer>().material.color = KmanUI.Espcolor;
                    left.GetComponent<Renderer>().material.color = KmanUI.Espcolor;
                    right.GetComponent<Renderer>().material.color = KmanUI.Espcolor;

                    go.transform.LookAt(go.transform.position + Camera.main.transform.rotation * Vector3.forward, Camera.main.transform.rotation * Vector3.up);
                    Destroy(go, Time.deltaTime);
                }
            }
        }

        public void AdvancedWASD()
        {
            GorillaTagger.Instance.rigidbody.velocity = new Vector3(0, 0.0735f, 0);
            float NSpeed = KmanUI.speed * Time.deltaTime;
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
    }
}
