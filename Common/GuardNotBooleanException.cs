using System;

namespace Common;

public class GuardNotBooleanException : Exception
{
    public GuardNotBooleanException(string message) : base(message)
    {
    }

    public GuardNotBooleanException(string message, Exception innerException) : base(message, innerException)
    {
    }

    public GuardNotBooleanException()
    {
    }
}