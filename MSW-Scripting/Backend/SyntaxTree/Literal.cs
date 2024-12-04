namespace MSW.Scripting
{
    public class Literal : Expression
    {
        public readonly object literal;
        public Literal(object literal)
        {
            this.literal = literal;
        }

        public override object Accept(IMSWVisitor visitor)
        {
            return visitor.VisitLiteral(this);
        }
    }
}
