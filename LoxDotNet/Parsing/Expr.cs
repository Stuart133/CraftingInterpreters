using LoxDotNet.Scanning;

namespace LoxDotNet.Parsing
{
	abstract class Expr
	{
		class Binary : Expr
		{
			internal Binary(Expr left, Token op, Expr right)
			{
				this.left = left;
				this.op = op;
				this.right = right;
			}

			internal Expr left { get; }
			internal Token op { get; }
			internal Expr right { get; }
		}

		class Grouping : Expr
		{
			internal Grouping(Expr expression)
			{
				this.expression = expression;
			}

			internal Expr expression { get; }
		}

		class Literal : Expr
		{
			internal Literal(object value)
			{
				this.value = value;
			}

			internal object value { get; }
		}

		class Unary : Expr
		{
			internal Unary(Token op, Expr right)
			{
				this.op = op;
				this.right = right;
			}

			internal Token op { get; }
			internal Expr right { get; }
		}

	}
}
