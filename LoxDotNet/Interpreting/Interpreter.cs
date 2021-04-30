using LoxDotNet.Parsing;
using LoxDotNet.Scanning;
using System;
using System.Collections.Generic;
using static LoxDotNet.Scanning.TokenType;

namespace LoxDotNet.Interpreting
{
    public class Interpreter : Expr.IVisitor<object>, Stmt.IVisitor<object>
    {
        internal Environment Globals { get; } = new Environment();
        private Environment _environment;
        private readonly Dictionary<Expr, int> _locals = new Dictionary<Expr, int>();

        // Setinal value
        private static readonly object _uninitialized = new object();

        public Interpreter()
        {
            _environment = Globals;

            Globals.Define("clock", new Clock());
        }

        public void Interpret(List<Stmt> statements)
        {
            try
            {
                foreach(var statement in statements)
                {
                    Execute(statement);
                }
            }
            catch (RuntimeException ex)
            {
                Lox.RuntimeError(ex);
            }
        }

        public object VisitAssignExpr(Expr.Assign expr)
        {
            var value = Evaluate(expr.value);

            var valuePresent = _locals.TryGetValue(expr, out var distance);
            if (valuePresent)
            {
                _environment.AssignAt(distance, expr.name, value);
            }
            else
            {
                Globals.Assign(expr.name, value);
            }

            return value;
        }

        public object VisitBinaryExpr(Expr.Binary expr)
        {
            var left = Evaluate(expr.left);
            var right = Evaluate(expr.right);

            switch (expr.op.Type)
            {
                case PLUS:
                    return StringOrDouble((l, r) => l + r, (l, r) => l + r, left, right, expr.op);
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
                case COMMA:
                    return right;
                default:
                    return null;
            }
        }

        public object VisitCallExpr(Expr.Call expr)
        {
            var callee = Evaluate(expr.callee);

            var args = new List<object>();
            foreach (var arg in expr.arguments)
            {
                args.Add(Evaluate(arg));
            }
            
            if (callee is ICallable function)
            {
                if (args.Count != function.Arity())
                {
                    throw new RuntimeException(expr.paren, $"Expected {function.Arity()} arguments but got {args.Count}.");
                }

               return function.Call(this, args);
            }

            throw new RuntimeException(expr.paren, "Can only call functions and classes");
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

        public object VisitFunctionExpr(Expr.Function expr)
        {
            return new LoxFunction(null, expr, _environment, false);
        }

        public object VisitGetExpr(Expr.Get expr)
        {
            var obj = Evaluate(expr.obj);
            if (obj is LoxInstance li)
            {
                return li.Get(expr.name);
            }

            throw new RuntimeException(expr.name, "Only instances have properties");
        }

        public object VisitGroupingExpr(Expr.Grouping expr)
        {
            return Evaluate(expr.expression);
        }

        public object VisitLiteralExpr(Expr.Literal expr)
        {
            return expr.value;
        }

        public object VisitLogicalExpr(Expr.Logical expr)
        {
            var left = Evaluate(expr.left);

            if (expr.op.Type == OR)
            {
                if (IsTruthy(left))
                {
                    return left;
                }
            }
            else
            {
                if (!IsTruthy(left))
                {
                    return left;
                }
            }

            return Evaluate(expr.right);
        }

        public object VisitSetExpr(Expr.Set expr)
        {
            var obj = Evaluate(expr.obj);

            if (obj is LoxInstance li)
            {
                var value = Evaluate(expr.value);
                li.Set(expr.name, value);
                return value;
            }

            throw new RuntimeException(expr.name, "Only instances have fields.");
        }

        public object VisitThisExpr(Expr.This expr)
        {
            return LookUpVariable(expr.keyword, expr);
        }

        public object VisitVariableExpr(Expr.Variable expr)
        {
            return LookUpVariable(expr.name, expr);
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

