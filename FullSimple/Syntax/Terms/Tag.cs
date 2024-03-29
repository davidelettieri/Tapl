﻿using Common;

namespace FullSimple.Syntax.Terms;

public class Tag : ITerm
{
    public IInfo Info { get; }
    public string Label { get; }
    public ITerm Term { get; }
    public IType Type { get; }

    public Tag(IInfo info, string label, ITerm term, IType type)
    {
            Info = info;
            Label = label;
            Term = term;
            Type = type;
        }

    public override string ToString()
    {
            return $"TmTag({Label},{Term},{Type})";
        }
}