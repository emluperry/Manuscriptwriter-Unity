namespace MSW.Scripting
{
    public interface IMSWExpressionVisitor
    {
        object VisitBinary(Binary visitor);
        object VisitUnary(Unary visitor);
        object VisitLiteral(Literal visitor);
        object VisitGrouping(Grouping visitor);

        object VisitVariable(Variable visitor);
        object VisitAssignment(Assign visitor);
    }
}
