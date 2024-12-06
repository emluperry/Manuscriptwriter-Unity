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

        public override object Accept(IMSWExpressionVisitor visitor)
        {
            return visitor.VisitUnary(this);
        }
    }
}
