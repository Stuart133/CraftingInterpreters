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

        public LoxFunction(Token name, Expr.Function declaration, Environment closure)
        {
            _name = name;
            _declaration = declaration;
            _closure = closure;
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
                return returnValue.Value;
            }

            return null;
        }

        public override string ToString()
        {
            return $"<fn {_name.Lexeme}>";
        }
    }
}
