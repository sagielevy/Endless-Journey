﻿using Assets.Scripts.CFGParser.DataHolder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Scripts.CFGParser.Modifiers
{
    class PlantsModifier : IWorldModifier//<IPlantsData>
    {
        public void ModifySection(ISentenceData data)
        {
            var plantData = data as IPlantsData;
            throw new NotImplementedException();
        }
    }
}