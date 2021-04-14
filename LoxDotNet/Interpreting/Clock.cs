using System;
using System.Collections.Generic;

namespace LoxDotNet.Interpreting
{
    class Clock : ICallable
    {
        public int Arity()
        {
            return 0;
        }

        public object Call(Interpreter interpreter, List<object> arguments)
        {
            return (DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalMilliseconds;
        }

        public override string ToString()
        {
            return "<native fn>";
        }
    }
}
