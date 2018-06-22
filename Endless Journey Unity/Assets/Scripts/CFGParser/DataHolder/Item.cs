using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Scripts.CFGParser.DataHolder
{
    // Has to be a class and not a struct to be able to access fields via reflections
    public class Item
    {
        public float pos_x_percent, pos_z_percent, scale_mul, angle, height;
        public int subtypeIndex;

        public Item(float x, float z, float scale, int type)
        {
            pos_x_percent = x;
            pos_z_percent = z;
            scale_mul = scale;
            angle = -1;
            height = -1;
            subtypeIndex = type;
        }
    }
}
