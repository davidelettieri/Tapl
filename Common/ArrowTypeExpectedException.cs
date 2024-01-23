using System;

namespace Common;

public class ArrowTypeExpectedException : Exception
{
    public ArrowTypeExpectedException(string message) : base(message)
    {
    }

    public ArrowTypeExpectedException(string message, Exception innerException) : base(message, innerException)
    {
    }

    public ArrowTypeExpectedException()
    {
    }
}