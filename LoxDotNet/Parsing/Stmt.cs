// This file is auto generated from LoxTools - Do not modify directly

namespace LoxDotNet.Parsing
{
    public abstract class Stmt
	{
		public interface IVisitor<T>
		{
			T VisitExpressionStmt(Expression stmt);
			T VisitPrintStmt(Print stmt);
		}

		public class Expression : Stmt
		{
			internal Expression(Expr expression)
			{
				this.expression = expression;
			}

			internal override T Accept<T>(IVisitor<T> visitor)
			{
				return visitor.VisitExpressionStmt(this);
			}

			internal Expr expression { get; }
		}

		public class Print : Stmt
		{
			internal Print(Expr expression)
			{
				this.expression = expression;
			}

			internal override T Accept<T>(IVisitor<T> visitor)
			{
				return visitor.VisitPrintStmt(this);
			}

			internal Expr expression { get; }
		}

		internal abstract T Accept<T>(IVisitor<T> visitor);
	}
}
