using Assets.Scripts.CFGParser.DataHolder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Assets.Scripts.CFGParser;

namespace Assets.Scripts.CFGParser.Modifiers
{
    public class SkyModifier : IWorldModifier<ISkyData>
    {
        const string SkyColorId = "_SkyColor";
        const string HorizonColorId = "_HorizonColor";
        Material skyMaterial;
        Color orgSkyColor, orgHorizonColor;
        float startTime;

        public SkyModifier(Material skyMaterial)
        {
            this.skyMaterial = skyMaterial;
            startTime = Time.time;
            orgSkyColor = skyMaterial.GetColor(SkyColorId);
            orgHorizonColor = skyMaterial.GetColor(HorizonColorId);
        }

        // TODO When modifying, change both the values of the skybox material AND the environment lighting gradient colors!
        public void ModifySection(ISkyData data)
        {
            // Interpolate colors!
            // Interpolate Environment settings as well
            var newSkyColor = Color.Lerp(orgSkyColor, Extensions.FromText(data.ColorSky()), Time.time - startTime);
            skyMaterial.SetColor(SkyColorId, newSkyColor);
            RenderSettings.ambientSkyColor = newSkyColor;

            // Change horizon to new horizon color
            if (data.IsSkyGradient())
            {
                var newHorizonColor = Color.Lerp(orgHorizonColor, Extensions.FromText(data.ColorHorizon()), Time.time - startTime);
                RenderSettings.ambientEquatorColor = newHorizonColor;
                skyMaterial.SetColor(HorizonColorId, newHorizonColor);
            } else
            {
                var newHorizonColor = Color.Lerp(orgHorizonColor, Extensions.FromText(data.ColorSky()), Time.time - startTime);
                
                // Change horizon to sky color
                RenderSettings.ambientEquatorColor = newHorizonColor;
                skyMaterial.SetColor(HorizonColorId, newHorizonColor);
            }
        }
    }
}
