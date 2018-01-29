using Assets.Scripts.CFGParser.DataHolder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts.CFGParser.Modifiers
{
    public class PlantsModifier : IWorldModifier//<IPlantsData>
    {
        private ISectionData sectionData;
        private GameObject originalModels;
        private TerrainGenerator terrainChunksParent;
        private Vector3 origin;
        private bool hasRun;

        public PlantsModifier(ISectionData sectionData, GameObject originalModels,
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
            if (!hasRun) {
                hasRun = true;
                System.Random random = new System.Random();
                var plantData = data as IPlantsData;
                float actualPosX, actualPosZ;

                int[] maxes = new int[] { 4, 8, 4 };

                foreach (var plant in plantData.Plants())
                {
                    // Origin center of section
                    actualPosX = origin.x + (plant.pos_x_percent * sectionData.SectionLength() * Globals.groundSeperateMul);
                    actualPosZ = origin.z + (plant.pos_z_percent * sectionData.SectionLength() * Globals.groundSeperateMul);

                    // FOR DEBUG
                    int subSubType = random.Next(1, maxes[plant.subtypeIndex - 1]);
                    var plantName = "Plants_" + plant.subtypeIndex + "_" + subSubType;
                    var newPlant = GameObject.Instantiate(originalModels.transform.Find(plantName));

                    // Uniform scale
                    newPlant.localScale *= plant.scale_mul;

                    // Set chunk parent
                    var chunk = Helpers.FindClosestTerrain(terrainChunksParent, new Vector2(actualPosX, actualPosZ));
                    chunk.AddItem(newPlant);

                    // Save original actual pos
                    newPlant.GetComponent<GroundItemComponent>().ActualOriginalPos = new Vector2(actualPosX, actualPosZ);

                    // Set current item position to X, Z
                    newPlant.position = new Vector3(actualPosX, Globals.maxHeight, actualPosZ);
                }
            }
        }
    }
}
