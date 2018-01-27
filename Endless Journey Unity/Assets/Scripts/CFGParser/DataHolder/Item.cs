using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Scripts.CFGParser.DataHolder
{
    public struct Item
    {
        public Item(float x, float z, float scale, int type)
        {
            pos_x_percent = x;
            pos_z_percent = z;
            scale_mul = scale;
            subtypeIndex = type;
        }

        public float pos_x_percent, pos_z_percent, scale_mul;
        public int subtypeIndex;
    }
}
