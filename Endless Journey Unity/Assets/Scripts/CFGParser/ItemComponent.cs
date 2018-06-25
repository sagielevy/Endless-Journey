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
        private Renderer compRenderer;
        private float alpha = 0;

        protected Vector3 orgLocalScale;

        private void Awake()
        {
            compRenderer = GetComponentInChildren<Renderer>();
        }

        //private void FixedUpdate()
        //{
        //    var newAlpha = Mathf.Lerp(alpha, 1, Time.deltaTime);

        //    if (newAlpha > 0.95f)
        //        newAlpha = 1;

        //    // Slowly reveal
        //    var newMats = new Material[compRenderer.materials.Length];
        //    var matsRef = compRenderer.materials;

        //    foreach (var mat in matsRef)
        //    {
        //        mat.color = new Color(compRenderer.material.color.r,
        //                                compRenderer.material.color.g,
        //                                compRenderer.material.color.b,
        //                                alpha);
        //    }

        //    compRenderer.materials = newMats;
        //}

        public void UpdatePosition(Vector3 newPos)
        {
            transform.position = newPos;

            alpha = 0;

            // Set as transparent
            //var newMats = new Material[compRenderer.materials.Length];
            //var matsRef = compRenderer.materials;

            //foreach (var mat in matsRef)
            //{
            //    mat.color = new Color(compRenderer.material.color.r,
            //                            compRenderer.material.color.g,
            //                            compRenderer.material.color.b,
            //                            alpha);
            //}

            //compRenderer.materials = newMats;
        }

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
