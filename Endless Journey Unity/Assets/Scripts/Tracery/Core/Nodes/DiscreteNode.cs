using System;
using System.Linq;
using UnityEngine.Assertions;

namespace Tracery
{
	public class DiscreteNode : TraceryNode
	{
		private string key;

		public DiscreteNode(string key) : base("")
		{
			this.key = key;
		}

        public override object Clone()
        {
            return new DiscreteNode(key);
        }

        public override string Flatten(Grammar grammar)
		{
            string range = this.key.Substring("discrete".Length);
            string[] splitted = range.Split('~');
            Assert.AreEqual(splitted.Length, 2, "Discrete range must have lower and upper bounds " + key);
            int lower = int.Parse(splitted[0]);
            int upper= int.Parse(splitted[1]);
            return Tracery.Rng.Next(lower, upper + 1).ToString();
        }
	}
}
