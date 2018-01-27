using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts.CFGParser.DataHolder
{
    public interface ISectionData : ISentenceData
    {
        int SectionLength();
        Vector2[] SectionAngles();
    }
}
