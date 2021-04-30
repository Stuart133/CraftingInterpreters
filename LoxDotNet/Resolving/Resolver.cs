using LoxDotNet.Interpreting;
using LoxDotNet.Parsing;
using LoxDotNet.Scanning;
using System.Collections.Generic;
using System.Linq;

namespace LoxDotNet.Resolving
{
    class Resolver : Expr.IVisitor<object>, Stmt.IVisitor<object>
    {
        private readonly Interpreter _interpreter;
        private readonly Stack<Dictionary<string, Variable>> _scopes = new Stack<Dictionary<string, Variable>>();
        private FunctionType _currentFunction = FunctionType.None;
        private ClassType _currentClass = ClassType.None;

        internal Resolver(Interpreter interpreter)
        {
            _interpreter = interpreter;
        }

        public void Resolve(List<Stmt> statements)
        {
            foreach (var statement in statements)
            {
                Resolve(statement);
            }
        }

        public object VisitAssignExpr(Expr.Assign expr)
        {
            Resolve(expr.value);
            ResolveLocal(expr, expr.name, false);

            return null;
        }

        public object VisitBinaryExpr(Expr.Binary expr)
        {
            Resolve(expr.left);
            Resolve(expr.right);

            return null;
        }

        public object VisitBlockStmt(Stmt.Block stmt)
        {
            BeginScope();
            Resolve(stmt.statements);
            EndScope();

            return null;
        }

        public object VisitClassStmt(Stmt.Class stmt)
        {
            var enclosingClass = _currentClass;
            _currentClass = ClassType.Class;

            Declare(stmt.name);
            Define(stmt.name);

            if (stmt.superclass is not null)
            {
                if (stmt.name.Lexeme == stmt.superclass.name.Lexeme)
                {
                    Lox.Error(stmt.superclass.name, "A class can't inherit from itself.");
                }

                Resolve(stmt.superclass);

                BeginScope();
                _scopes.Peek()["super"] = new Variable(stmt.name, VariableState.Read);
            }            

            BeginScope();
            _scopes.Peek()["this"] = new Variable(stmt.name, VariableState.Read);

            foreach (var method in stmt.methods)
            {
                var declaration = FunctionType.Method;
                if (method.name.Lexeme == "init")
                {
                    declaration = FunctionType.Initializer;
                }

                ResolveFunction(method.function, declaration);
            }

            EndScope();

            if (stmt.superclass is not null)
            {
                EndScope();
            }

            _currentClass = enclosingClass;

            return null;
        }

        public object VisitBreakStmt(Stmt.Break stmt)
        {
            return null;
        }

        public object VisitCallExpr(Expr.Call expr)
        {
            Resolve(expr.callee);

            foreach (var arg in expr.arguments)
            {
                Resolve(arg);
            }

            return null;
        }

        public object VisitConditionalExpr(Expr.Conditional expr)
        {
            throw new System.NotImplementedException();
        }

        public object VisitExpressionStmt(Stmt.Expression stmt)
        {
            Resolve(stmt.expression);

            return null;
        }

        public object VisitGetExpr(Expr.Get expr)
        {
            Resolve(expr.obj);

            return null;
        }

        public object VisitFunctionExpr(Expr.Function expr)
        {
            ResolveFunction(expr, FunctionType.Function);

            return null;
        }

        public object VisitFunctionStmt(Stmt.Function stmt)
        {
            Declare(stmt.name);
            Define(stmt.name);

            Resolve(stmt.function);

            return null;
        }

        public object VisitGroupingExpr(Expr.Grouping expr)
        {
            Resolve(expr.expression);

            return null;
        }

        public object VisitIfStmt(Stmt.If stmt)
        {
            Resolve(stmt.condition);
            Resolve(stmt.thenBranch);

            if (stmt.elseBranch != null)
            {
                Resolve(stmt.elseBranch);
            }

            return null;
        }

        public object VisitLiteralExpr(Expr.Literal expr)
        {
            return null;
        }

