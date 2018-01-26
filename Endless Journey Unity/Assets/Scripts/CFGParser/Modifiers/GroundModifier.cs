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
        const int MaxLayers = 3;
        private TextureData textureData;
        private Material groundMaterial;
        const float speed = 0.08f;
        Color[] orgLayerColors;
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
        }

        public void ModifySection(ISentenceData data)
        {
            var groundData = data as IGroundData;
            var groundColors = groundData.GroundColors();

            // Change each layer color
            for (int i = 0; i < MaxLayers; i++)
            {
                var currColor = Color.Lerp(orgLayerColors[i], Extensions.FromText(groundColors[i]), speed * (Time.time - startTime));
                textureData.layers[i].tint = currColor;
            }

            // Apply changes
            textureData.ApplyToMaterial(groundMaterial);
        }
    }
}
