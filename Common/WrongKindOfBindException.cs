using System;

namespace Common;

public class WrongKindOfBindException : Exception
{
    public WrongKindOfBindException(string message) : base(message)
    {
    }

    public WrongKindOfBindException(string message, Exception innerException) : base(message, innerException)
    {
    }

    public WrongKindOfBindException()
    {
    }
}