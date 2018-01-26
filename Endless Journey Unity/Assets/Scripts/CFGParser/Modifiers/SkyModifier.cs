using Assets.Scripts.CFGParser.DataHolder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts.CFGParser.Modifiers
{
    public class SkyModifier : IWorldModifier<ISkyData>
    {
        Material skyMaterial;
        Color orgSkyColor, orgHorizonColor;
        float startTime;

        public SkyModifier(Material skyMaterial)
        {
            this.skyMaterial = skyMaterial;
            startTime = Time.time;
            orgSkyColor = skyMaterial.GetColor("_SkyColor");
            orgHorizonColor = skyMaterial.GetColor("_HorizonColor");
        }

        // TODO When modifying, change both the values of the skybox material AND the environment lighting gradient colors!
        public void ModifySection(ISkyData data)
        {
            throw new NotImplementedException();
        }
    }
}
