﻿using System.Collections.Generic;
using static LoxDotNet.Scanning.TokenType;

namespace LoxDotNet.Scanning
{
    class Scanner
    {
        private readonly string _source;
        private readonly List<Token> _tokens = new List<Token>();
        private int _start = 0;
        private int _current = 0;
        private int _line = 1;

        internal Scanner(string source)
        {
            _source = source;
        }

        internal List<Token> ScanTokens()
        {
            while (!IsAtEnd())
            {
                // We are at the beginning of the next lexeme
                _start = _current;
                ScanToken();
            }

            _tokens.Add(new Token(EOF, "", null, _line));
            return _tokens;
        }

        private void ScanToken()
        {
            var c = Advance();
            switch (c)
            {
                case '(': AddToken(LEFT_PAREN); break;
                case ')': AddToken(RIGHT_PAREN); break;
                case '{': AddToken(LEFT_BRACE); break;
                case '}': AddToken(RIGHT_BRACE); break;
                case ',': AddToken(COMMA); break;
                case '.': AddToken(DOT); break;
                case '-': AddToken(MINUS); break;
                case '+': AddToken(PLUS); break;
                case ';': AddToken(SEMICOLON); break;
                case '*': AddToken(STAR); break;
                case ':': AddToken(COLON); break;
                case '?': AddToken(QUESTION); break;
                case '!':
                    AddToken(Match('=') ? BANG_EQUAL : BANG);
                    break;
                case '=':
                    AddToken(Match('=') ? EQUAL_EQUAL : EQUAL);
                    break;
                case '<':
                    AddToken(Match('=') ? LESS_EQUAL : LESS);
                    break;
                case '>':
                    AddToken(Match('=') ? GREATER_EQUAL : GREATER);
                    break;
                case '/':
                    if (Match('/'))
                    {
                        // A comment goes until the end of the line
                        while (Peek() != '\n' && !IsAtEnd())
                        {
                            Advance();
                        }
                    }
                    else if (Match('*'))
                    {
                        BlockComment();
                    }
                    else
                    {
                        AddToken(SLASH);
                    }
                    break;
                
                case ' ':
                case '\r':
                case '\t':
                    // Ignore whitespace
                    break;

                case '\n':
                    _line++;
                    break;

                case '"': String(); break;

                default:
                    if (IsDigit(c))
                    {
                        Number();
                    }
                    else if (IsAlpha(c))
                    {
                        Identifier();
                    }
                    else
                    {
                        Lox.Error(_line, "Unexpected character.");
                    }
                    break;
            }
        }

        private bool IsAtEnd()
        {
            return _current >= _source.Length;
        }

        private bool IsDigit(char c)
        {
            return c >= '0' && c <= '9';
        }

        private bool IsAlpha(char c)
        {
            return (c >= 'a' && c <= 'z') ||
                (c >= 'A' && c <= 'Z') ||
                c == '_';
        }

        private bool IsAlphaNumeric(char c)
        {
            return IsAlpha(c) || IsDigit(c);
        }

        private char Advance()
        {
            return _source[_current++];
        }

        private char Peek()
        {
            if (IsAtEnd())
            {
                return '\0';
            }

            return _source[_current];
        }

        private char PeekNext()
        {
            if (_current + 1 >= _source.Length)
            {
                return '\0';
            }

            return _source[_current + 1];
        }

        private bool Match(char expected)
        {
            if (IsAtEnd())
            {
                return false;
            }

            if (_source[_current] != expected)
            {
                return false;
            }

            // Make sure to advance if the character matches
            _current++;
            return true;
        }

        private void AddToken(TokenType type)
        {
            AddToken(type, null);
        }

        private void AddToken(TokenType type, object literal)
        {
            var text = _source[_start.._current];
            _tokens.Add(new Token(type, text, literal, _line));
        }

        private void BlockComment()
        {
            // For now don't handle nested block comments
            while (Peek() != '*' && PeekNext() != '/' && !IsAtEnd())
            {
                // Increment the line if we hit a newline char
                if (Peek() == '\n')
                {
                    _line++;
                }

                Advance();
            }

            if (IsAtEnd())
            {
                Lox.Error(_line, "Unclosed block comment");
                return;
            }

            // Advance over the final */
            Advance();
            Advance();
        }

        private void String()
        {
            while (Peek() != '"' && !IsAtEnd())
            {
                if (Peek() == '\n' && !IsAtEnd())
                {
                    _line++;
                }
                Advance();
            }

            if (IsAtEnd())
            {
                Lox.Error(_line, "Unterminated string");
                return;
            }

            // Advance over the closing "
            Advance();

            // Trim the quotes
            var value = _source[(_start + 1)..(_current - 1)];
            AddToken(STRING, value);
        }

        private void Number()
        {
            // Keep going until the next character is not a digit
            while (IsDigit(Peek()))
            {
                Advance();
            }

            if (Peek() == '.' && IsDigit(PeekNext()))
            {
                // Consume the . and continue to the end of the number
                Advance();
                while (IsDigit(Peek()))
                {
                    Advance();
                }
            }

            AddToken(NUMBER, double.Parse(_source[_start.._current]));
        }

        private void Identifier()
        {
            while (IsAlphaNumeric(Peek()))
            {
                Advance();
            }

            var text = _source[_start.._current];
            var reserverd = ReservedIdentifiers.Identifiers.TryGetValue(text, out var type);
            if (!reserverd)
            {
                type = IDENTIFIER;
            }

            AddToken(type);
        }
    }
}
