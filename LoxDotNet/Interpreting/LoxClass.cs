using System.Collections.Generic;

namespace LoxDotNet.Interpreting
{
    class LoxClass : ICallable
    {
        internal string Name { get; }

        internal LoxClass(string name)
        {
            Name = name;
        }

        public override string ToString()
        {
            return Name;
        }

        public int Arity()
        {
            return 0;
        }

        public object Call(Interpreter interpreter, List<object> arguments)
        {
            var instance = new LoxInstance(this);
            return instance;
        }
    }
}