        public object VisitBlockStmt(Stmt.Block stmt)
        {
            ExecuteBlock(stmt.statements, new Environment(_environment));
            return null;
        }

        public object VisitClassStmt(Stmt.Class stmt)
        {
            object superclass = null;
            if (stmt.superclass is not null)
            {
                superclass = Evaluate(stmt.superclass);
                if (superclass is not LoxClass)
                {
                    throw new RuntimeException(stmt.superclass.name, "Superclass must be a class.");
                }
            }

            _environment.Define(stmt.name.Lexeme, null);

            var methods = new Dictionary<string, LoxFunction>();
            foreach (var method in stmt.methods)
            {
                var function = new LoxFunction(stmt.name, method.function, _environment, method.name.Lexeme == "init");
                methods[method.name.Lexeme] = function;
            }
          
            var clas = new LoxClass(stmt.name.Lexeme, methods);
            _environment.Assign(stmt.name, clas);

            return null;
        }

        public object VisitBreakStmt(Stmt.Break stmt)
        {
            throw new BreakException();
        }

        public object VisitExpressionStmt(Stmt.Expression stmt)
        {
            Evaluate(stmt.expression);
            return null;
        }

        public object VisitFunctionStmt(Stmt.Function stmt)
        {
            var function = new LoxFunction(stmt.name, stmt.function, _environment, false);
            _environment.Define(stmt.name.Lexeme, function);
            return null;
        }

        public object VisitIfStmt(Stmt.If stmt)
        {
            if (IsTruthy(Evaluate(stmt.condition)))
            {
                Execute(stmt.thenBranch);
            }
            else if (stmt.elseBranch is not null)
            {
                Execute(stmt.elseBranch);
            }

            return null;
        }

        public object VisitPrintStmt(Stmt.Print stmt)
        {
            var value = Evaluate(stmt.expression);
            Console.WriteLine(Stringify(value));
            return null;
        }

        public object VisitReturnStmt(Stmt.Return stmt)
        {
            object value = null;
            if (stmt.value is not null)
            {
                value = Evaluate(stmt.value);
            }

            throw new Return(value);
        }

        public object VisitVarStmt(Stmt.Var stmt)
        {
            if (stmt.initializer is not null)
            {
                var value = Evaluate(stmt.initializer);
                _environment.Define(stmt.name.Lexeme, value);
            }
            else
            {
                _environment.Define(stmt.name.Lexeme, _uninitialized);
            }

            return null;
        }

        public object VisitWhileStmt(Stmt.While stmt)
        {
            while (IsTruthy(Evaluate(stmt.condition)))
            {
                try
                {
                    Execute(stmt.body);
                }
                catch (BreakException)
                {
                    break;
                }
            }

            return null;
        }

        private void Execute(Stmt stmt)
        {
            stmt.Accept(this);
        }

        internal void ExecuteBlock(List<Stmt> statements, Environment environment)
        {
            var previous = _environment;
            try
            {
                _environment = environment;
                foreach (var stmt in statements)
                {
                    Execute(stmt);
                }
            }
            finally
            {
                _environment = previous;
            }
        }

        internal void Resolve(Expr expr, int depth)
        {
            _locals[expr] = depth;
        }

        private object Evaluate(Expr expr)
        {
            return expr.Accept(this);
        }

        private object LookUpVariable(Token name, Expr expr)
        {
            var valuePresent = _locals.TryGetValue(expr, out var distance);
            if (valuePresent)
            {
                return _environment.GetAt(distance, name.Lexeme);
            }
            else
            {
                return Globals.Get(name);
            }
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

        private static object StringOrDouble(Func<double, double, object> doubleFunc, Func<string, string, object> stringFunc, object left, object right, Token op)
        {
            if (left is double ld && right is double rd)
            {
                return doubleFunc(ld, rd);
            }

            if (left is string ls && right is string rs)
            {
                return stringFunc(ls, rs);
            }

            throw new RuntimeException(op, "Operands must be two numbers or two strings");
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
