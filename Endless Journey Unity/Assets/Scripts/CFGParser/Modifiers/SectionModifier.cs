using Assets.Scripts.CFGParser.DataHolder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts.CFGParser.Modifiers
{
    public class SectionModifier : IWorldModifier<ISectionData>
    {
        Vector3 initialPosition;
        Transform player;
        ISectionData data;

        public SectionModifier(Vector3 initialPos, Transform player, ISectionData data)
        {
            initialPosition = initialPos;
            this.player = player;
            this.data = data;
        }

        public void ModifySection(ISectionData data)
        {
            
        }

        public bool IsSectionComplete()
        {
            return Vector3.Distance(initialPosition, player.position) >= data.SectionLength();
        }
    }
}
