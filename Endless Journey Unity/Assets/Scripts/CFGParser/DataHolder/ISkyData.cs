using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Scripts.CFGParser.DataHolder
{
    public interface ISkyData : ISectionData
    {
        string ColorSky();
        string ColorHorizon();
    }
}
