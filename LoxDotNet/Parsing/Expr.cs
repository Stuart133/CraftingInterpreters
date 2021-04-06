// This file is auto generated from LoxTools - Do not modify directly

using LoxDotNet.Scanning;

namespace LoxDotNet.Parsing
{
	public abstract class Expr
	{
		public interface IVisitor<T>
		{
			T VisitBinaryExpr(Binary expr);
			T VisitGroupingExpr(Grouping expr);
			T VisitLiteralExpr(Literal expr);
			T VisitUnaryExpr(Unary expr);
		}

		public class Binary : Expr
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

		public class Grouping : Expr
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

		public class Literal : Expr
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

		public class Unary : Expr
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
