using LoxDotNet.Scanning;
using System.Collections.Generic;

namespace LoxDotNet.Interpreting
{
    class LoxInstance
    {
        private readonly LoxClass _loxClass;
        private readonly Dictionary<string, object> _fields = new Dictionary<string, object>();

        internal LoxInstance(LoxClass loxClass)
        {
            _loxClass = loxClass;
        }

        internal object Get(Token name)
        {
            if (_fields.ContainsKey(name.Lexeme))
            {
                return _fields[name.Lexeme];
            }

            throw new RuntimeException(name, $"Undefined property '{name.Lexeme}'.");
        }

        public override string ToString()
        {
            return $"{_loxClass.Name} instance";
        }
    }
}
