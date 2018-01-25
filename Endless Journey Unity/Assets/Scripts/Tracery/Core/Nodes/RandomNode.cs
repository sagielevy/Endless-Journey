using NUnit.Framework;
using System;
using System.Linq;

namespace Tracery
{
	public class RandomNode : TraceryNode
	{
		private string key;

		public RandomNode(string key) : base("")
		{
			this.key = key;
		}

		public override string Flatten(Grammar grammar)
		{
            string range = this.key.Substring("random".Length);
            string[] splitted = range.Split('~');
            Assert.AreEqual(splitted.Length, 2, "Random range must have lower and upper bounds " + key);
            double lower = double.Parse(splitted[0]);
            double upper= double.Parse(splitted[1]);
            double ratio = Tracery.Rng.NextDouble();
            double result = lower + ((upper - lower) * ratio);
            return result.ToString();
        }
	}
}
