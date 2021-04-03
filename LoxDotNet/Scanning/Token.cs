namespace LoxDotNet.Scanning
{
    class Token
    {
        internal TokenType _type;
        internal string _lexeme;
        internal object _literal;
        internal int _line;

        internal Token(TokenType type, string lexeme, object literal, int line)
        {
            _type = type;
            _lexeme = lexeme;
            _literal = literal;
            _line = line;
        }

        public override string ToString()
        {
            return $"{_type} {_lexeme} {_literal}";
        }
    }
}
