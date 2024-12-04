namespace MSW.Scripting
{
    public interface IMSWVisitor
    {
        object VisitBinary(Binary visitor);
        object VisitUnary(Unary visitor);
        object VisitLiteral(Literal visitor);
        object VisitGrouping(Grouping visitor);
    }
}
