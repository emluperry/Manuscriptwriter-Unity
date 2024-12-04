namespace MSW.Scripting
{
    public abstract class Expression
    {
        public abstract object Accept(IMSWVisitor visitor);
    }
}
