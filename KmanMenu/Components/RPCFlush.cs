using Photon.Pun;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace KmanMenu.Components
{
    internal class RPCFlush : MonoBehaviour
    {
        float FlushTimeout;

        public void Update()
        {
            if (PhotonNetwork.InRoom)
            {
                if (Time.time > FlushTimeout + 10)
                {
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
