using System;

namespace Common
{
    public class ArmsOfConditionalHaveDifferentTypesException : Exception
    {
        public ArmsOfConditionalHaveDifferentTypesException(string message) : base(message)
        {
        }

        public ArmsOfConditionalHaveDifferentTypesException(string message, Exception innerException) : base(message, innerException)
        {
        }

        public ArmsOfConditionalHaveDifferentTypesException()
        {
        }
    }
}
