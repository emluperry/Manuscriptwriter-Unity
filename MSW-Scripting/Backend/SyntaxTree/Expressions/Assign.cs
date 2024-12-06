namespace MSW.Scripting
{
    public class Assign : Expression
    {
        public readonly MSWToken token;
        public readonly Expression value;

        public Assign(MSWToken token, Expression value)
        {
            this.token = token;
            this.value = value;
        }

        public override object Accept(IMSWExpressionVisitor visitor)
        {
            return visitor.VisitAssignment(this);
        }
    }
}
