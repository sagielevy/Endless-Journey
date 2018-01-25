using Assets.Scripts.CFGParser.DataHolder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts.CFGParser.Modifiers
{
    public interface ISectionModifier<T> where T : ISentenceData
    {
        // TODO Think of how to pass data to the modifier so it'll modify according to it - both CFG sentence and
        // Unity objects to modify!
        void ModifySection(T data);
    }
}
