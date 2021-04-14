using LoxDotNet.Scanning;
using System;
using System.Collections.Generic;
using static LoxDotNet.Scanning.TokenType;

namespace LoxDotNet.Parsing
{
    class Parser
    {
        private readonly List<Token> _tokens;
        private int _current = 0;
        private int _loopDepth = 0;

        internal Parser(List<Token> tokens)
        {
            _tokens = tokens;
        }

        internal List<Stmt> Parse()
        {
            var statements = new List<Stmt>();

            while (!IsAtEnd())
            {
                statements.Add(Declaration(0));
            }

            return statements;
        }

        private Stmt Declaration(int loopDepth)
        {
            try
            {
                if (Match(VAR))
                {
                    return VarDeclaration();
                }

                return Statement(loopDepth);
            }
            catch (ParseException)
            {
                Synchronize();
                return null;
            }
        }

        private Stmt VarDeclaration()
        {
            var name = Consume(IDENTIFIER, "Expect variable name");

            Expr initializer = null;
            if (Match(EQUAL))
            {
                initializer = Expression();
            }

            Consume(SEMICOLON, "Expect ';' after variable declaration");
            return new Stmt.Var(name, initializer);
        }

        private Stmt Statement(int loopDepth)
        {
            if (Match(FOR))
            {
                return ForStatement(loopDepth);
            }

            if (Match(IF))
            {
                return IfStatement(loopDepth);
            }

            if (Match(PRINT))
            {
                return PrintStatement();
            }

            if (Match(WHILE))
            {
                return WhileStatement(loopDepth);
            }

            if (Match(BREAK))
            {
                return BreakStatement(loopDepth);
            }

            if (Match(LEFT_BRACE))
            {
                return new Stmt.Block(Block(loopDepth));
            }

            return ExpressionStatement();
        }

        private Stmt ForStatement(int loopDepth)
        {
            Consume(LEFT_PAREN, "Expect '(' after 'for'.");

            Stmt initializer;
            if (Match(SEMICOLON))
            {
                initializer = null;
            }
            else if (Match(VAR))
            {
                initializer = VarDeclaration();
            }
            else
            {
                initializer = ExpressionStatement();
            }

            // Default condition to true if none is supplied
            Expr condition = new Expr.Literal(true);
            if (!Check(SEMICOLON))
            {
                condition = Expression();
            }
            Consume(SEMICOLON, "Expect ';' after loop condition.");

            Expr increment = null;
            if (!Check(RIGHT_PAREN))
            {
                increment = Expression();
            }
            Consume(RIGHT_PAREN, "Expect ')' after for clauses.");

            var body = Statement(loopDepth + 1);

            if (increment is not null)
            {
                body = new Stmt.Block(new List<Stmt> { body, new Stmt.Expression(increment) });
            }

            body = new Stmt.While(condition, body);

            if (initializer is not null)
            {
                body = new Stmt.Block(new List<Stmt> { initializer, body });
            }

            return body;
        }

        private Stmt IfStatement(int loopDepth)
        {
            Consume(LEFT_PAREN, "Expect '(' after 'if'.");
            var condition = Expression();
            Consume(RIGHT_PAREN, "Expect ')' after if condition");

            var thenBranch = Statement(loopDepth);
            Stmt elseBranch = null;

            if (Match(ELSE))
            {
                elseBranch = Statement(loopDepth);
            }

            return new Stmt.If(condition, thenBranch, elseBranch);
        }

        private Stmt ExpressionStatement()
        {
            var expr = Expression();
            Consume(SEMICOLON, "Expect ';' after expression");
            return new Stmt.Expression(expr);
        }

        private Stmt PrintStatement()
        {
            var value = Expression();
            Consume(SEMICOLON, "Expect ';' after value");
            return new Stmt.Print(value);
        }

        private Stmt WhileStatement(int loopDepth)
        {
            Consume(LEFT_PAREN, "Expect '(' after 'while'.");
            var condition = Expression();
            Consume(RIGHT_PAREN, "Expect ')' after condition.");

            var body = Statement(loopDepth + 1);
            return new Stmt.While(condition, body);
        }

        private Stmt BreakStatement(int loopDepth)
        {
            if (loopDepth == 0)
            {
                Error(Previous(), "Must be inside a loop to use 'break'.");
            }

            Consume(SEMICOLON, "Expect ';' after 'break'.");

            return new Stmt.Break();
        }

        private List<Stmt> Block(int loopDepth)
        {
            var statements = new List<Stmt>();

            while (!Check(RIGHT_BRACE) && !IsAtEnd())
            {
                statements.Add(Declaration(loopDepth));
            }

            Consume(RIGHT_BRACE, "Expect '}' after block.");
            return statements;
        }

        private Expr Expression()
        {
            return Comma();
        }

        private Expr Comma()
        {
            return BinaryExpr(Assignment, COMMA);
        }

        private Expr Assignment()
        {
            var expr = Conditional();

            if (Match(EQUAL))
            {
                var equals = Previous();
                var value = Assignment();

                if (expr is Expr.Variable varExpr)
                {
                    var name = varExpr.name;
                    return new Expr.Assign(name, value);
                }

                Error(equals, "Invalid assignment target");
            }

            return expr;
        }

