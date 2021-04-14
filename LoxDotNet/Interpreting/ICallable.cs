using System.Collections.Generic;

namespace LoxDotNet.Interpreting
{
    public interface ICallable
    {
        object Call(Interpreter interpreter, List<object> arguments);
    }
}
