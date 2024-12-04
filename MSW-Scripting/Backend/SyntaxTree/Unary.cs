namespace MSW.Scripting
{
    public class Unary : Expression
    {
        public readonly MSWToken op;
        public readonly Expression right;
        public Unary(MSWToken op, Expression right)
        {
            this.op = op;
            this.right = right;
        }

        public override object Accept(IMSWVisitor visitor)
        {
            return visitor.VisitUnary(this);
        }
    }
}
