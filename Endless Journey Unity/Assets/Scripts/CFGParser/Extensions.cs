using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts.CFGParser
{
    public static class Extensions
    {
        public static Color FromText(string text)
        {
            return new Color(float.Parse(text.Substring(0, 2)) / 255, 
                float.Parse(text.Substring(2, 2)) / 255, float.Parse(text.Substring(4, 2)) / 255);
        }
    }
}
