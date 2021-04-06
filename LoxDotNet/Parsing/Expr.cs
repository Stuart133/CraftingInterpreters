// This file is auto generated from LoxTools - Do not modify directly

using LoxDotNet.Scanning;

namespace LoxDotNet.Parsing
{
	abstract class Expr
	{
		internal interface IVisitor<T>
		{
			internal T VisitBinaryExpr(Binary expr);
			internal T VisitGroupingExpr(Grouping expr);
			internal T VisitLiteralExpr(Literal expr);
			internal T VisitUnaryExpr(Unary expr);
		}

		internal class Binary : Expr
		{
			internal Binary(Expr left, Token op, Expr right)
			{
				this.left = left;
				this.op = op;
				this.right = right;
			}

			internal override T Accept<T>(IVisitor<T> visitor)
			{
				return visitor.VisitBinaryExpr(this);
			}

			internal Expr left { get; }
			internal Token op { get; }
			internal Expr right { get; }
		}

		internal class Grouping : Expr
		{
			internal Grouping(Expr expression)
			{
				this.expression = expression;
			}

			internal override T Accept<T>(IVisitor<T> visitor)
			{
				return visitor.VisitGroupingExpr(this);
			}

			internal Expr expression { get; }
		}

		internal class Literal : Expr
		{
			internal Literal(object value)
			{
				this.value = value;
			}

			internal override T Accept<T>(IVisitor<T> visitor)
			{
				return visitor.VisitLiteralExpr(this);
			}

			internal object value { get; }
		}

		internal class Unary : Expr
		{
			internal Unary(Token op, Expr right)
			{
				this.op = op;
				this.right = right;
			}

			internal override T Accept<T>(IVisitor<T> visitor)
			{
				return visitor.VisitUnaryExpr(this);
			}

			internal Token op { get; }
			internal Expr right { get; }
		}

		internal abstract T Accept<T>(IVisitor<T> visitor);
	}
}
