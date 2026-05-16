using System;
using System.Collections.Generic;
using Common;

namespace FullUpdate.Parser;

public enum TokenKind
{
    // Keywords
    Type, Inert, If, Then, Else, True, False, Bool, As, Lambda, Let, In, Fix,
    Letrec, All, Some, Unit, UUnit, TimesFlt, UFloat, Succ, Pred, IsZero, Nat,
    UString, Star, TTop, Import,
    // Punctuation/operators
    Arrow, DArrow, DDArrow, Colon, ColonColon, ColonEq, Eq, EqEq,
    Dot, Semi, Comma, Slash, Bang, Hash, Triangle, VBar,
    LParen, RParen, LCurly, RCurly, LSquare, RSquare, Lt, Gt,
    LeftArrow, LCurlyBar, LSquareBar, RCurlyBar, BarGt, BarRSquare,
    Apostrophe, DQuote, Uscore,
    // Literals
    IntV, FloatV, StringV,
    // Identifiers
    UCid, LCid,
    // Special
    Eof, LEq
}

public sealed class Token
{
    public TokenKind Kind { get; }
    public string Text { get; }
    public int Line { get; }
    public int Column { get; }

    public Token(TokenKind kind, string text, int line, int col)
    {
        Kind = kind; Text = text; Line = line; Column = col;
    }

    public IInfo Info => new Common.FileInfo(Text, Line, Column);
    public override string ToString() => $"{Kind}({Text}) at {Line}:{Column}";
}

public sealed class Lexer
{
    private readonly string _src;
    private int _pos;
    private int _line = 1;
    private int _col = 1;

    private static readonly Dictionary<string, TokenKind> Keywords = new()
    {
        ["type"] = TokenKind.Type,
        ["inert"] = TokenKind.Inert,
        ["if"] = TokenKind.If,
        ["then"] = TokenKind.Then,
        ["else"] = TokenKind.Else,
        ["true"] = TokenKind.True,
        ["false"] = TokenKind.False,
        ["Bool"] = TokenKind.Bool,
        ["as"] = TokenKind.As,
        ["lambda"] = TokenKind.Lambda,
        ["let"] = TokenKind.Let,
        ["in"] = TokenKind.In,
        ["fix"] = TokenKind.Fix,
        ["letrec"] = TokenKind.Letrec,
        ["All"] = TokenKind.All,
        ["Some"] = TokenKind.Some,
        ["unit"] = TokenKind.Unit,
        ["Unit"] = TokenKind.UUnit,
        ["timesfloat"] = TokenKind.TimesFlt,
        ["Float"] = TokenKind.UFloat,
        ["succ"] = TokenKind.Succ,
        ["pred"] = TokenKind.Pred,
        ["iszero"] = TokenKind.IsZero,
        ["Nat"] = TokenKind.Nat,
        ["String"] = TokenKind.UString,
        ["Top"] = TokenKind.TTop,
        ["import"] = TokenKind.Import,
    };

    public Lexer(string src) { _src = src; }

    private char Peek(int offset = 0) => _pos + offset < _src.Length ? _src[_pos + offset] : '\0';
    private char Advance()
    {
        var c = _src[_pos++];
        if (c == '\n') { _line++; _col = 1; } else { _col++; }
        return c;
    }

    public List<Token> Tokenize()
    {
        var tokens = new List<Token>();
        while (true)
        {
            SkipWhitespace();
            if (_pos >= _src.Length) { tokens.Add(new Token(TokenKind.Eof, "", _line, _col)); break; }
            var tok = NextToken();
            if (tok != null) tokens.Add(tok);
        }
        return tokens;
    }

    private void SkipWhitespace()
    {
        while (_pos < _src.Length && char.IsWhiteSpace(_src[_pos])) Advance();
        // Skip line comments starting with /*
        if (_pos + 1 < _src.Length && _src[_pos] == '/' && _src[_pos + 1] == '*')
        {
            Advance(); Advance();
            while (_pos + 1 < _src.Length && !(_src[_pos] == '*' && _src[_pos + 1] == '/'))
                Advance();
            if (_pos + 1 < _src.Length) { Advance(); Advance(); }
            SkipWhitespace();
        }
    }

