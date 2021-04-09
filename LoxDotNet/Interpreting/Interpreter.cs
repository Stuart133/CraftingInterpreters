using LoxDotNet.Parsing;
using LoxDotNet.Scanning;
using System;
using static LoxDotNet.Scanning.TokenType;

namespace LoxDotNet.Interpreting
{
    class Interpreter : Expr.IVisitor<object>
    {
        public void Interpret(Expr expression)
        {
            try
            {
                var value = Evaluate(expression);
                Console.WriteLine(Stringify(value));
            }
            catch (RuntimeException ex)
            {
                Lox.RuntimeError(ex);
            }
        }

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
                        throw new RuntimeException(expr.op, "Operands must be two numbers or two strings");
                    }                    
                case MINUS:
                    CheckNumberOperands(expr.op, left, right);
                    return (double)left - (double)right;
                case SLASH:
                    CheckNumberOperands(expr.op, left, right);
                    return (double)left / (double)right;
                case STAR:
                    CheckNumberOperands(expr.op, left, right);
                    return (double)left * (double)right;
                case GREATER:
                    CheckNumberOperands(expr.op, left, right);
                    return (double)left > (double)right;
                case GREATER_EQUAL:
                    CheckNumberOperands(expr.op, left, right);
                    return (double)left >= (double)right;
                case LESS:
                    CheckNumberOperands(expr.op, left, right);
                    return (double)left < (double)right;
                case LESS_EQUAL:
                    CheckNumberOperands(expr.op, left, right);
                    return (double)left <= (double)right;
                case BANG_EQUAL:
                    return !IsEqual(left, right);
                case EQUAL_EQUAL:
                    return IsEqual(left, right);
                default:
                    return null;
            }
        }

        public object VisitConditionalExpr(Expr.Conditional expr)
        {
            if (IsTruthy(Evaluate(expr.ifExpr)))
            {
                return Evaluate(expr.thenBranch);
            }
            else
            {
                return Evaluate(expr.elseBranch);
            }
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

            switch (expr.op.Type)
            {
                case MINUS:
                    CheckNumberOperand(expr.op, right);
                    return -(double)right;
                case BANG:
                    return !IsTruthy(right);
                default:
                    return null;
            }
        }

        private object Evaluate(Expr expr)
        {
            return expr.Accept(this);
        }

        private static bool IsTruthy(object value)
        {
            // Use ruby like truthy semantics
            return value switch
            {
                bool b => b,
                null => false,
                _ => true,
            };
        }

        private static bool IsEqual(object a, object b)
        {
            if (a is null && b is null)
            {
                return true;
            }

            if (a is null)
            {
                return false;
            }

            return a.Equals(b);
        }

        private static void CheckNumberOperand(Token op, object operand)
        {
            if (operand is not double)
            {
                throw new RuntimeException(op, "Operand must be a number");
            }    
        }

        private static void CheckNumberOperands(Token op, object left, object right)
        {
            if (left is not double || right is not double)
            {
                throw new RuntimeException(op, "Operands must be numbers");
            }    
        }

        private static string Stringify(object value)
        {
            if (value is null)
            {
                return "nil";
            }

            if (value is double)
            {
                var text = value.ToString();
                if (text.EndsWith(".0"))
                {
                    text = text.Substring(0, text.Length - 2);
                }
                return text;
            }

            return value.ToString();
        }
    }
}
