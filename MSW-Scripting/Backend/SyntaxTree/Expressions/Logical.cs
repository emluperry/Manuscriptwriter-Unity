namespace MSW.Scripting
{
    public class Logical : Expression
    {
        public readonly Expression left;
        public readonly MSWToken op;
        public readonly Expression right;

        public Logical(Expression left, MSWToken op, Expression right)
        {
            this.left = left;
            this.op = op;
            this.right = right;
        }

        public override object Accept(IMSWExpressionVisitor visitor)
        {
            return visitor.VisitLogical(this);
        }
    }
}
