namespace MSW.Scripting
{
    public abstract class Statement
    {
        public abstract object Accept(IMSWStatementVisitor visitor);
    }
}
