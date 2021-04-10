using LoxDotNet.Scanning;
using System.Collections.Generic;

namespace LoxDotNet.Interpreting
{
    class Environment
    {
        private readonly Dictionary<string, object> _values = new Dictionary<string, object>();

        internal void Define(string name, object value)
        {
            _values[name] = value;
        }

        internal object Get(Token name)
        {
            if (_values.TryGetValue(name.Lexeme, out var value))
            {
                return value;
            }

            throw new RuntimeException(name, $"Undefined variable '{name.Lexeme}'.");
        }
    }
}
