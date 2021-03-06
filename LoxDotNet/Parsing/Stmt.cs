// This file is auto generated from LoxTools - Do not modify directly

using LoxDotNet.Scanning;
using System.Collections.Generic;

namespace LoxDotNet.Parsing
{
	public abstract class Stmt
	{
		public interface IVisitor<T>
		{
			T VisitBlockStmt(Block stmt);
			T VisitClassStmt(Class stmt);
			T VisitBreakStmt(Break stmt);
			T VisitExpressionStmt(Expression stmt);
			T VisitFunctionStmt(Function stmt);
			T VisitIfStmt(If stmt);
			T VisitPrintStmt(Print stmt);
			T VisitReturnStmt(Return stmt);
			T VisitVarStmt(Var stmt);
			T VisitWhileStmt(While stmt);
		}

		public class Block : Stmt
		{
			internal Block(List<Stmt> statements)
			{
				this.statements = statements;
			}

			internal override T Accept<T>(IVisitor<T> visitor)
			{
				return visitor.VisitBlockStmt(this);
			}

			internal List<Stmt> statements { get; }
		}

		public class Class : Stmt
		{
			internal Class(Token name, Expr.Variable superclass, List<Stmt.Function> methods)
			{
				this.name = name;
				this.superclass = superclass;
				this.methods = methods;
			}

			internal override T Accept<T>(IVisitor<T> visitor)
			{
				return visitor.VisitClassStmt(this);
			}

			internal Token name { get; }
			internal Expr.Variable superclass { get; }
			internal List<Stmt.Function> methods { get; }
		}

		public class Break : Stmt
		{
			internal Break()
			{
			}

			internal override T Accept<T>(IVisitor<T> visitor)
			{
				return visitor.VisitBreakStmt(this);
			}

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

		public class Function : Stmt
		{
			internal Function(Token name, Expr.Function function)
			{
				this.name = name;
				this.function = function;
			}

			internal override T Accept<T>(IVisitor<T> visitor)
			{
				return visitor.VisitFunctionStmt(this);
			}

			internal Token name { get; }
			internal Expr.Function function { get; }
		}

		public class If : Stmt
		{
			internal If(Expr condition, Stmt thenBranch, Stmt elseBranch)
			{
				this.condition = condition;
				this.thenBranch = thenBranch;
				this.elseBranch = elseBranch;
			}

			internal override T Accept<T>(IVisitor<T> visitor)
			{
				return visitor.VisitIfStmt(this);
			}

			internal Expr condition { get; }
			internal Stmt thenBranch { get; }
			internal Stmt elseBranch { get; }
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

		public class Return : Stmt
		{
			internal Return(Token keyword, Expr value)
			{
				this.keyword = keyword;
				this.value = value;
			}

			internal override T Accept<T>(IVisitor<T> visitor)
			{
				return visitor.VisitReturnStmt(this);
			}

			internal Token keyword { get; }
			internal Expr value { get; }
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

		public class While : Stmt
		{
			internal While(Expr condition, Stmt body)
			{
				this.condition = condition;
				this.body = body;
			}

			internal override T Accept<T>(IVisitor<T> visitor)
			{
				return visitor.VisitWhileStmt(this);
			}

			internal Expr condition { get; }
			internal Stmt body { get; }
		}

		internal abstract T Accept<T>(IVisitor<T> visitor);
	}
}
