using System;
using System.Text;

namespace Tracery
{
	public class RuleNode : TraceryNode
	{
		private TraceryNode[] sections;

		public RuleNode(TraceryNode[] sections, string raw) : base(raw)
		{
			this.sections = sections;
		}

        public override object Clone()
        {
            return new RuleNode(sections, Raw);
        }

        public override string Flatten(Grammar grammar)
		{
			StringBuilder builder = new StringBuilder();
			foreach (TraceryNode section in sections)
			{
				builder.Append(section.Flatten(grammar));
			}
			return builder.ToString();
		}
	}
}
