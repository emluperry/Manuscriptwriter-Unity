using System;
using System.Collections.Generic;

namespace MSW.Scripting
{
    public class MSWInterpreter : IMSWExpressionVisitor, IMSWStatementVisitor
    {
        public Action<MSWRuntimeException> ReportRuntimeError;

        private Environment environment = new Environment();

        public object Interpret(IEnumerable<Statement> statements)
        {
            try
            {
                foreach (Statement s in statements)
                {
                    this.Execute(s);
                }
            }
            catch(MSWRuntimeException e)
            {
                ReportRuntimeError?.Invoke(e);
            }

            return "Failed to run interpretation.";
        }

        private object Evaluate(Expression expr)
        {
            return expr.Accept(this);
        }

        private void Execute(Statement statement)
        {
            statement.Accept(this);
        }

        private void ExecuteBlock(IEnumerable<Statement> statements, Environment blockEnvironment)
        {
            Environment previous = this.environment;

            try
            {
                this.environment = blockEnvironment;

                foreach(Statement statement in statements)
                {
                    this.Execute(statement);
                }
            }
            finally
            {
                this.environment = previous;
            }
        }

        private bool IsTrue(object obj)
        {
            if(obj == null)
            {
                return false;
            }

            if(obj is bool b)
            {
                return b;
            }

            return true;
        }

        private bool IsEqual(object a, object b)
        {
            if(a == null && b == null)
            {
                return true;
            }

            if(a == null)
            {
                return false;
            }

            return a.Equals(b);
        }
        
        #region ERRORS + VALIDATION

        private void ValidateNumberOperand(MSWToken op, object operand)
        {
            if (operand is double)
            {
                return;
            }

            throw new MSWRuntimeException(op, $"Operand {(operand != null ? operand.ToString() : string.Empty)} must be a number.");
        }
        #endregion

        #region Expression Visitors
        public object VisitAssignment(Assign visitor)
        {
            object value = this.Evaluate(visitor.value);
            environment.Assign(visitor.token, value);
            return value;
        }

        public object VisitVariable(Variable visitor)
        {
            return environment.Get(visitor.token);
        }

        public object VisitLiteral(Literal expr)
        {
            return expr.literal;
        }

        public object VisitGrouping(Grouping expr)
        {
            return this.Evaluate(expr.expression);
        }

        public object VisitBinary(Binary visitor)
        {
            object left = this.Evaluate(visitor.left);
            object right = this.Evaluate(visitor.right);

            switch(visitor.op.type)
            {
                // ARITHMETIC
                case MSWTokenType.MINUS:
                    this.ValidateNumberOperand(visitor.op, left);
                    this.ValidateNumberOperand(visitor.op, right);
                    return (double)left - (double)right;
                case MSWTokenType.DIVIDE:
                    this.ValidateNumberOperand(visitor.op, left);
                    this.ValidateNumberOperand(visitor.op, right);
                    return (double)left / (double)right;
                case MSWTokenType.MULTIPLY:
                    this.ValidateNumberOperand(visitor.op, left);
                    this.ValidateNumberOperand(visitor.op, right);
                    return (double)left * (double)right;
                case MSWTokenType.PLUS:
                    if(left is double ld && right is double rd)
                    {
                        return ld + rd;
                    }

                    if(left is string ls && right is string rs)
                    {
                        return ls + rs;
                    }

                    throw new MSWRuntimeException(visitor.op, "Operands must be two numbers or two strings.");

                // COMPARISON
                case MSWTokenType.GREATER:
                    this.ValidateNumberOperand(visitor.op, left);
                    this.ValidateNumberOperand(visitor.op, right);
                    return (double)left > (double)right;
                case MSWTokenType.GREATER_EQUAL:
                    this.ValidateNumberOperand(visitor.op, left);
                    this.ValidateNumberOperand(visitor.op, right);
                    return (double)left >= (double)right;
                case MSWTokenType.LESS:
                    this.ValidateNumberOperand(visitor.op, left);
                    this.ValidateNumberOperand(visitor.op, right);
                    return (double)left < (double)right;
                case MSWTokenType.LESS_EQUAL:
                    this.ValidateNumberOperand(visitor.op, left);
                    this.ValidateNumberOperand(visitor.op, right);
                    return (double)left <= (double)right;

                // EQUALITY
                case MSWTokenType.EQUAL_EQUAL:
                    return this.IsEqual(left, right);
                case MSWTokenType.NOT_EQUAL:
                    return !this.IsEqual(left, right);
            }

            return null;
        }

        public object VisitUnary(Unary visitor)
        {
            object right = this.Evaluate(visitor.right);

            switch(visitor.op.type)
            {
                case MSWTokenType.NOT:
                    return !IsTrue(right);
                case MSWTokenType.MINUS:
                    this.ValidateNumberOperand(visitor.op, right);
                    return -(double)right;
            }

            return null;
        }

        public object VisitLogical(Logical visitor)
        {
            object left = this.Evaluate(visitor.left);

            if(visitor.op.type == MSWTokenType.OR)
            {
                if(this.IsTrue(left))
                {
                    return left;
                }
            }
            else
            {
                if(!this.IsTrue(left))
                {
                    return left;
                }
            }

            return this.Evaluate(visitor.right);
        }
        #endregion

        #region STATEMENT VISITORS
        public object VisitExpression(StatementExpression visitor)
        {
            this.Evaluate(visitor.expression);
            return null;
        }

        public object VisitPrint(Print visitor)
        {
            object value = this.Evaluate(visitor.expression);
            Console.WriteLine(value != null ? value.ToString() : "Null");
            return null;
        }

        public object VisitVar(VarDeclaration visitor)
        {
            object value = null;
            if(visitor.initialiser != null)
            {
                value = Evaluate(visitor.initialiser);
            }

            environment.Define(visitor.token.lexeme, value);
            return null;
        }

        public object VisitBlock(Block visitor)
        {
            this.ExecuteBlock(visitor.statements, new Environment(environment));
            return null;
        }

        public object VisitIfBlock(If visitor)
        {
            if(this.IsTrue(this.Evaluate(visitor.condition)))
            {
                this.Execute(visitor.thenBranch);
            }
            else if(visitor.elseBranch != null)
            {
                this.Execute(visitor.elseBranch);
            }

            return null;
        }

        public object VisitWhileBlock(While visitor)
        {
            while(this.IsTrue(this.Evaluate(visitor.condition)))
            {
                this.Execute(visitor.statement);
            }

            return null;
        }
        #endregion
    }
}
