using System;
using Common;

namespace FullUntyped.Syntax.Terms;

public sealed class Var : ITerm
{
    public IInfo Info { get; }
    public int Index { get; }
    public int ContextLength { get; }

    /// <summary>
    /// Variable term
    /// </summary>
    /// <param name="index">De bruijn index</param>
    /// <param name="ctxl">Context length</param>
    public Var(IInfo info, int index, int ctxl)
    {
        if (ctxl < index)
            throw new InvalidOperationException();

        Info = info;
        Index = index;
        ContextLength = ctxl;
    }

    public override string ToString() => $"TmVar({Index},{ContextLength})";
}