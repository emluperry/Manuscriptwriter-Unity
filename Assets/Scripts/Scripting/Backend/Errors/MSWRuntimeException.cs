using System;

namespace MSW.Scripting
{
    public class MSWRuntimeException : SystemException
    {
        public readonly MSWToken operatorToken;
        public MSWRuntimeException(MSWToken token, string message) : base(message)
        {
            this.operatorToken = token;
        }
    }
}
