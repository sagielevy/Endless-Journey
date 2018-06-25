using Assets.Scripts.CFGParser.DataHolder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts.CFGParser.Modifiers
{
    public class GroundModifier : IWorldModifier//<IGroundData>
    {
        const string Glossiness = "_Glossiness";
        const string Metallic = "_Metallic";
        const int MaxLayers = 3;
        private TextureData textureData;
        private Material groundMaterial;
        //const float speed = 0.08f;
        Color[] orgLayerColors;
        float orgGlosiness, orgMetallic;
        float startTime;

        public GroundModifier(TextureData textureData, Material groundMaterial)
        {
            this.textureData = textureData;
            this.groundMaterial = groundMaterial;
            startTime = Time.time;

            orgLayerColors = new Color[MaxLayers];

            for (int i = 0; i < MaxLayers; i++)
            {
                orgLayerColors[i] = textureData.layers[i].tint;
            }

            orgGlosiness = groundMaterial.GetFloat(Glossiness);
            orgMetallic = groundMaterial.GetFloat(Metallic);
        }

        public IEnumerator<WaitForEndOfFrame> ModifySection(ISentenceData data)
        {
            var groundData = data as IGroundData;
            var groundColors = groundData.GroundColors();

            // Keep runing till replaced by new enumerator
            while (true)
            {
                // Change each layer color
                for (int i = 0; i < MaxLayers; i++)
                {
                    var currColor = Color.Lerp(orgLayerColors[i], Helpers.FromText(groundColors[i]), Globals.speedChange * (Time.time - startTime));
                    textureData.layers[i].tint = currColor;
                }

                // Apply color changes
                textureData.ApplyToMaterial(groundMaterial);

                // Change glossiness and metallic
                groundMaterial.SetFloat(Glossiness, Mathf.Lerp(orgGlosiness, groundData.Smoothness(), Globals.speedChange * (Time.time - startTime)));
                groundMaterial.SetFloat(Glossiness, Mathf.Lerp(orgMetallic, groundData.Metallic(), Globals.speedChange * (Time.time - startTime)));

                yield return Globals.EndOfFrame;
            }
        }
    }
}
