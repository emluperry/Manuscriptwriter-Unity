namespace MSW.Scripting
{
    internal class Print : Statement
    {
        public readonly Expression expression;

        public Print(Expression expression)
        {
            this.expression = expression;
        }

        public override object Accept(IMSWStatementVisitor visitor)
        {
            return visitor.VisitPrint(this);
        }
    }
}
