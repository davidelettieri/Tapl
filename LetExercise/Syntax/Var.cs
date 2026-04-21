using System;
using Common;

namespace LetExercise.Syntax;

/// <summary>
/// Variable term
/// </summary>
/// <param name="index">De bruijn index</param>
/// <param name="contextLength">Context length</param>
public class Var(IInfo info, int index, int contextLength) : ITerm
{
    public IInfo Info { get; } = info;
    public int Index { get; } = contextLength >= index ? index : throw new InvalidOperationException();
    public int ContextLength { get; } = contextLength;
}