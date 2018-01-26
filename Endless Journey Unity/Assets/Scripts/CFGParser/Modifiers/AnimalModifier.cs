using Assets.Scripts.CFGParser.DataHolder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Scripts.CFGParser.Modifiers
{
    class AnimalModifier : IWorldModifier//<IAnimalsData>
    {
        public void ModifySection(ISentenceData data)
        {
            var animalData = data as IAnimalsData;
            throw new NotImplementedException();
        }
    }
}
