// This file is auto generated from LoxTools - Do not modify directly

using LoxDotNet.Scanning;

namespace LoxDotNet.Parsing
{
	public abstract class Stmt
	{
		public interface IVisitor<T>
		{
			T VisitExpressionStmt(Expression stmt);
			T VisitPrintStmt(Print stmt);
			T VisitVarStmt(Var stmt);
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

		public class Var : Stmt
		{
			internal Var(Token name, Expr initializer)
			{
				this.name = name;
				this.initializer = initializer;
			}

			internal override T Accept<T>(IVisitor<T> visitor)
			{
				return visitor.VisitVarStmt(this);
			}

			internal Token name { get; }
			internal Expr initializer { get; }
		}

		internal abstract T Accept<T>(IVisitor<T> visitor);
	}
}
