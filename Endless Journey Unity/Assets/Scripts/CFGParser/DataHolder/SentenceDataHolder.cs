using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Scripts.CFGParser.DataHolder
{
    public class SentenceDataHolder : ISkyData // Implement each section component here
    {
        private string orgSentence;

        public SentenceDataHolder(string cfgSentence)
        {
            // TODO parse sentence here into components. No need to convert them aside from int/float/string?
            orgSentence = cfgSentence;
        }

        public string ColorHorizon()
        {
            throw new NotImplementedException();
        }

        public string ColorSky()
        {
            throw new NotImplementedException();
        }

        public override string ToString()
        {
            return orgSentence;
        }
    }
}
