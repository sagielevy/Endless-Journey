using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Scripts.CFGParser.DataHolder
{
    public interface IGroundData : ISentenceData
    {
        float Metallic();
        float Smoothness();

        string GroundColor1();
        string GroundColor2();
        string GroundColor3();
        string[] GroundColors();
    }
}
