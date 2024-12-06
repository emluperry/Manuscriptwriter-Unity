namespace MSW.Scripting
{
    public class Variable : Expression
    {
        public readonly MSWToken token;
        public Variable(MSWToken token)
        {
            this.token = token;
        }

        public override object Accept(IMSWExpressionVisitor visitor)
        {
            return visitor.VisitVariable(this);
        }
    }
}
