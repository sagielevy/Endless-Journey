using Assets.Scripts.CFGParser.DataHolder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityStandardAssets.Characters.FirstPerson;

namespace Assets.Scripts.CFGParser.Modifiers
{
    public class SectionModifier : IWorldModifier//<ISectionData>
    {
        private float distThreshold = 1.5f;
        private float sectionLengthPassThreshold = 1.3f;
        Vector3 initialPosition;
        Rigidbody player;
        RigidbodyFirstPersonController Controller;
        ISectionData data;
        SectionAngle[] angles;
        Vector2[] points;
        int currSubPath;
        bool hasRun;

        public Vector2 movement { get; private set; }

        public SectionModifier(Transform player, ISectionData data)
        {
            initialPosition = player.position;
            this.player = player.GetComponent<Rigidbody>();
            Controller = player.GetComponent<RigidbodyFirstPersonController>();
            this.data = data;
            hasRun = false;
            currSubPath = 0;
        }

        public void ModifySection(ISentenceData data)
        {
            var sectionData = data as ISectionData;

            // Get data
            if (!hasRun)
            {
                hasRun = true;

                angles = sectionData.SectionAngles();
                Array.Sort(angles, new SectionAngleComp());

                
                points = new Vector2[angles.Length + 1];

                for (int i = 0; i < angles.Length; i++)
                {
                    points[i] = new Vector2(initialPosition.x + angles[i].pos_x * sectionData.SectionLength(),
                                            initialPosition.z + angles[i].pos_z * sectionData.SectionLength());

                }

                points[angles.Length] = new Vector2(sectionData.SectionLength(), 0);
            }

            // Move Character! Currently not working well...
            // If not done with curr sub path
            //if (Vector2.Distance(points[currSubPath], new Vector2(player.position.x, player.position.z)) < distThreshold) {
            //    currSubPath++;
            //}

            //if (currSubPath < points.Length)
            //{
            //    movement = new Vector2(Mathf.Clamp(points[currSubPath].x - player.position.x, 0, 1),
            //                       Mathf.Clamp(points[currSubPath].y - player.position.z, 0, 1));
            //}
            //else
            //{
            //    movement = new Vector2();
            //}
        }

        public bool IsSectionComplete()
        {
            return Vector3.Distance(initialPosition, player.position) >= data.SectionLength() / sectionLengthPassThreshold;
        }
    }

    class SectionAngleComp : IComparer<SectionAngle>
    {
        public int Compare(SectionAngle x, SectionAngle y)
        {
            //return Mathf.RoundToInt(x.x - y.x);
            return Mathf.RoundToInt(new Vector2(x.pos_x, x.pos_z).magnitude -
                                    new Vector2(y.pos_x, y.pos_z).magnitude);
        }
    }
}
