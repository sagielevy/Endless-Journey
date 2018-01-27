using UnityEngine.Assertions;
using System;
using System.Linq;
using System.Text;

namespace Tracery
{
	public class RepeatNode : TraceryNode
	{
		private string key;

		public RepeatNode(string key) : base("")
		{
			this.key = key;
		}

		public override string Flatten(Grammar grammar)
		{
            string inner = this.key.Substring("repeat".Length + 1);
            int exprStart = inner.IndexOf(' ');
            string expression = inner.Substring(exprStart);
            int repeat = int.Parse(inner.TrimEnd(expression.ToCharArray()));
            string innerExpr = expression.Substring(2, expression.Length - 3);
            string flattened = grammar.Flatten(innerExpr);
            return Repeat(flattened, repeat);
        }

        public static string Repeat(string input, int count)
        {
            if (!string.IsNullOrEmpty(input))
            {
                StringBuilder builder = new StringBuilder(input.Length * count);

                for (int i = 0; i < count; i++) builder.Append(input);

                return builder.ToString();
            }

            return string.Empty;
        }

        public override object Clone()
        {
            return new RepeatNode(key);
        }
    }
}
