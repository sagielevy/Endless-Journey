using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts.CFGParser
{
    public static class Extensions
    {
        public static Color FromText(string text)
        {
            return new Color(int.Parse(text.Substring(0, 2), NumberStyles.HexNumber) / 255.0f, 
                int.Parse(text.Substring(2, 2), NumberStyles.HexNumber) / 255.0f, int.Parse(text.Substring(4, 2), NumberStyles.HexNumber) / 255.0f);
        }
    }
}
