using LoxDotNet.Scanning;

namespace LoxDotNet.Parsing
{
    abstract class Expr
    {
        class Binary : Expr
        {
            internal Expr Left { get; }
            internal Token Operator { get; }
            internal Expr Right { get; }

            internal Binary(Expr left, Token op, Expr right)
            {
                Left = left;
                Operator = op;
                Right = right;
            }
        }
    }
}
