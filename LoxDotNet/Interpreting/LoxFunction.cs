﻿using LoxDotNet.Parsing;
using System;
using System.Collections.Generic;

namespace LoxDotNet.Interpreting
{
    class LoxFunction : ICallable
    {
        private readonly Stmt.Function _declaration;
        private readonly Environment _closure;

        public LoxFunction(Stmt.Function declaration, Environment closure)
        {
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
            return $"<fn {_declaration.name.Lexeme}>";
        }
    }
}
