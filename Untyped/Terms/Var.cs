using Common;
using System;

namespace Untyped.Terms;

public class Var : ITerm
{
    public int Index { get; }
    public int ContextLength { get; }

    /// <summary>
    /// Variable term
    /// </summary>
    /// <param name="index">De bruijn index</param>
    /// <param name="contextLength">Context length</param>
    public Var(int index, int contextLength)
    {
        if (contextLength < index)
            throw new ArgumentOutOfRangeException(nameof(index), "Index cannot be greater than context length");

        Index = index;
        ContextLength = contextLength;
    }
}