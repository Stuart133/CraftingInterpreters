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

        private ParseError Error(Token token, string message)
        {
            Lox.Error(token, message);
            return new ParseError();
        }

        private Expr Expression()
        {
            return Equality();
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
