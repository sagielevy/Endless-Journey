using Assets.Scripts.CFGParser.DataHolder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Scripts.CFGParser.Modifiers
{
    class MusicModifier : IWorldModifier//<IMusicData>
    {
        public void ModifySection(ISentenceData data)
        {
            var musicData = data as IMusicData;
            throw new NotImplementedException();
        }
    }
}
