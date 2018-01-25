using Assets.Scripts.CFGParser.DataHolder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Scripts.CFGParser.Modifiers
{
    public class SkyModifier : ISectionModifier<ISkyData>
    {
        // TODO When modifying, change both the values of the skybox material AND the environment lighting gradient colors!
        public void ModifySection(ISkyData data)
        {
            throw new NotImplementedException();
        }
    }
}
