using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Scripts.CFGParser.DataHolder
{
    public interface ISkyData : ISentenceData
    {
        string ColorSky();
        string ColorHorizon();

        bool IsSkyGradient();
    }
}
