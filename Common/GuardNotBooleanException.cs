using System;
using System.Collections.Generic;
using System.Text;

namespace Common
{

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
}
