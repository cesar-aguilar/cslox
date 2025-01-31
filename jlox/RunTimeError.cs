using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace jlox
{
	internal class RunTimeError : Exception
	{
		public readonly Token token;

		public RunTimeError(Token token, string message) : base(message)
		{
			this.token = token;
		}
	}
}
