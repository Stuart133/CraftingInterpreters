using System.Collections.Generic;

namespace LoxDotNet.Interpreting
{
    class LoxClass : ICallable
    {
        internal string Name { get; }

        private readonly LoxClass _superclass;
        private readonly Dictionary<string, LoxFunction> _methods;

        internal LoxClass(string name, LoxClass superclass, Dictionary<string, LoxFunction> methods)
        {
            Name = name;
            _superclass = superclass;
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

            if (_superclass is not null)
            {
                return _superclass.FindMethod(name);
            }

            return null;
        }
    }
}
