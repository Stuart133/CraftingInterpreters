﻿using LoxDotNet.Scanning;
using System.Collections.Generic;

namespace LoxDotNet.Interpreting
{
    class Environment
    {
        private readonly Environment _enclosing;
        private readonly Dictionary<string, object> _values = new Dictionary<string, object>();

        public Environment(Environment environment = null)
        {
            _enclosing = environment;
        }

        internal void Define(string name, object value)
        {
            _values[name] = value;
        }

        internal void Assign(Token name, object value)
        {
            if (_values.ContainsKey(name.Lexeme))
            {
                _values[name.Lexeme] = value;
                return;
            }

            if (_enclosing is not null)
            {
                _enclosing.Assign(name, value);
                return;
            }

            throw new RuntimeException(name, $"Undefined variable '{name.Lexeme}'.");
        }

        internal void AssignAt(int distance, Token name, object value)
        {
            Ancestor(distance)._values[name.Lexeme] = value;
        }

        internal object Get(Token name)
        {
            if (_values.TryGetValue(name.Lexeme, out var value))
            {
                return value;
            }

            if (_enclosing is not null)
            {
                return _enclosing.Get(name);
            }

            throw new RuntimeException(name, $"Undefined variable '{name.Lexeme}'.");
        }

        internal object GetAt(int distance, string name)
        {
            return Ancestor(distance)._values[name];
        }

        private Environment Ancestor(int distance)
        {
            var environment = this;
            for (var i = 0; i < distance; i++)
            {
                environment = environment._enclosing;
            }

            return environment;
        }
    }
}
