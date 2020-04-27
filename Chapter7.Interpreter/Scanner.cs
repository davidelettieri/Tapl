using System;
using System.Collections.Generic;

namespace Chapter7.Interpreter
{
    internal class Scanner
    {
        private readonly string _source;
        private readonly List<Token> _tokens = new List<Token>();
        private int _start = 0;
        private int _current = 0;
        private int _line = 1;
        public Scanner(string source)
        {
            _source = source;
        }

        internal List<Token> ScanTokens()
        {
            while (!IsAtEnd())
            {
                _start = _current;
                ScanToken();
            }

            _tokens.Add(new Token(TokenType.EOF, "", null, _line));
            return _tokens;
        }

        private void ScanToken()
        {
            var c = Advance();

            switch (c)
            {
                case '(': AddToken(TokenType.OP); break;
                case ')': AddToken(TokenType.CP); break;
                case '.': AddToken(TokenType.DOT); break;
                case '\\': AddToken(TokenType.LAMBDA); break;
                case ';': AddToken(TokenType.SEMICOLON); break;
                case '/': AddToken(TokenType.BIND); break;

                default:
                    if (IsAlpha(c))
                    {
                        Identifier();
                    }
                    else
                    {
                        Program.Error(_line, "Unexpected character.");
                    }
                    break;
            }

        }

        private void Identifier()
        {
            AddToken(TokenType.VAR);
        }

        private bool IsAlpha(char c) => char.IsLetter(c);

        private char Advance()
        {
            _current++;
            return _source[_current - 1];
        }

        private void AddToken(TokenType type) => AddToken(type, null);

        private void AddToken(TokenType type, object literal)
        {
            var text = _source.Substring(_start, _current - _start);
            _tokens.Add(new Token(type, text, literal, _line));
        }

        private bool IsAtEnd()
        {
            return _current >= _source.Length;
        }
    }
}