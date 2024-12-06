namespace MSW.Scripting
{
    public class Binary : Expression
    {
        public readonly Expression left;
        public readonly MSWToken op;
        public readonly Expression right;
        public Binary(Expression left, MSWToken op, Expression right)
        {
            this.left = left;
            this.op = op;
            this.right = right;
        }

        public override object Accept(IMSWExpressionVisitor visitor)
        {
            return visitor.VisitBinary(this);
        }
    }
}
