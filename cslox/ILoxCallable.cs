using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cslox
{
  internal interface ILoxCallable
  {
    Object Call(Interpreter interpreter, List<Object> arguments);
  }
}
