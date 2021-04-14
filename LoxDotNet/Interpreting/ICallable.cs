using System.Collections.Generic;

namespace LoxDotNet.Interpreting
{
    public interface ICallable
    {
        int Arity();
        object Call(Interpreter interpreter, List<object> arguments);
    }
}
