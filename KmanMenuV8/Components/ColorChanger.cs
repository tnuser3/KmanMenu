using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace KmanMenuV8.Components
{
    internal class ColorChanger : MonoBehaviour
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
