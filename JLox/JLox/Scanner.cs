using System.Collections.Generic;

namespace JLox
{
    internal class Scanner
    {
        private readonly string _source;
        private readonly List<Token> _tokens = new();
        private int _start = 0; // points to the first character in the lexeme being scanned.
        private int _current = 0;   // points to the character currently being considered.
        private int _line = 1;  // the line number that we are currently on.

        private static readonly Dictionary<string, TokenType> _keywords = new()
        {
            { "and",        TokenType.AND },
            { "class",      TokenType.CLASS },
            { "else",       TokenType.ELSE },
            { "false",      TokenType.FALSE },
            { "for",        TokenType.FOR },
            { "fun",        TokenType.FUN },
            { "if",         TokenType.IF },
            { "nil",        TokenType.NIL },
            { "or",         TokenType.OR },
            { "print",      TokenType.PRINT },
            { "return",     TokenType.RETURN },
            { "super",      TokenType.SUPER },
            { "this",       TokenType.THIS },
            { "true",       TokenType.TRUE },
            { "var",        TokenType.VAR },
            { "while",      TokenType.WHILE }
        };

        public Scanner(string source)
        {
            _source = source;
        }

        public List<Token> ScanTokens()
        {
            while (!IsAtEnd())
            {
                // We are at the beginning of the next lexeme.
                _start = _current;
                ScanToken();
            }

            _tokens.Add(new Token(TokenType.EOF, string.Empty, null, _line));
            return _tokens;
        }

        private bool IsAtEnd()
        {
            return _current >= _source.Length;
        }

        private void ScanToken()
        {
            char c = Advance();
            switch (c)
            {
                case '(': AddToken(TokenType.LEFT_PAREN); break;
                case ')': AddToken(TokenType.RIGHT_PAREN); break;
                case '{': AddToken(TokenType.LEFT_BRACE); break;
                case '}': AddToken(TokenType.RIGHT_BRACE); break;
                case ',': AddToken(TokenType.COMMA); break;
                case '.': AddToken(TokenType.DOT); break;
                case '-': AddToken(TokenType.MINUS); break;
                case '+': AddToken(TokenType.PLUS); break;
                case ';': AddToken(TokenType.SEMICOLON); break;
                case '*': AddToken(TokenType.STAR); break;
                case '!':
                    AddToken(Match('=') ? TokenType.BANG_EQUAL : TokenType.BANG);
                    break;
                case '=':
                    AddToken(Match('=') ? TokenType.EQUAL_EQUAL : TokenType.EQUAL);
                    break;
                case '<':
                    AddToken(Match('=') ? TokenType.LESS_EQUAL : TokenType.LESS);
                    break;
                case '>':
                    AddToken(Match('=') ? TokenType.GREATER_EQUAL : TokenType.GREATER);
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
                    else
                    {
                        AddToken(TokenType.SLASH);
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
                case '"':
                    HandleString();
                    break;

                default:
                    if (char.IsDigit(c))
                    {
                        HandleNumber();
                    }
                    else if (char.IsLetter(c))
                    {
                        HandleIdentifier();
                    }
                    else
                    {
                        Lox.Error(_line, $"Unexpected character '{c}'");
                    }
                    break;
            }
        }

        private char Advance() => _source[_current++];

        private void AddToken(TokenType type)
        {
            AddToken(type, null);
        }

        private void AddToken(TokenType type, object? literal)
        {
            string text = _source[_start.._current];
            _tokens.Add(new Token(type, text, literal, _line));
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

            _current++;
            return true;
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

        private void HandleString()
        {
            while (Peek() != '"' && !IsAtEnd())
            {
                if (Peek() == '\n')
                {
                    _line++;
                }
                Advance();
            }

            if (IsAtEnd())
            {
                Lox.Error(_line, "Unterminated string.");
                return;
            }

            // The closing ".
            Advance();

            // Trim the surrounding quotes.
            string value = _source[(_start + 1)..(_current - 1)];
            AddToken(TokenType.STRING, value);
        }

        private void HandleNumber()
        {
            while (char.IsDigit(Peek()))
            {
                Advance();
            }

            // Look for a fractional part.
            if (Peek() == '.' && char.IsDigit(PeekNext()))
            {
                // Consume the "."
                Advance();

                while (char.IsDigit(Peek()))
                {
                    Advance();
                }
            }

            AddToken(TokenType.NUMBER, double.Parse(_source[_start.._current]));
        }

        private void HandleIdentifier()
        {
            while (char.IsLetterOrDigit(Peek()))
            {
                Advance();
            }

            string text = _source[_start.._current];
            if (!_keywords.TryGetValue(text, out TokenType type))
            {
                type = TokenType.IDENTIFIER;
            }

            AddToken(type);
        }
    }
}
