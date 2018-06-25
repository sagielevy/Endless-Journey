using Assets.Scripts.CFGParser.DataHolder;
using EZObjectPools;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts.CFGParser.Modifiers
{
    public class RocksModifier : PoolRequester, IWorldModifier
    {
        private ISectionData sectionData;
        private TerrainGenerator terrainChunksParent;
        private Vector3 origin;

        public RocksModifier(ISectionData sectionData, EZObjectPool[] pools,
                              TerrainGenerator terrainChunksParent, Vector3 origin) :
            base("Rocks_", pools)
        {
            this.sectionData = sectionData;
            this.terrainChunksParent = terrainChunksParent;
            this.origin = origin;
        }

        public IEnumerator<WaitForEndOfFrame> ModifySection(ISentenceData data)
        {
            System.Random random = new System.Random();
            var rocksData = data as IRocksData;
            float actualPosX, actualPosZ;

            int[] maxes = new int[] { 6 };

            foreach (var rock in rocksData.Rocks())
            {
                // Origin center of section
                actualPosX = origin.x + (rock.pos_x_percent * sectionData.SectionLength() * Globals.groundSeperateMul);
                actualPosZ = origin.z + (rock.pos_z_percent * sectionData.SectionLength() * Globals.groundSeperateMul);

                // FOR DEBUG
                int subSubType = random.Next(1, maxes[rock.subtypeIndex - 1]);
                var rockName = "Rocks_" + rock.subtypeIndex + "_" + subSubType;
                //var newRock = GameObject.Instantiate(originalModels.transform.Find(rockName));
                GameObject newRock;

                if (poolsDict[rockName].TryGetNextObject(new Vector3(), Globals.defaultRotation, out newRock))
                {
                    newRock.GetComponent<ItemComponent>().SetOrgLocalScale(newRock.transform.localScale);

                    // Uniform scale
                    newRock.transform.localScale *= rock.scale_mul;

                    // Set chunk parent
                    var chunk = Helpers.FindClosestTerrain(terrainChunksParent, new Vector2(actualPosX, actualPosZ));
                    chunk.AddItem(newRock.GetComponent<ItemComponent>());

                    // Save original actual pos
                    newRock.GetComponent<GroundItemComponent>().ActualOriginalPos = new Vector2(actualPosX, actualPosZ);

                    // Set inital pos!
                    chunk.PositionSingleGroundItem(newRock.GetComponent<GroundItemComponent>());
                }

                yield return Globals.EndOfFrame;
            }
        }
    }
}
