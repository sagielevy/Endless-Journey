using Assets.Scripts.CFGParser.DataHolder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Scripts.CFGParser.Modifiers
{
    public class GroundModifier : IWorldModifier//<IGroundData>
    {
        public void ModifySection(ISentenceData data)
        {
            var groundData = data as IGroundData;

            throw new NotImplementedException();
        }
    }
}
