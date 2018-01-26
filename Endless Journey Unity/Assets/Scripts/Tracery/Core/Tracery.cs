namespace Tracery
{
	public static class Tracery
	{
		public static System.Random Rng = new System.Random();
        public static Grammar g_Grammar;
		public static TraceryNode ParseRule(string rawRule)
		{
			return new Parser(rawRule).Parse();
		}
	}
}
