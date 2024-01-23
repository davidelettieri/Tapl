using System;

namespace Common;

public class ParameterTypeMismatchException : Exception
{
    public ParameterTypeMismatchException(string message) : base(message)
    {
    }

    public ParameterTypeMismatchException(string message, Exception innerException) : base(message, innerException)
    {
    }

    public ParameterTypeMismatchException()
    {
    }
}