using System;
using System.Collections.Generic;
using System.Text;

namespace Common
{
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
}
