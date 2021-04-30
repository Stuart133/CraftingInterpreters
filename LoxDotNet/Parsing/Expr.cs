// This file is auto generated from LoxTools - Do not modify directly

using LoxDotNet.Scanning;
using System.Collections.Generic;

namespace LoxDotNet.Parsing
{
	public abstract class Expr
	{
		public interface IVisitor<T>
		{
			T VisitAssignExpr(Assign expr);
			T VisitBinaryExpr(Binary expr);
			T VisitCallExpr(Call expr);
			T VisitConditionalExpr(Conditional expr);
			T VisitGetExpr(Get expr);
			T VisitSetExpr(Set expr);
			T VisitSuperExpr(Super expr);
			T VisitFunctionExpr(Function expr);
			T VisitGroupingExpr(Grouping expr);
			T VisitLiteralExpr(Literal expr);
			T VisitLogicalExpr(Logical expr);
			T VisitThisExpr(This expr);
			T VisitVariableExpr(Variable expr);
			T VisitUnaryExpr(Unary expr);
		}

		public class Assign : Expr
		{
			internal Assign(Token name, Expr value)
			{
				this.name = name;
				this.value = value;
			}

			internal override T Accept<T>(IVisitor<T> visitor)
			{
				return visitor.VisitAssignExpr(this);
			}

			internal Token name { get; }
			internal Expr value { get; }
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

		public class Call : Expr
		{
			internal Call(Expr callee, Token paren, List<Expr> arguments)
			{
				this.callee = callee;
				this.paren = paren;
				this.arguments = arguments;
			}

			internal override T Accept<T>(IVisitor<T> visitor)
			{
				return visitor.VisitCallExpr(this);
			}

			internal Expr callee { get; }
			internal Token paren { get; }
			internal List<Expr> arguments { get; }
		}

		public class Conditional : Expr
		{
			internal Conditional(Expr ifExpr, Expr thenBranch, Expr elseBranch)
			{
				this.ifExpr = ifExpr;
				this.thenBranch = thenBranch;
				this.elseBranch = elseBranch;
			}

			internal override T Accept<T>(IVisitor<T> visitor)
			{
				return visitor.VisitConditionalExpr(this);
			}

			internal Expr ifExpr { get; }
			internal Expr thenBranch { get; }
			internal Expr elseBranch { get; }
		}

		public class Get : Expr
		{
			internal Get(Expr obj, Token name)
			{
				this.obj = obj;
				this.name = name;
			}

			internal override T Accept<T>(IVisitor<T> visitor)
			{
				return visitor.VisitGetExpr(this);
			}

			internal Expr obj { get; }
			internal Token name { get; }
		}

		public class Set : Expr
		{
			internal Set(Expr obj, Token name, Expr value)
			{
				this.obj = obj;
				this.name = name;
				this.value = value;
			}

			internal override T Accept<T>(IVisitor<T> visitor)
			{
				return visitor.VisitSetExpr(this);
			}

			internal Expr obj { get; }
			internal Token name { get; }
			internal Expr value { get; }
		}

		public class Super : Expr
		{
			internal Super(Token keyword, Token method)
			{
				this.keyword = keyword;
				this.method = method;
			}

			internal override T Accept<T>(IVisitor<T> visitor)
			{
				return visitor.VisitSuperExpr(this);
			}

			internal Token keyword { get; }
			internal Token method { get; }
		}

		public class Function : Expr
		{
			internal Function(List<Token> parameters, List<Stmt> body)
			{
				this.parameters = parameters;
				this.body = body;
			}

			internal override T Accept<T>(IVisitor<T> visitor)
			{
				return visitor.VisitFunctionExpr(this);
			}

			internal List<Token> parameters { get; }
			internal List<Stmt> body { get; }
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

		public class Logical : Expr
		{
			internal Logical(Expr left, Token op, Expr right)
			{
				this.left = left;
				this.op = op;
				this.right = right;
			}

			internal override T Accept<T>(IVisitor<T> visitor)
			{
				return visitor.VisitLogicalExpr(this);
			}

			internal Expr left { get; }
			internal Token op { get; }
			internal Expr right { get; }
		}

		public class This : Expr
		{
			internal This(Token keyword)
			{
				this.keyword = keyword;
			}

			internal override T Accept<T>(IVisitor<T> visitor)
			{
				return visitor.VisitThisExpr(this);
			}

			internal Token keyword { get; }
		}

		public class Variable : Expr
		{
			internal Variable(Token name)
			{
				this.name = name;
			}

			internal override T Accept<T>(IVisitor<T> visitor)
			{
				return visitor.VisitVariableExpr(this);
			}

			internal Token name { get; }
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