        private Expr Conditional()
        {
            var ifExpr = Or();

            if (Match(QUESTION))
            {
                var thenExpr = Expression();
                Consume(COLON, "Expect ':' after then branch of conditional expression.");
                var elseExpr = Conditional();
                return new Expr.Conditional(ifExpr, thenExpr, elseExpr);
            }

            return ifExpr;
        }

        private Expr Or()
        {
            return LogicalExpr(And, OR);
        }

        private Expr And()
        {
            return LogicalExpr(Equality, AND);
        }

        private Expr Equality()
        {
            return BinaryExpr(Comparison, BANG_EQUAL, EQUAL_EQUAL);
        }

        private Expr Comparison()
        {
            return BinaryExpr(Term, GREATER, GREATER_EQUAL, LESS, LESS_EQUAL);
        }

        private Expr Term()
        {
            return BinaryExpr(Factor, MINUS, PLUS);
        }

        private Expr Factor()
        {
            return BinaryExpr(Unary, SLASH, STAR);
        }

        private Expr Unary()
        {
            if (Match(BANG, MINUS))
            {
                var op = Previous();
                var right = Unary();
                return new Expr.Unary(op, right);
            }

            return Call();
        }

        private Expr Call()
        {
            var expr = Primary();

            while (true)
            {
                if (Match(LEFT_PAREN))
                {
                    expr = FinishCall(expr);
                }
                else
                {
                    break;
                }
            }

            return expr;
        }

        private Expr FinishCall(Expr callee)
        {
            var args = new List<Expr>();
            if (!Check(RIGHT_PAREN))
            {
                do
                {
                    if (args.Count >= 255)
                    {
                        Error(Peek(), "Can't have more than 255 arguments.");
                    }

                    args.Add(Expression());
                }
                while (Match(COMMA));
            }

            var paren = Consume(RIGHT_PAREN, "Expect ')' after arguments.");

            return new Expr.Call(callee, paren, args);
        }

        private Expr Primary()
        {
            if (Match(FALSE)) return new Expr.Literal(false);
            if (Match(TRUE)) return new Expr.Literal(true);
            if (Match(NIL)) return new Expr.Literal(null);

            if (Match(NUMBER, STRING))
            {
                return new Expr.Literal(Previous().Literal);
            }

            if (Match(IDENTIFIER))
            {
                return new Expr.Variable(Previous());
            }

            if (Match(LEFT_PAREN))
            {
                var expr = Expression();
                Consume(RIGHT_PAREN, "Expect ')' after expression.");
                return new Expr.Grouping(expr);
            }

            // Error productions
            if (Match(BANG_EQUAL, EQUAL_EQUAL))
            {
                Error(Previous(), "Missing left hand operand");
                Equality();
                return null;
            }
            if (Match(GREATER, GREATER_EQUAL, LESS, LESS_EQUAL))
            {
                Error(Previous(), "Missing left hand operand");
                Comparison();
                return null;
            }
            if (Match(PLUS))
            {
                Error(Previous(), "Missing left hand operand");
                Term();
                return null;
            }
            if (Match(SLASH, STAR))
            {
                Error(Previous(), "Missing left hand operand");
                Factor();
                return null;
            }

            throw Error(Peek(), "Expected expression");
        }

        private Expr BinaryExpr(Func<Expr> operandMethod, params TokenType[] matchTokenTypes)
        {
            var expr = operandMethod();

            while (Match(matchTokenTypes))
            {
                var op = Previous();
                var right = operandMethod();
                expr = new Expr.Binary(expr, op, right);
            }

            return expr;
        }

        private Expr LogicalExpr(Func<Expr> operandMethod, params TokenType[] matchTokenTypes)
        {
            var expr = operandMethod();

            while (Match(matchTokenTypes))
            {
                var op = Previous();
                var right = operandMethod();
                expr = new Expr.Logical(expr, op, right);
            }

            return expr;
        }

        private bool Match(params TokenType[] types)
        {
            foreach (var tokenType in types)
            {
                if (Check(tokenType))
                {
                    Advance();
                    return true;
                }
            }

            return false;
        }

        private bool Check(TokenType type)
        {
            if (IsAtEnd())
            {
                return false;
            }

            return Peek().Type == type;
        }

        private Token Advance()
        {
            if (!IsAtEnd())
            {
                _current++;
            }

            return Previous();
        }

        private Token Peek()
        {
            return _tokens[_current];
        }

        private Token Previous()
        {
            return _tokens[_current - 1];
        }

        private Token Consume(TokenType type, string message)
        {
            if (Check(type))
            {
                return Advance();
            }

            throw Error(Peek(), message);
        }

        private bool IsAtEnd()
        {
            return Peek().Type == EOF;
        }

        private static ParseException Error(Token token, string message)
        {
            Lox.Error(token, message);
            return new ParseException();
        }

        private void Synchronize()
        {
            Advance();

            // Advance until we find what's probably the next statement
            while (!IsAtEnd())
            {
                if (Previous().Type == SEMICOLON)
                {
                    return;
                }

                switch (Peek().Type)
                {
                    case CLASS:
                    case FUN:
                    case VAR:
                    case FOR:
                    case IF:
                    case WHILE:
                    case PRINT:
                    case RETURN:
                        return;
                }

                Advance();
            }
        }
    }
}
