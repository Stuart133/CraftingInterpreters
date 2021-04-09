using LoxDotNet.Scanning;
using System;

namespace LoxDotNet.Interpreting
{
    public class RuntimeException : Exception
    {
        internal Token Token { get; }

        internal RuntimeException(Token token, string message)
            : base(message)
        {
            Token = token;
        }
    }
}
