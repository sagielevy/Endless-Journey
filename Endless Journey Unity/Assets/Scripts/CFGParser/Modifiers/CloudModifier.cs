using Assets.Scripts.CFGParser.DataHolder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts.CFGParser.Modifiers
{
    public class CloudModifier : IWorldModifier//<ICloudsData>
    {
        private ISectionData sectionData;
        private GameObject originalModels;
        private TerrainGenerator terrainChunksParent;
        private bool hasRun;
        private Vector3 origin;

        public CloudModifier(ISectionData sectionData, GameObject originalModels,
                              TerrainGenerator terrainChunksParent, Vector3 origin)
        {
            this.sectionData = sectionData;
            this.originalModels = originalModels;
            this.terrainChunksParent = terrainChunksParent;
            this.origin = origin;
            hasRun = false;
        }

        public void ModifySection(ISentenceData data)
        {
            if (!hasRun)
            {
                hasRun = true;
                System.Random random = new System.Random();
                var cloudData = data as ICloudsData;
                float actualPosX, actualPosZ;

                int[] maxes = new int[] { 3, 9 };

                foreach (var cloud in cloudData.Clouds())
                {
                    // Origin center of section
                    actualPosX = origin.x + (cloud.pos_x_percent * sectionData.SectionLength() * Globals.cloudSeperateMul);
                    actualPosZ = origin.z + (cloud.pos_z_percent * sectionData.SectionLength() * Globals.cloudSeperateMul);

                    // FOR DEBUG
                    int subSubType = random.Next(1, maxes[cloud.subtypeIndex - 1]);
                    var plantName = "Clouds_" + cloud.subtypeIndex + "_" + subSubType;
                    var newCloud = GameObject.Instantiate(originalModels.transform.Find(plantName));

                    // Uniform scale
                    newCloud.localScale *= cloud.scale_mul;

                    // Set chunk parent
                    var chunk = Helpers.FindClosestTerrain(terrainChunksParent, new Vector2(actualPosX, actualPosZ));
                    chunk.AddItem(newCloud);

                    // Set current item position to X, Y = cloud height, Z
                    newCloud.position = new Vector3(actualPosX, Globals.cloudHeight, actualPosZ);
                }
            }
        }
    }
}