        public object VisitLogicalExpr(Expr.Logical expr)
        {
            Resolve(expr.left);
            Resolve(expr.right);

            return null;
        }

        public object VisitPrintStmt(Stmt.Print stmt)
        {
            Resolve(stmt.expression);

            return null;
        }

        public object VisitReturnStmt(Stmt.Return stmt)
        {
            if (_currentFunction == FunctionType.None)
            {
                Lox.Error(stmt.keyword, "Can't return from top-level code");
            }

            if (_currentFunction == FunctionType.Initializer && stmt.value is not null)
            {
                Lox.Error(stmt.keyword, "Can't return a value from an initializer.");
            }

            if (stmt.value != null)
            {
                Resolve(stmt.value);
            }

            return null;
        }

        public object VisitSetExpr(Expr.Set expr)
        {
            Resolve(expr.value);
            Resolve(expr.obj);

            return null;
        }

        public object VisitSuperExpr(Expr.Super expr)
        {
            ResolveLocal(expr, expr.keyword, true);

            return null;
        }

        public object VisitThisExpr(Expr.This expr)
        {
            if (_currentClass == ClassType.None)
            {
                Lox.Error(expr.keyword, "Can't use 'this' keyword outside of a class");
            }

            ResolveLocal(expr, expr.keyword, true);
            return null;
        }

        public object VisitUnaryExpr(Expr.Unary expr)
        {
            Resolve(expr.right);

            return null;
        }

        public object VisitVariableExpr(Expr.Variable expr)
        {
            if (_scopes.Count != 0 && _scopes.Peek().TryGetValue(expr.name.Lexeme, out var value) && value.State == VariableState.Declared)
            {
                Lox.Error(expr.name, "Can't read local variable in its own initializer.");
            }

            ResolveLocal(expr, expr.name, true);

            return null;
        }

        public object VisitVarStmt(Stmt.Var stmt)
        {
            Declare(stmt.name);
            if (stmt.initializer != null)
            {
                Resolve(stmt.initializer);
            }

            Define(stmt.name);
            return null;
        }

        public object VisitWhileStmt(Stmt.While stmt)
        {
            Resolve(stmt.condition);
            Resolve(stmt.body);

            return null;
        }

        private void BeginScope()
        {
            _scopes.Push(new Dictionary<string, Variable>());
        }

        private void EndScope()
        {
            var scope = _scopes.Pop();
        }

        private void Resolve(Stmt stmt)
        {
            stmt.Accept(this);
        }

        private void Resolve(Expr expr)
        {
            expr.Accept(this);
        }

        private void ResolveLocal(Expr expr, Token name, bool isRead)
        {
            for (var i = _scopes.Count - 1; i >= 0; i--)
            {
                if (_scopes.ElementAt(i).ContainsKey(name.Lexeme))
                {
                    _interpreter.Resolve(expr, i);

                    if (isRead)
                    {
                        _scopes.ElementAt(i)[name.Lexeme].State = VariableState.Read;
                    }

                    return;
                }
            }
        }

        private void ResolveFunction(Expr.Function function, FunctionType functionType)
        {
            var enclosingFunction = _currentFunction;
            _currentFunction = functionType;

            BeginScope();
            foreach (var parameter in function.parameters)
            {
                Declare(parameter);
                Define(parameter);
            }

            Resolve(function.body);
            EndScope();

            _currentFunction = enclosingFunction;
        }

        private void Declare(Token name)
        {
            if (_scopes.Count == 0)
            {
                return;
            }

            var scope = _scopes.Peek();
            if (scope.ContainsKey(name.Lexeme))
            {
                Lox.Error(name, "Already variable with this name in scope.");
            }

            scope[name.Lexeme] = new Variable(name, VariableState.Declared);
        }

        private void Define(Token name)
        {
            if (_scopes.Count == 0)
            {
                return;
            }

            _scopes.Peek()[name.Lexeme].State = VariableState.Defined;
        }
    }
}
