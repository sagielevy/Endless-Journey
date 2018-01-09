using System;
using System.Collections.Generic;
using System.Text;

namespace CFGGeneratorCore.Tracery
{
    /// <summary>
    /// Singleton class that handles Tracery engine and generates CFG sentences
    /// </summary>
    public class TraceryHandler
    {
        private static TraceryHandler Instance;
        private string cfgJson;

        private Tracery() { }

        // Singleton method. Maybe discard singleton and make it a normal class?
        public static TraceryHandler GetInstance()
        {
            if (Instance == null)
            {
                Instance = new TraceryHandler();
            }

            return Instance;
        }

        public void LoadCFG(string cfgFilepath)
        {
            throw new NotImplementedException();
        }

        public string GenerateSentence()
        {
            throw new NotImplementedException();
        }
    }
}
