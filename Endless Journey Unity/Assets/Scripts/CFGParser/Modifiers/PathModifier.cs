using Assets.Scripts.CFGParser.DataHolder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Scripts.CFGParser.Modifiers
{
    class PathModifier : IWorldModifier//<IPathData>
    {
        public void ModifySection(ISentenceData data)
        {
            var pathData = data as IPathData;
            throw new NotImplementedException();
        }
    }
}
