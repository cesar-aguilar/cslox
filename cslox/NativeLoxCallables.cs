using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cslox
{
  internal class NativeLoxCallables
  {

    public class Clock : ILoxCallable
    {

      public int Arity()
      {
        return 0;
      }

      public Object Call(Interpreter interpreter, List<Object> arguments)
      {
        return (double)DateTimeOffset.Now.ToUnixTimeMilliseconds() / 1000.0;
      }

      public override string ToString()
      {
        return "<native fn: Clock>";
      }

    }

  }
}
