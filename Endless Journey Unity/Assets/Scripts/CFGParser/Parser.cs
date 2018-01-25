using Assets.Scripts.CFGParser.DataHolder;
using Assets.Scripts.CFGParser.Modifiers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.CFGParser
{
    public class Parser
    {
        // TODO either make this class a MonoBehaviour and reference all the required components in the scene
        // or have another class do it. Either way make interface getters just like the sentence data ones.
        // Send to modifiers
        SentenceDataHolder sentenceDataHolder;
        List<ISectionModifier<ISectionData>> modifiers;

        public void Start()
        {
            // Init stuff
            
        }

        public void Update()
        {

        }

        // TODO Think of a way to free items created previously
    }
}
