﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Scripts.CFGParser.DataHolder
{
    public interface IPathData : ISentenceData
    {
        float BloomIntensity();
        float BloomThreshold();
    }
}
