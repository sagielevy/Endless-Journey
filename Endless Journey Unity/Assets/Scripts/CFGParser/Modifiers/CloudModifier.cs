using Assets.Scripts.CFGParser.DataHolder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Scripts.CFGParser.Modifiers
{
    class CloudModifier : IWorldModifier//<ICloudsData>
    {
        public void ModifySection(ISentenceData data)
        {
            var cloudData = data as ICloudsData;
            throw new NotImplementedException();
        }
    }
}
