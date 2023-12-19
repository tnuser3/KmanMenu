using Photon.Pun;
using UnityEngine;

namespace KmanMenu.Components
{
    internal class RPCFlush : MonoBehaviour
    {
        float FlushTimeout;

        public void LateUpdate()
        {
            if (PhotonNetwork.InRoom)
            {
                if (Time.time > FlushTimeout + 1)
                {
                    PhotonNetwork.NetworkingClient.LoadBalancingPeer.SendOutgoingCommands();
                    PhotonNetwork.OpCleanActorRpcBuffer(PhotonNetwork.LocalPlayer.ActorNumber);
                    PhotonNetwork.MaxResendsBeforeDisconnect = int.MaxValue;
                    PhotonNetwork.SendAllOutgoingCommands();
                    PhotonNetwork.RemoveRPCs(PhotonNetwork.LocalPlayer);
                    PhotonNetwork.RemoveBufferedRPCs();
                    if (GorillaTagger.Instance.myVRRig != null)
                    {
                        PhotonNetwork.OpCleanRpcBuffer(GorillaTagger.Instance.myVRRig);
                    }
                    Plugin.debug.LogMessage("Flushed RPCS");
                    FlushTimeout = Time.time;
                }
            }
        }
    }
}
