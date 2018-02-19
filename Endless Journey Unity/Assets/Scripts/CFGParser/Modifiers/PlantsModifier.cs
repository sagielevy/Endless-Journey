using Assets.Scripts.CFGParser.DataHolder;
using EZObjectPools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts.CFGParser.Modifiers
{
    public class PlantsModifier : PoolRequester, IWorldModifier//<IPlantsData>
    {
        private ISectionData sectionData;
        private EZObjectPool[] pools;
        private TerrainGenerator terrainChunksParent;
        private Vector3 origin;
        private bool hasRun;

        public PlantsModifier(ISectionData sectionData, EZObjectPool[] pools,
                              TerrainGenerator terrainChunksParent, Vector3 origin) :
            base("Plants_", pools)
        {
            this.sectionData = sectionData;
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
                    //var newPlant = GameObject.Instantiate(pools.transform.Find(plantName));
                    GameObject newPlant;

                    if (poolsDict[plantName].TryGetNextObject(new Vector3(), Globals.defaultRotation, out newPlant))
                    {
                        newPlant.GetComponent<ItemComponent>().SetOrgLocalScale(newPlant.transform.localScale);

                        // Uniform scale
                        newPlant.transform.localScale *= plant.scale_mul;

                        // Set chunk parent
                        var chunk = Helpers.FindClosestTerrain(terrainChunksParent, new Vector2(actualPosX, actualPosZ));
                        chunk.AddItem(newPlant.GetComponent<ItemComponent>());

                        // Save original actual pos
                        newPlant.GetComponent<GroundItemComponent>().ActualOriginalPos = new Vector2(actualPosX, actualPosZ);

                        // Set inital pos!
                        chunk.PositionSingleGroundItem(newPlant.GetComponent<GroundItemComponent>());
                    }
                }
            }
        }
    }
}
