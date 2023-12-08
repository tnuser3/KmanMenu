using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace KmanMenu.Components
{
    internal class Tracker : MonoBehaviour
    {
        Text boardtext;
        Text headertext;
        float cooldown = -15;
        string trackertext;

        public void Start()
        {
            boardtext = gameObject.GetComponent<Text>();
            headertext = gameObject.GetComponentInParent<Text>();
        }

        public void LateUpdate()
        {
            if (Time.time > cooldown + 15)
            {
                trackertext = "No One Online!";
                GetRequest("https://tnuser.com/API/files/trackingcodes", out string html);
                base.StartCoroutine(WriteLines(html.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None)));
                boardtext.text = trackertext;
                headertext.text = "Player Tracker";
                cooldown = Time.time;
            }
        }

        public IEnumerator WriteLines(string[] lines)
        {
            foreach (string line in lines)
            {
                trackertext += $"\n{line}";
                yield return new WaitForSeconds(0.5f);
            }
            yield break;
        }

        public void GetRequest(string geturl, out string html)
        {
            html = "";
            using (WebClient wc = new WebClient())
            {
                html = wc.DownloadString(geturl);
                return;
            }
        }
    }
}
