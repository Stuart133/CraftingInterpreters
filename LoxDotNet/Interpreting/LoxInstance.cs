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

            var method = _loxClass.FindMethod(name.Lexeme);
            if (method is not null)
            {
                return method.Bind(this);
            }

            throw new RuntimeException(name, $"Undefined property '{name.Lexeme}'.");
        }

        internal void Set(Token name, object value)
        {
            _fields[name.Lexeme] = value;
        }

        public override string ToString()
        {
            return $"{_loxClass.Name} instance";
        }
    }
}
