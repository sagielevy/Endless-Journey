using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts.Control
{
    public class RotationIgnore : MonoBehaviour
    {
        Quaternion rotation;

        void Awake()
        {
            rotation = transform.rotation;
        }
        void LateUpdate()
        {
            // Keep original rotation
            transform.rotation = rotation;
        }
    }
}
