using System.Collections.Generic;

namespace MSW.Scripting
{
    public class Call : Expression
    {
        public readonly string callee;
        // Needed for debugging purposes.
        public readonly MSWToken calleeToken;
        public readonly List<Expression> arguments;
        
        public Call(string callee, MSWToken calleeToken, List<Expression> arguments)
        {
            this.callee = callee;
            this.calleeToken = calleeToken;
            this.arguments = arguments;
        }

        public override object Accept(IMSWExpressionVisitor visitor)
        {
            return visitor.VisitCall(this);
        }
    }
}