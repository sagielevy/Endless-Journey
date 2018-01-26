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
        //private float[,] heightMap;
        private GameObject originalModels;
        private bool hasRun;
        private Transform parent;

        public PlantsModifier(ISectionData sectionData, GameObject originalModels, Transform parent)
        {
            this.sectionData = sectionData;
            this.originalModels = originalModels;
            this.parent = parent;
            hasRun = false;
        }

        public void ModifySection(ISentenceData data)
        {
            if (!hasRun) {
                hasRun = true;
                System.Random random = new System.Random();
                var plantData = data as IPlantsData;
                float actualPosX, actualPosZ;

                int[] maxes = new int[] { 10, 8, 4 };

                foreach (var plant in plantData.Plants())
                {
                    // Origin center of section
                    actualPosX = plant.pos_x_percent * sectionData.SectionLength() / 2;
                    actualPosZ = plant.pos_z_percent * sectionData.SectionLength() / 2;

                    // FOR DEBUG
                    int subSubType = random.Next(1, maxes[plant.subtypeIndex - 1]);
                    var plantName = "Plants_" + plant.subtypeIndex + "_" + subSubType;
                    var newPlant = GameObject.Instantiate(originalModels.transform.Find(plantName));
                    newPlant.parent = parent;

                    // Set normal Y value!
                    newPlant.localPosition = new Vector3(actualPosX, 70f, actualPosZ);

                    // Enable stuff!!
                    newPlant.GetComponent<Rigidbody>().useGravity = true;
                    newPlant.GetComponent<MeshRenderer>().enabled = true;
                }
            }
        }
    }
}