    private Token? NextToken()
    {
        int startLine = _line, startCol = _col;
        char c = _src[_pos];

        // String literal
        if (c == '"')
        {
            Advance();
            var sb = new System.Text.StringBuilder();
            while (_pos < _src.Length && _src[_pos] != '"') sb.Append(Advance());
            if (_pos < _src.Length) Advance(); // closing "
            return new Token(TokenKind.StringV, sb.ToString(), startLine, startCol);
        }

        // Operators / punctuation
        Token? op = TryOperator(startLine, startCol);
        if (op != null) return op;

        // Numbers
        if (char.IsDigit(c))
        {
            var sb = new System.Text.StringBuilder();
            while (_pos < _src.Length && char.IsDigit(_src[_pos])) sb.Append(Advance());
            if (_pos < _src.Length && _src[_pos] == '.')
            {
                sb.Append(Advance());
                while (_pos < _src.Length && char.IsDigit(_src[_pos])) sb.Append(Advance());
                return new Token(TokenKind.FloatV, sb.ToString(), startLine, startCol);
            }
            return new Token(TokenKind.IntV, sb.ToString(), startLine, startCol);
        }

        // Identifiers / keywords
        if (char.IsLetter(c) || c == '_')
        {
            var sb = new System.Text.StringBuilder();
            while (_pos < _src.Length && (char.IsLetterOrDigit(_src[_pos]) || _src[_pos] == '_' || _src[_pos] == '\''))
                sb.Append(Advance());
            var text = sb.ToString();
            if (Keywords.TryGetValue(text, out var kw))
                return new Token(kw, text, startLine, startCol);
            // UCID starts with upper, LCID starts with lower or _
            var kind = char.IsUpper(text[0]) ? TokenKind.UCid : TokenKind.LCid;
            return new Token(kind, text, startLine, startCol);
        }

        // Unknown - skip
        Advance();
        return null;
    }

    private Token? TryOperator(int line, int col)
    {
        char c = _src[_pos];
        char n = Peek(1);

        // Two/three-char first
        if (c == '=' && n == '=' && Peek(2) == '>') { Advance(); Advance(); Advance(); return new Token(TokenKind.DDArrow, "==>", line, col); }
        if (c == '=' && n == '=') { Advance(); Advance(); return new Token(TokenKind.EqEq, "==", line, col); }
        if (c == '=' && n == '>') { Advance(); Advance(); return new Token(TokenKind.DArrow, "=>", line, col); }
        if (c == ':' && n == ':') { Advance(); Advance(); return new Token(TokenKind.ColonColon, "::", line, col); }
        if (c == ':' && n == '=') { Advance(); Advance(); return new Token(TokenKind.ColonEq, ":=", line, col); }
        if (c == '-' && n == '>') { Advance(); Advance(); return new Token(TokenKind.Arrow, "->", line, col); }
        if (c == '<' && n == ':') { Advance(); Advance(); return new Token(TokenKind.LEq, "<:", line, col); }
        if (c == '<' && n == '-') { Advance(); Advance(); return new Token(TokenKind.LeftArrow, "<-", line, col); }
        if (c == '{' && n == '|') { Advance(); Advance(); return new Token(TokenKind.LCurlyBar, "{|", line, col); }
        if (c == '[' && n == '|') { Advance(); Advance(); return new Token(TokenKind.LSquareBar, "[|", line, col); }
        if (c == '|' && n == '}') { Advance(); Advance(); return new Token(TokenKind.RCurlyBar, "|}", line, col); }
        if (c == '|' && n == '>') { Advance(); Advance(); return new Token(TokenKind.BarGt, "|>", line, col); }
        if (c == '|' && n == ']') { Advance(); Advance(); return new Token(TokenKind.BarRSquare, "|]", line, col); }

        // Single char
        return c switch
        {
            '.' => Tok(TokenKind.Dot, "."),
            ';' => Tok(TokenKind.Semi, ";"),
            ',' => Tok(TokenKind.Comma, ","),
            '/' => Tok(TokenKind.Slash, "/"),
            ':' => Tok(TokenKind.Colon, ":"),
            '=' => Tok(TokenKind.Eq, "="),
            '(' => Tok(TokenKind.LParen, "("),
            ')' => Tok(TokenKind.RParen, ")"),
            '{' => Tok(TokenKind.LCurly, "{"),
            '}' => Tok(TokenKind.RCurly, "}"),
            '[' => Tok(TokenKind.LSquare, "["),
            ']' => Tok(TokenKind.RSquare, "]"),
            '<' => Tok(TokenKind.Lt, "<"),
            '>' => Tok(TokenKind.Gt, ">"),
            '!' => Tok(TokenKind.Bang, "!"),
            '#' => Tok(TokenKind.Hash, "#"),
            '$' => Tok(TokenKind.Triangle, "$"),
            '*' => Tok(TokenKind.Star, "*"),
            '|' => Tok(TokenKind.VBar, "|"),
            '\'' => Tok(TokenKind.Apostrophe, "'"),
            '_' => Tok(TokenKind.Uscore, "_"),
            _ => null
        };

        Token Tok(TokenKind k, string text) { Advance(); return new Token(k, text, line, col); }
    }
}
