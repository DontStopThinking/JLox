using System.Collections.Generic;

namespace JLox.Scanning;

internal class Scanner
{
    private readonly string _source;
    private readonly List<Token> _tokens = new();
    private int _start; // points to the first character in the lexeme being scanned.
    private int _current;   // points to the character currently being considered.
    private int _line = 1;  // the line number that we are currently on.

    private static readonly Dictionary<string, TokenType> s_keywords = new()
    {
        { "and",        TokenType.And },
        { "class",      TokenType.Class },
        { "else",       TokenType.Else },
        { "false",      TokenType.False },
        { "for",        TokenType.For },
        { "fun",        TokenType.Fun },
        { "if",         TokenType.If },
        { "nil",        TokenType.Nil },
        { "or",         TokenType.Or },
        { "print",      TokenType.Print },
        { "return",     TokenType.Return },
        { "super",      TokenType.Super },
        { "this",       TokenType.This },
        { "true",       TokenType.True },
        { "var",        TokenType.Var },
        { "while",      TokenType.While }
    };

    public Scanner(string source)
    {
        _source = source;
        _current = 0;
    }

    public List<Token> ScanTokens()
    {
        while (!IsAtEnd())
        {
            // We are at the beginning of the next lexeme.
            _start = _current;
            ScanToken();
        }

        _tokens.Add(new Token(TokenType.Eof, string.Empty, null, _line));
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
            case '(': AddToken(TokenType.LeftParen); break;
            case ')': AddToken(TokenType.RightParen); break;
            case '{': AddToken(TokenType.LeftBrace); break;
            case '}': AddToken(TokenType.RightBrace); break;
            case ',': AddToken(TokenType.Comma); break;
            case '.': AddToken(TokenType.Dot); break;
            case '-': AddToken(TokenType.Minus); break;
            case '+': AddToken(TokenType.Plus); break;
            case ';': AddToken(TokenType.Semicolon); break;
            case '*': AddToken(TokenType.Star); break;
            case '!':
                // If next character is '=' then add '!=' otherwise just add '!'
                AddToken(Match('=') ? TokenType.BangEqual : TokenType.Bang);
                break;
            case '=':
                AddToken(Match('=') ? TokenType.EqualEqual : TokenType.Equal);
                break;
            case '<':
                AddToken(Match('=') ? TokenType.LessEqual : TokenType.Less);
                break;
            case '>':
                AddToken(Match('=') ? TokenType.GreaterEqual : TokenType.Greater);
                break;
            case '/':
                if (Match('/'))
                {
                    // Since this is a line comment, keep advancing until the end of line
                    while (Peek() != '\n' && !IsAtEnd())
                    {
                        Advance();
                    }
                }
                else if (Match('*'))
                {
                    HandleBlockComment();
                }
                else
                {
                    AddToken(TokenType.Slash);
                }
                break;
            case '\n':
                _line++;
                break;
            case '"':
                HandleString();
                break;
            case ' ': case '\r': case '\t': break;    // Ignore whitespace (i.e. ' ', '\r', '\t')
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

    private void AddToken(TokenType type, object? literal = null)
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

    /// <summary>
    /// Does a lookahead. Like <see cref="Advance"/>, but doesn't consume the character.
    /// </summary>
    private char Peek()
    {
        return IsAtEnd() ? '\0' : _source[_current];
    }

    private char PeekNext()
    {
        return _current + 1 >= _source.Length ? '\0' : _source[_current + 1];
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

        // The closing '"'
        Advance();

        // Trim the surrounding quotes.
        string value = _source[(_start + 1)..(_current - 1)];
        AddToken(TokenType.String, value);
    }

    private void HandleBlockComment()
    {
        while (Peek() != '*' && PeekNext() != '/' && !IsAtEnd())
        {
            Advance();
        }

        if (IsAtEnd())
        {
            return;
        }

        // The closing '*/'
        Advance();
        Advance();
    }

    private void HandleNumber()
    {
        while (char.IsDigit(Peek()))
        {
            Advance();
        }

        // Look for a fractional part
        if (Peek() == '.' && char.IsDigit(PeekNext()))
        {
            // Consume the "."
            Advance();

            while (char.IsDigit(Peek()))
            {
                Advance();
            }
        }

        AddToken(TokenType.Number, double.Parse(_source[_start.._current]));
    }

    private void HandleIdentifier()
    {
        while (char.IsLetterOrDigit(Peek()))
        {
            Advance();
        }

        string text = _source[_start.._current];

        if (!s_keywords.TryGetValue(text, out TokenType type))
        {
            type = TokenType.Identifier;
        }

        AddToken(type);
    }
}
