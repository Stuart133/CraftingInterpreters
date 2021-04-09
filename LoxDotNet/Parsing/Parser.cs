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

        internal Parser(List<Token> tokens)
        {
            _tokens = tokens;
        }

        internal Expr Parse()
        {
            try
            {
                return Expression();
            }
            catch (ParseException)
            { 
                return null; 
            }
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

        private Expr Expression()
        {
            return Comma();
        }

        private Expr Comma()
        {
            return BinaryExpr(Conditional, COMMA);
        }

        private Expr Conditional()
        {
            var ifExpr = Equality();

            if (Match(QUESTION))
            {
                var thenExpr = Expression();
                Consume(COLON, "Expect ':' after then branch of conditional expression.");
                var elseExpr = Conditional();
                return new Expr.Conditional(ifExpr, thenExpr, elseExpr);
            }

            return ifExpr;
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

            return Primary();
        }

        private Expr Primary()
        {
            if (Match(FALSE)) return new Expr.Literal(false);
            if (Match(TRUE)) return new Expr.Literal(true);
            if (Match(NULL)) return new Expr.Literal(null);

            if (Match(NUMBER, STRING))
            {
                return new Expr.Literal(Previous().Literal);
            }

            if (Match(LEFT_PAREN))
            {
                var expr = Expression();
                Consume(RIGHT_PAREN, "Expect ')' after expression.");
                return new Expr.Grouping(expr);
            }

            return null;
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
    }
}
