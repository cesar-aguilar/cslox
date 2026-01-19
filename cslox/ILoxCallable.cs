using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cslox
{
  internal interface ILoxCallable
  {

    int Arity();

    Object Call(Interpreter interpreter, List<Object> arguments);
  }
}
