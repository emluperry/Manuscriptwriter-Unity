namespace MSW.Scripting
{
    public class Grouping : Expression
    {
        public readonly Expression expression;
        public Grouping(Expression exp)
        {
            this.expression = exp;
        }

        public override object Accept(IMSWVisitor visitor)
        {
            return visitor.VisitGrouping(this);
        }
    }
}
