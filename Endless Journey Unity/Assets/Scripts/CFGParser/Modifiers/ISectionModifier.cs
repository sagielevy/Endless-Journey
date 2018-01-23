using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Scripts.CFGParser.Modifiers
{
    public interface ISectionModifier
    {
        // TODO Think of how to pass data to the modifier so it'll modify according to it
        void ModifySection();
    }
}
