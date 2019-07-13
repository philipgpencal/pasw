using System;
using PASW.Util.Extensions;

namespace PASW.Util.Exceptions
{
    public class PASWException : Exception
    {
        public PASWException(ExceptionType exceptionType) : base (exceptionType.Description())
        {

        }
    }
}
