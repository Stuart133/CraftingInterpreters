﻿using LoxDotNet.Parsing;
using System;
using System.Collections.Generic;

namespace LoxDotNet.Interpreting
{
    class LoxFunction : ICallable
    {
        private readonly Stmt.Function _declaration;

        public LoxFunction(Stmt.Function declaration)
        {
            _declaration = declaration;
        }

        public int Arity()
        {
            return _declaration.parameters.Count;
        }

        public object Call(Interpreter interpreter, List<object> arguments)
        {
            var environment = new Environment(interpreter.Globals);
            for (var i = 0; i < _declaration.parameters.Count; i++)
            {
                environment.Define(_declaration.parameters[i].Lexeme, arguments[i]);
            }

            interpreter.ExecuteBlock(_declaration.body, environment);
            return null;
        }
    }
}