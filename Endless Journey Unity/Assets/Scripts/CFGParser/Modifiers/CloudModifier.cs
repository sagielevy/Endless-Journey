using Assets.Scripts.CFGParser.DataHolder;
using EZObjectPools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts.CFGParser.Modifiers
{
    public class CloudModifier : PoolRequester, IWorldModifier//<ICloudsData>
    {
        private ISectionData sectionData;
        private TerrainGenerator terrainChunksParent;
        private bool hasRun;
        private Vector3 origin;

        public CloudModifier(ISectionData sectionData, EZObjectPool[] pools,
                              TerrainGenerator terrainChunksParent, Vector3 origin) :
            base("Clouds_", pools)
        {
            this.sectionData = sectionData;
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
                    var cloudName = "Clouds_" + cloud.subtypeIndex + "_" + subSubType;
                    //GameObject newCloud = GameObject.Instantiate(originalModels.transform.Find(plantName));
                    GameObject newCloud;

                    if (poolsDict[cloudName].TryGetNextObject(new Vector3(), Globals.defaultRotation, out newCloud))
                    {
                        newCloud.GetComponent<ItemComponent>().SetOrgLocalScale(newCloud.transform.localScale);

                        // Uniform scale
                        newCloud.transform.localScale *= cloud.scale_mul;

                        // Set chunk parent
                        var chunk = Helpers.FindClosestTerrain(terrainChunksParent, new Vector2(actualPosX, actualPosZ));
                        chunk.AddItem(newCloud.GetComponent<ItemComponent>());

                        // Set current item position to X, Y = cloud height, Z
                        newCloud.transform.position = new Vector3(actualPosX, cloud.height, actualPosZ);
                    }
                }
            }
        }
    }
}
