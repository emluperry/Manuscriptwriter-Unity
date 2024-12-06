using System.Collections.Generic;

namespace MSW.Scripting
{
    public class Block : Statement
    {
        public readonly List<Statement> statements;
        public Block(List<Statement> statements)
        {
            this.statements = statements;
        }

        public override object Accept(IMSWStatementVisitor visitor)
        {
            return visitor.VisitBlock(this);
        }
    }
}
