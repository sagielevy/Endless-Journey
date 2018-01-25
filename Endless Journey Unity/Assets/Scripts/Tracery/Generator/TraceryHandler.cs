using System;
using System.Collections.Generic;
using System.Text;
using Tracery;
using UnityEngine;

namespace Scripts.Tracery.Generator
{
    /// <summary>
    /// Singleton class that handles Tracery engine and generates CFG sentences
    /// </summary>
    public class TraceryHandler
    {
        const string CFG_TRUNK = "#start#";
        private Grammar grammar;

        public void LoadCFG(string cfgFilepath)
        {
            TextAsset jsonFile = Resources.Load(cfgFilepath) as TextAsset; 
            this.grammar = Grammar.LoadFromJSON(jsonFile);
        }

        public string GenerateSentence()
        {
            return this.grammar.Flatten(CFG_TRUNK);
        }
    }
}
