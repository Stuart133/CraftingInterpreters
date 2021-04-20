using LoxDotNet.Scanning;

namespace LoxDotNet.Resolving
{
    internal class Variable
    {
        internal Token Name { get; }
        internal VariableState State { get; set; }

        internal Variable(Token name, VariableState state)
        {
            Name = name;
            State = state;
        }
    }

    internal enum VariableState
    {
        Declared,
        Defined,
        Read
    }
}
