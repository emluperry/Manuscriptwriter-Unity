namespace MSW.Scripting
{
    public interface IMSWStatementVisitor
    {
        object VisitExpression(StatementExpression visitor);
        object VisitPrint(Print visitor);

        object VisitVar(VarDeclaration visitor);
    }
}
