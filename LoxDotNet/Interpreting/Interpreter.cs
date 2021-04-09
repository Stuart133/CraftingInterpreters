using LoxDotNet.Parsing;
using static LoxDotNet.Scanning.TokenType;

namespace LoxDotNet.Interpreting
{
    class Interpreter : Expr.IVisitor<object>
    {
        public object VisitBinaryExpr(Expr.Binary expr)
        {
            var left = Evaluate(expr.left);
            var right = Evaluate(expr.right);

            switch (expr.op.Type)
            {
                case PLUS:
                    if (left is double ld && right is double rd)
                    {
                        return ld + rd;
                    }
                    else if (left is string ls && right is string rs)
                    {
                        return ls + rs;
                    }
                    else
                    {
                        return null;
                    }                    
                case MINUS:
                    return (double)left - (double)right;
                case SLASH:
                    return (double)left / (double)right;
                case STAR:
                    return (double)left * (double)right;
                default:
                    return null;
            }
        }

        public object VisitConditionalExpr(Expr.Conditional expr)
        {
            throw new System.NotImplementedException();
        }

        public object VisitGroupingExpr(Expr.Grouping expr)
        {
            return Evaluate(expr.expression);
        }

        public object VisitLiteralExpr(Expr.Literal expr)
        {
            return expr.value;
        }

        public object VisitUnaryExpr(Expr.Unary expr)
        {
            var right = Evaluate(expr.right);

            return expr.op.Type switch
            {
                MINUS => -(double)right,
                BANG => !IsTruthy(right),
                _ => null,
            };
        }

        private object Evaluate(Expr expr)
        {
            return expr.Accept(this);
        }

        private bool IsTruthy(object value)
        {
            // Use ruby like truthy semantics
            return value switch
            {
                bool b => b,
                null => false,
                _ => true,
            };
        }
    }
}
