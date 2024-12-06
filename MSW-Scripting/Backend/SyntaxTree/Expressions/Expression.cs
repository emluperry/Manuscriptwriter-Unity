namespace MSW.Scripting
{
    public abstract class Expression
    {
        public abstract object Accept(IMSWExpressionVisitor visitor);
    }
}
