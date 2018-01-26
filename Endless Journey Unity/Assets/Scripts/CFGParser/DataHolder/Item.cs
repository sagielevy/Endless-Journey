using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Scripts.CFGParser.DataHolder
{
    public struct Item
    {
        public Item(float x, float z, int type)
        {
            pos_x_percent = x;
            pos_z_percent = z;
            subtypeIndex = type;
        }

        public float pos_x_percent, pos_z_percent;
        public int subtypeIndex;
    }
}
