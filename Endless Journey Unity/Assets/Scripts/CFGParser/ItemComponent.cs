using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts.CFGParser
{
    public class ItemComponent : MonoBehaviour
    {
        private TerrainChunk parentChunk;

        protected Vector3 orgLocalScale;

        public TerrainChunk ParentChunk { 
            get
            {
                return parentChunk;
            }
            set
            {
                if (parentChunk != null)
                {
                    parentChunk.RemoveItem(this);
                }

                parentChunk = value;
            }
        }


        public void SetOrgLocalScale(Vector3 orgLocalScale)
        {
            this.orgLocalScale = orgLocalScale;
        }

        public virtual void PreDisable()
        {
            transform.localScale = orgLocalScale;
        }
    }
}
