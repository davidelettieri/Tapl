﻿using Common;

namespace FullSimple.Syntax.Terms;

public class StringTerm : ITerm
{
    public string Value { get; }
    public StringTerm(string value)
    {
            Value = value;
        }

    public override string ToString()
    {
            return $"TmString({Value})";
        }
}