using System.Collections.Generic;

namespace MSW.Scripting
{
    public class Block : Statement
    {
        public readonly IEnumerable<Statement> statements;
        public Block(IEnumerable<Statement> statements)
        {
            this.statements = statements;
        }

        public override object Accept(IMSWStatementVisitor visitor)
        {
            return visitor.VisitBlock(this);
        }
    }
}
