using KmanMenu.Helpers.Notifacations;
using KmanMenu.Helpers;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using KmanMenu;

namespace KmanMenu.Components
{
    internal class BtnCollider : MonoBehaviour
    {
        void OnTriggerEnter(Collider collider)
        {
            if (Time.frameCount >= Main.framePressCooldown + 10 && collider.gameObject.name == "MenuClicker")
            {
                AssetLoader.Instance.PlayClick();
                GorillaTagger.Instance.StartVibration(false, GorillaTagger.Instance.tagHapticStrength / 2, GorillaTagger.Instance.tagHapticDuration / 2);
                Main.Toggle(this.relatedText);
                Notif.ClearPastNotifications(100);
                Main.framePressCooldown = Time.frameCount;
            }
        }
        public string relatedText;
    }
}
