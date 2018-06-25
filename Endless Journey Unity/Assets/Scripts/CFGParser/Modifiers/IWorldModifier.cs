using Assets.Scripts.CFGParser.DataHolder;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts.CFGParser.Modifiers
{
    public interface IWorldModifier//<T> where T : ISentenceData
    {
        IEnumerator<WaitForEndOfFrame> ModifySection(ISentenceData data);
    }
}
