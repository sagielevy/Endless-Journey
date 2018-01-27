using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Scripts.CFGParser.DataHolder
{
    public struct Item
    {
        public float pos_x_percent, pos_z_percent, scale_mul, angle;
        public int subtypeIndex;

        public Item(float x, float z, float scale, float angle, int type)
        {
            pos_x_percent = x;
            pos_z_percent = z;
            scale_mul = scale;
            this.angle = angle;
            subtypeIndex = type;
        }

        public Item(float x, float z, float scale, int type)
        {
            pos_x_percent = x;
            pos_z_percent = z;
            scale_mul = scale;
            angle = -1;
            subtypeIndex = type;
        }
    }
}
