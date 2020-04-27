using System;
using System.Collections.Generic;
using System.Text;

namespace Chapter7.Interpreter
{
    public enum TokenType
    {
        // Single character tokens
        LAMBDA,DOT,VAR,OP,CP,EOF,SEMICOLON,

        // Multiple characters tokens
        BIND
    }
}
