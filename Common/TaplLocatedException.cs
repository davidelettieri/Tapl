using System;

namespace Common;

public sealed class TaplTypingException(IInfo info, string message) : Exception(message)
{
    public IInfo Info { get; } = info;
}