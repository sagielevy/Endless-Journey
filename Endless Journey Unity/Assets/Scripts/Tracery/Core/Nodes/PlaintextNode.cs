using System;

namespace Tracery
{
	public class PlaintextNode : TraceryNode
	{
		private string text;

		public PlaintextNode(string text, string raw) : base(raw)
		{
			this.text = text;
		}

        public override object Clone()
        {
            return new PlaintextNode(text, Raw);
        }

        public override string Flatten(Grammar grammar)
		{
			return text;
		}
	}
}
