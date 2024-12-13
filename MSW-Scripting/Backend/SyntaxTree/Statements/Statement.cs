namespace MSW.Scripting
{
    internal abstract class Statement
    {
        public abstract object Accept(IMSWStatementVisitor visitor);
    }
}
