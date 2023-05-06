using System.Collections.Generic;

namespace JLox.Scanning;

internal class Scanner
{
    private readonly string _source;
    private readonly List<Token> _tokens = new();
    private int start; // points to the first character in the lexeme being scanned.
    private int current;   // points to the character currently being considered.
    private int line = 1;  // the line number that we are currently on.

    private static readonly Dictionary<string, TokenType> Keywords = new()
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
        current = 0;
    }

    public List<Token> ScanTokens()
    {
        while (!IsAtEnd())
        {
            // We are at the beginning of the next lexeme.
            start = current;
            ScanToken();
        }

        _tokens.Add(new Token(TokenType.Eof, string.Empty, null, line));
        return _tokens;
    }

    private bool IsAtEnd()
    {
        return current >= _source.Length;
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
            case '*':
                AddToken(TokenType.Star); break;
            case '!':
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
                    // A comment goes until the end of the line
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
            case ' ':
            case '\r':
            case '\t':
                // Ignore whitespace
                break;
            case '\n':
                line++;
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
                    Lox.Error(line, $"Unexpected character '{c}'");
                }
                break;
        }
    }

    private char Advance() => _source[current++];

    private void AddToken(TokenType type, object? literal = null)
    {
        string text = _source[start..current];
        _tokens.Add(new Token(type, text, literal, line));
    }

    private bool Match(char expected)
    {
        if (IsAtEnd())
        {
            return false;
        }

        if (_source[current] != expected)
        {
            return false;
        }

        current++;
        return true;
    }

    private char Peek()
    {
        return IsAtEnd() ? '\0' : _source[current];
    }

    private char PeekNext()
    {
        return current + 1 >= _source.Length ? '\0' : _source[current + 1];
    }

    private void HandleString()
    {
        while (Peek() != '"' && !IsAtEnd())
        {
            if (Peek() == '\n')
            {
                line++;
            }
            Advance();
        }

        if (IsAtEnd())
        {
            Lox.Error(line, "Unterminated string.");
            return;
        }

        // The closing '"'
        Advance();

        // Trim the surrounding quotes.
        string value = _source[(start + 1)..(current - 1)];
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

        AddToken(TokenType.Number, double.Parse(_source[start..current]));
    }

    private void HandleIdentifier()
    {
        while (char.IsLetterOrDigit(Peek()))
        {
            Advance();
        }

        string text = _source[start..current];
        if (!Keywords.TryGetValue(text, out TokenType type))
        {
            type = TokenType.Identifier;
        }

        AddToken(type);
    }
}
