using EZObjectPools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts.CFGParser.Modifiers
{
    public class PoolRequester
    {
        protected Dictionary<string, EZObjectPool> poolsDict;

        public PoolRequester(string poolsPrefix, EZObjectPool[] pools)
        {
            poolsDict = new Dictionary<string, EZObjectPool>();

            foreach (var pool in pools)
            {
                if (pool.PoolName.StartsWith(poolsPrefix))
                {
                    poolsDict[pool.PoolName] = pool;
                }
            }
        }
    }
}
