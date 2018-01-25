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
        const string CFG_NAME = "CFG/EndlessJourneyCFG";
        const string CFG_TRUNK = "#start#";
        private static TraceryHandler Instance;
        private string cfgJson;

        public void LoadCFG(string cfgFilepath)
        {
            TextAsset jsonFile = Resources.Load(CFG_NAME) as TextAsset; 
            Grammar grammar = Grammar.LoadFromJSON(jsonFile);
            this.cfgJson = grammar.Flatten(CFG_TRUNK);
        }

        public string GenerateSentence()
        {
            throw new NotImplementedException();
        }
    }
}
