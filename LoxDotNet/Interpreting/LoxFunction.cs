using LoxDotNet.Parsing;
using LoxDotNet.Scanning;
using System.Collections.Generic;

namespace LoxDotNet.Interpreting
{
    class LoxFunction : ICallable
    {
        private readonly Token _name;
        private readonly Expr.Function _declaration;
        private readonly Environment _closure;
        private readonly bool _isInitializer;

        public LoxFunction(Token name, Expr.Function declaration, Environment closure, bool isInitializer)
        {
            _name = name;
            _declaration = declaration;
            _closure = closure;
            _isInitializer = isInitializer;
        }

        public int Arity()
        {
            return _declaration.parameters.Count;
        }

        public object Call(Interpreter interpreter, List<object> arguments)
        {
            var environment = new Environment(_closure);
            for (var i = 0; i < _declaration.parameters.Count; i++)
            {
                environment.Define(_declaration.parameters[i].Lexeme, arguments[i]);
            }

            try
            {
                interpreter.ExecuteBlock(_declaration.body, environment);
            }
            catch (Return returnValue)
            {
                if (_isInitializer)
                {
                    return _closure.GetAt(0, "this");
                }

                return returnValue.Value;
            }

            if (_isInitializer)
            {
                return _closure.GetAt(0, "this");
            }

            return null;
        }

        public LoxFunction Bind(LoxInstance loxInstance)
        {
            var environment = new Environment(_closure);
            environment.Define("this", loxInstance);
            return new LoxFunction(_name, _declaration, environment, _isInitializer);
        }

        public override string ToString()
        {
            return $"<fn {_name.Lexeme}>";
        }
    }
}
