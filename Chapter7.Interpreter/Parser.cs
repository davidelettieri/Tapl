using System;
using System.Collections.Generic;
using System.Text;

namespace Chapter7.Interpreter
{
    public class Parser
    {
        private readonly List<Token> _tokens;
        private int current = 0;

        Parser(List<Token> tokens)
        {
            _tokens = tokens;
        }
    }
}
