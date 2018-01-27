using Assets.Scripts.CFGParser.DataHolder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts.CFGParser.Modifiers
{
    public class AnimalModifier : IWorldModifier//<IAnimalsData>
    {
        const int flyerIndex = 1;
        const int groundIndex = 2;

        private ISectionData sectionData;
        private GameObject originalModels;
        private TerrainGenerator terrainChunksParent;
        private bool hasRun;

        public AnimalModifier(ISectionData sectionData, GameObject originalModels,
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
                var animalData = data as IAnimalsData;
                float actualPosX, actualPosZ;

                int[] maxes = new int[] { 1,1 };

                foreach (var animal in animalData.Animals())
                {
                    // Origin center of section
                    actualPosX = animal.pos_x_percent * sectionData.SectionLength() * Globals.animalSeperateMul / 2;
                    actualPosZ = animal.pos_z_percent * sectionData.SectionLength() * Globals.animalSeperateMul / 2;

                    // FOR DEBUG
                    int subSubType = random.Next(1, maxes[animal.subtypeIndex - 1]);
                    var animalName = "Animals_" + animal.subtypeIndex + "_" + subSubType;
                    var newAnimal = GameObject.Instantiate(originalModels.transform.Find(animalName));

                    // Uniform scale
                    newAnimal.localScale *= animal.scale_mul;

                    // Set chunk parent
                    var chunk = Helpers.FindClosestTerrain(terrainChunksParent, new Vector2(actualPosX, actualPosZ));
                    chunk.AddItem(newAnimal);

                    // Set current item position
                    newAnimal.position = new Vector3(actualPosX, (flyerIndex == animal.subtypeIndex) ? Globals.birdHeight : Globals.maxHeight, actualPosZ);
                }
            }
        }
    }
}
