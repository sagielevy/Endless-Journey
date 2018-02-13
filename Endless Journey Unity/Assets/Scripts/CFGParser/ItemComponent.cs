using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts.CFGParser
{
    public class ItemComponent : MonoBehaviour
    {
        protected Vector3 orgLocalScale;

        public void SetOrgLocalScale(Vector3 orgLocalScale)
        {
            this.orgLocalScale = orgLocalScale;
        }

        public void PreDisable()
        {
            transform.localScale = orgLocalScale;
        }
    }
}
