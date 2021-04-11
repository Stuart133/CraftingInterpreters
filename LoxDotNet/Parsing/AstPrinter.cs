using System.Collections.Generic;
using System.Text;

namespace LoxDotNet.Parsing
{
    public class AstPrinter : Expr.IVisitor<string>, Stmt.IVisitor<string>
    {
        public IEnumerable<string> Print(List<Stmt> statements)
        {
            var output = new List<string>();
            foreach(var statement in statements)
            {
                output.Add(statement.Accept(this));
            }

            return output;
        }

        public string VisitAssignExpr(Expr.Assign expr)
        {
            return Parenthesize($"set {expr.name.Lexeme}", expr.value);
        }

        public string VisitBinaryExpr(Expr.Binary expr)
        {
            return Parenthesize(expr.op.Lexeme, expr.left, expr.right);
        }

        public string VisitGroupingExpr(Expr.Grouping expr)
        {
            return Parenthesize("group", expr.expression);
        }

        public string VisitLiteralExpr(Expr.Literal expr)
        {
            if (expr.value == null)
            {
                return "null";
            }

            return expr.value.ToString();
        }

        public string VisitUnaryExpr(Expr.Unary expr)
        {
            return Parenthesize(expr.op.Lexeme, expr.right);
        }

        public string VisitVariableExpr(Expr.Variable expr)
        {
            return expr.name.Lexeme;
        }

        public string VisitConditionalExpr(Expr.Conditional expr)
        {
            return Parenthesize("conditional", expr.ifExpr, expr.thenBranch, expr.elseBranch);
        }

        public string VisitExpressionStmt(Stmt.Expression stmt)
        {
            return stmt.expression.Accept(this);
        }

        public string VisitPrintStmt(Stmt.Print stmt)
        {
            return Parenthesize("print", stmt.expression);
        }

        public string VisitVarStmt(Stmt.Var stmt)
        {
            return Parenthesize($"define {stmt.name.Lexeme}", stmt.initializer);
        }

        private string Parenthesize(string name, params Expr[] exprs)
        {
            var builder = new StringBuilder();

            builder.Append($"({name}");
            foreach (var expr in exprs)
            {
                builder.Append(" ");
                builder.Append(expr.Accept(this));
            }
            builder.Append(")");

            return builder.ToString();
        }
    }
}
