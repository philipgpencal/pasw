using System;

namespace PASW.Util.Exceptions
{
    public class PASWException : Exception
    {
        public PASWException(string error): base(error)
        {

        }
    }
}
