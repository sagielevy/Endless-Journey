using Assets.Scripts.CFGParser.DataHolder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Assets.Scripts.CFGParser;

namespace Assets.Scripts.CFGParser.Modifiers
{
    public class SkyModifier : IWorldModifier//<ISkyData>
    {
        const string SkyColorId = "_SkyColor";
        const string HorizonColorId = "_HorizonColor";
        const float FogDarknessFactor = 1.15f; // 10% darker
        Material skyMaterial;
        GameObject lights;
        Color orgSkyColor, orgHorizonColor;
        float startTime;
        

        public SkyModifier(Material skyMaterial, GameObject lights)
        {
            this.skyMaterial = skyMaterial;
            this.lights = lights;
            startTime = Time.time;
            orgSkyColor = skyMaterial.GetColor(SkyColorId);
            orgHorizonColor = skyMaterial.GetColor(HorizonColorId);
        }

        public void ModifySection(ISentenceData data)
        {
            var skyData = data as ISkyData;

            // Interpolate colors!
            // Interpolate Environment settings & directional lights as well
            var newSkyColor = Color.Lerp(orgSkyColor, Helpers.FromText(skyData.ColorSky()), Globals.speedChange * (Time.time - startTime));
            skyMaterial.SetColor(SkyColorId, newSkyColor);
            RenderSettings.ambientSkyColor = newSkyColor;

            // Darken color
            float h, s, v;
            Color.RGBToHSV(newSkyColor, out h, out s, out v);
            v = Mathf.Clamp01(v * FogDarknessFactor);

            RenderSettings.fogColor = Color.HSVToRGB(h, s, v); // TODO Color a bit darker than the actual sky color!

            foreach (var light in lights.GetComponentsInChildren<Light>())
            {
                // Just a touch of the new light color
                light.color = Color.Lerp(Color.white, newSkyColor, 0.1f);
            }

            // Change horizon to new horizon color
            if (skyData.IsSkyGradient())
            {
                var newHorizonColor = Color.Lerp(orgHorizonColor, Helpers.FromText(skyData.ColorHorizon()), Globals.speedChange * (Time.time - startTime));
                RenderSettings.ambientEquatorColor = newHorizonColor;
                skyMaterial.SetColor(HorizonColorId, newHorizonColor);
            } else
            {
                var newHorizonColor = Color.Lerp(orgHorizonColor, Helpers.FromText(skyData.ColorSky()), Globals.speedChange * (Time.time - startTime));
                
                // Change horizon to sky color
                RenderSettings.ambientEquatorColor = newHorizonColor;
                skyMaterial.SetColor(HorizonColorId, newHorizonColor);
            }

            // Change ground color to match horizon
            RenderSettings.ambientGroundColor = RenderSettings.ambientEquatorColor;
        }
    }
}
