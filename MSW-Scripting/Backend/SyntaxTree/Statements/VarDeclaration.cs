namespace MSW.Scripting
{
    public class VarDeclaration : Statement
    {
        public readonly MSWToken token;
        public readonly Expression initialiser;

        public VarDeclaration(MSWToken token, Expression initialiser)
        {
            this.token = token;
            this.initialiser = initialiser;
        }

        public override object Accept(IMSWStatementVisitor visitor)
        {
            return visitor.VisitVar(this);
        }
    }
}