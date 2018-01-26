using System;

namespace Tracery
{
	public abstract class TraceryNode : ICloneable
    {
		public readonly string Raw;

		public TraceryNode(string raw)
		{
			this.Raw = raw;
		}

        public abstract object Clone();

        public abstract string Flatten(Grammar grammar);
	}
}
