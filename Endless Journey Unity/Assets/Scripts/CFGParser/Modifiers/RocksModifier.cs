using Assets.Scripts.CFGParser.DataHolder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts.CFGParser.Modifiers
{
    public class RocksModifier : IWorldModifier
    {
        private ISectionData sectionData;
        private GameObject originalModels;
        private TerrainGenerator terrainChunksParent;
        private Vector3 origin;
        private bool hasRun;

        public RocksModifier(ISectionData sectionData, GameObject originalModels,
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
                var rocksData = data as IRocksData;
                float actualPosX, actualPosZ;

                int[] maxes = new int[] { 5 };

                foreach (var rock in rocksData.Rocks())
                {
                    // Origin center of section
                    actualPosX = origin.x + (rock.pos_x_percent * sectionData.SectionLength() * Globals.groundSeperateMul);
                    actualPosZ = origin.z + (rock.pos_z_percent * sectionData.SectionLength() * Globals.groundSeperateMul);

                    // FOR DEBUG
                    int subSubType = random.Next(1, maxes[rock.subtypeIndex - 1]);
                    var rockName = "Rocks_" + rock.subtypeIndex + "_" + subSubType;
                    var newRock = GameObject.Instantiate(originalModels.transform.Find(rockName));

                    // Uniform scale
                    newRock.localScale *= rock.scale_mul;

                    // Set chunk parent
                    var chunk = Helpers.FindClosestTerrain(terrainChunksParent, new Vector2(actualPosX, actualPosZ));
                    chunk.AddItem(newRock);

                    // Save original actual pos
                    newRock.GetComponent<GroundItemComponent>().ActualOriginalPos = new Vector2(actualPosX, actualPosZ);

                    // Set current item position to X, Z
                    newRock.position = new Vector3(actualPosX, Globals.maxHeight, actualPosZ);
                }
            }
        }
    }
}
