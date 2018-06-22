using Assets.Scripts.CFGParser.DataHolder;
using EZObjectPools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.AI;

namespace Assets.Scripts.CFGParser.Modifiers
{
    public class AnimalModifier : PoolRequester, IWorldModifier//<IAnimalsData>
    {
        const int flyerIndex = 1;
        const int groundIndex = 2;

        private ISectionData sectionData;
        private TerrainGenerator terrainChunksParent;
        private Vector3 origin;
        private bool hasRun;

        public AnimalModifier(ISectionData sectionData, EZObjectPool[] pools,
                              TerrainGenerator terrainChunksParent, Vector3 origin) :
            base("Animals_", pools)
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
                var animalData = data as IAnimalsData;
                float actualPosX, actualPosZ;

                int[] maxes = new int[] { 1,1 };

                foreach (var animal in animalData.Animals())
                {
                    // Origin center of section
                    actualPosX = origin.x + (animal.pos_x_percent * sectionData.SectionLength() * Globals.animalSeperateMul);
                    actualPosZ = origin.z + (animal.pos_z_percent * sectionData.SectionLength() * Globals.animalSeperateMul);

                    // FOR DEBUG (choose one of the available animal subsub types)
                    int subSubType = random.Next(1, maxes[animal.subtypeIndex - 1]);

                    var animalName = "Animals_" + animal.subtypeIndex + "_" + subSubType;
                    //var newAnimal = GameObject.Instantiate(originalModels.transform.Find(animalName));
                    GameObject newAnimal;

                    if (poolsDict[animalName].TryGetNextObject(new Vector3(), Quaternion.Euler(0, animal.angle * 360, 0), out newAnimal))
                    {
                        // Set org scale
                        newAnimal.GetComponent<ItemComponent>().SetOrgLocalScale(newAnimal.transform.localScale);

                        // Uniform scale
                        newAnimal.transform.localScale *= animal.scale_mul;

                        // Rotation around Y axis
                        //newAnimal.rotation = Quaternion.Euler(0, animal.angle * 360, 0);

                        // Set chunk parent
                        var chunk = Helpers.FindClosestTerrain(terrainChunksParent, new Vector2(actualPosX, actualPosZ));
                        chunk.AddItem(newAnimal.GetComponent<ItemComponent>());

                        if (newAnimal.GetComponent<GroundItemComponent>() != null)
                        {
                            // Save original actual pos
                            newAnimal.GetComponent<GroundItemComponent>().ActualOriginalPos = new Vector2(actualPosX, actualPosZ);

                            // Set inital pos!
                            chunk.PositionSingleGroundItem(newAnimal.GetComponent<GroundItemComponent>());

                        } else if (newAnimal.GetComponent<AirborneItemComponent>() != null)
                        {
                            // Set current item position
                            newAnimal.transform.position = new Vector3(actualPosX, animal.height, actualPosZ);
                        }
                    }
                }
            }
        }
    }
}
