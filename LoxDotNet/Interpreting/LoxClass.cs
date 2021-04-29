using System.Collections.Generic;

namespace LoxDotNet.Interpreting
{
    class LoxClass : ICallable
    {
        internal string Name { get; }
        private Dictionary<string, LoxFunction> _methods;

        internal LoxClass(string name, Dictionary<string, LoxFunction> methods)
        {
            Name = name;
            _methods = methods;
        }

        public override string ToString()
        {
            return Name;
        }

        public int Arity()
        {
            var initializer = FindMethod("init");

            return initializer?.Arity() ?? 0;
        }

        public object Call(Interpreter interpreter, List<object> arguments)
        {
            var instance = new LoxInstance(this);
            var initializer = FindMethod("init");

            if (initializer is not null)
            {
                initializer.Bind(instance).Call(interpreter, arguments);
            }

            return instance;
        }

        internal LoxFunction FindMethod(string name)
        {
            if (_methods.ContainsKey(name))
            {
                return _methods[name];
            }

            return null;
        }
    }
}
