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

        public CloudModifier(ISectionData sectionData, GameObject originalModels,
                              TerrainGenerator terrainChunksParent)
        {
            this.sectionData = sectionData;
            this.originalModels = originalModels;
            this.terrainChunksParent = terrainChunksParent;
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
                    actualPosX = cloud.pos_x_percent * sectionData.SectionLength() * Globals.cloudSeperateMul / 2;
                    actualPosZ = cloud.pos_z_percent * sectionData.SectionLength() * Globals.cloudSeperateMul / 2;

                    // FOR DEBUG
                    int subSubType = random.Next(1, maxes[cloud.subtypeIndex - 1]);
                    var plantName = "Clouds_" + cloud.subtypeIndex + "_" + subSubType;
                    var newPlant = GameObject.Instantiate(originalModels.transform.Find(plantName));

                    // Set chunk parent
                    var chunk = Helpers.FindClosestTerrain(terrainChunksParent, new Vector2(actualPosX, actualPosZ));
                    chunk.AddItem(newPlant);

                    // Set current item position to X, Y = 100, Z
                    newPlant.position = new Vector3(actualPosX, Globals.cloudHeight, actualPosZ);
                }
            }
        }
    }
}
