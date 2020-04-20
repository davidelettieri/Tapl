using System;
using System.Collections.Generic;
using System.Text;

namespace Common
{
    public class NoRulesAppliesException : Exception
    {
        public NoRulesAppliesException(string message) : base(message)
        {
        }

        public NoRulesAppliesException(string message, Exception innerException) : base(message, innerException)
        {
        }

        public NoRulesAppliesException()
        {
        }
    }
}
