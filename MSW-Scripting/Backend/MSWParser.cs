using System;
using System.Collections.Generic;

namespace MSW.Scripting
{
    public class MSWParser
    {
        public Action<MSWToken, string> ReportTokenError;

        public MSWParser()
        {

        }

        public List<Statement> Parse(List<MSWToken> tokens)
        {
            int finalIndex = tokens.Count;
            int currentIndex = 0;

            List<Statement> statements = new List<Statement>();
            while(currentIndex < finalIndex)
            {
                if (this.IsOfType(MSWTokenType.EOF, tokens, currentIndex, finalIndex))
                {
                    break;
                }

                var s = this.Declaration(tokens, ref currentIndex, finalIndex);

                if(s != null)
                {
                    statements.Add(s);
                }
            }

            return statements;
        }

        private MSWToken PopToken(List<MSWToken> tokens, ref int currentIndex, int finalIndex)
        {
            var token = tokens[currentIndex];
            if(currentIndex < finalIndex)
            {
                ++currentIndex;
            }

            return token;
        }

        private MSWToken PeekToken(List<MSWToken> tokens, int currentIndex, int finalIndex)
        {
            if (currentIndex >= finalIndex)
            {
                return null;
            }

            return tokens[currentIndex];
        }

        private MSWToken PeekPreviousToken(List<MSWToken> tokens, int currentIndex)
        {
            if(currentIndex - 1 < 0)
            {
                return null;
            }

            return tokens[currentIndex - 1];
        }

        private bool IsOneOfTypes(List<MSWTokenType> tokenTypes, List<MSWToken> tokens, ref int currentIndex, int finalIndex)
        {
            foreach (MSWTokenType tokenType in tokenTypes)
            { 
                if(this.IsOfType(tokenType, tokens, currentIndex, finalIndex))
                {
                    this.PopToken(tokens, ref currentIndex, finalIndex);
                    return true;
                }
            }

            return false;
        }

        private bool IsOfType(MSWTokenType tokenType, List<MSWToken> tokens, int currentIndex, int finalIndex)
        {
            if(currentIndex >= finalIndex)
            {
                return false;
            }

            return PeekToken(tokens, currentIndex, finalIndex).type == tokenType;
        }

        private MSWToken ConsumeToken(MSWTokenType type, string message, List<MSWToken> tokens, ref int currentIndex, int finalIndex)
        {
            if(this.IsOfType(type, tokens, currentIndex, finalIndex))
            {
                return this.PopToken(tokens, ref currentIndex, finalIndex);
            }

            throw this.ParseError(this.PeekToken(tokens, currentIndex, finalIndex), message);
        }

        private MSWToken ConsumeOneOfTokens(List<MSWTokenType> types, string message, List<MSWToken> tokens, ref int currentIndex, int finalIndex)
        {
            foreach (MSWTokenType tokenType in types)
            {
                if (this.IsOfType(tokenType, tokens, currentIndex, finalIndex))
                {
                    return this.PopToken(tokens, ref currentIndex, finalIndex);
                }
            }

            throw this.ParseError(this.PeekToken(tokens, currentIndex, finalIndex), message);
        }

        #region ERROR HANDLING
        private MSWParseException ParseError(MSWToken token, string message)
        {
            ReportTokenError?.Invoke(token, message);

            return new MSWParseException();
        }

        private void Synchronise(List<MSWToken> tokens, ref int currentIndex, int finalIndex)
        {
            this.PopToken(tokens, ref currentIndex, finalIndex);

            while(currentIndex < finalIndex)
            {
                switch(this.PeekToken(tokens, currentIndex, finalIndex).type)
                {
                    case MSWTokenType.END:
                    case MSWTokenType.EOL:
                        this.PopToken(tokens, ref currentIndex, finalIndex);
                        return;
                    case MSWTokenType.EOF:
                        return;
                }

                this.PopToken(tokens, ref currentIndex, finalIndex);
            }
        }
        #endregion

        #region EXPRESSIONS
        private Expression Expression(List<MSWToken> tokens, ref int currentIndex, int finalIndex)
        {
            return this.Assignment(tokens, ref currentIndex, finalIndex);
        }

        private Expression Assignment(List<MSWToken> tokens, ref int currentIndex, int finalIndex)
        {
            Expression expression = this.Or(tokens, ref currentIndex, finalIndex);

            if(this.IsOneOfTypes(new List<MSWTokenType> { MSWTokenType.EQUAL }, tokens, ref currentIndex, finalIndex))
            {
                MSWToken equals = this.PeekPreviousToken(tokens, currentIndex);
                Expression value = this.Assignment(tokens, ref currentIndex, finalIndex);

                if(expression is Variable var)
                {
                    MSWToken token = var.token;
                    return new Assign(token, value);
                }

                this.ParseError(equals, "Invalid assignment target.");
            }

            return expression;
        }

        private Expression Or(List<MSWToken> tokens, ref int currentIndex, int finalIndex)
        {
            Expression expression = this.And(tokens, ref currentIndex, finalIndex);

            while(this.IsOfType(MSWTokenType.OR, tokens, currentIndex, finalIndex))
            {
                MSWToken op = this.PopToken(tokens, ref currentIndex, finalIndex);
                Expression right = this.And(tokens, ref currentIndex, finalIndex);
                expression = new Logical(expression, op, right);
            }

            return expression;
        }

        private Expression And(List<MSWToken> tokens, ref int currentIndex, int finalIndex)
        {
            Expression expression = this.Equality(tokens, ref currentIndex, finalIndex);

            while (this.IsOfType(MSWTokenType.AND, tokens, currentIndex, finalIndex))
            {
                MSWToken op = this.PopToken(tokens, ref currentIndex, finalIndex);
                Expression right = this.Equality(tokens, ref currentIndex, finalIndex);
                expression = new Logical(expression, op, right);
            }

            return expression;
        }

        private Expression Equality(List<MSWToken> tokens, ref int currentIndex, int finalIndex)
        {
            Expression expression = this.Comparison(tokens, ref currentIndex, finalIndex);

            while(this.IsOneOfTypes(new List<MSWTokenType> { MSWTokenType.NOT_EQUAL, MSWTokenType.EQUAL_EQUAL }, tokens, ref currentIndex, finalIndex))
            {
                MSWToken op = this.PeekPreviousToken(tokens, currentIndex);
                Expression right = this.Comparison(tokens, ref currentIndex, finalIndex);
                expression = new Binary(expression, op, right);
            }

            return expression;
        }

        private Expression Comparison(List<MSWToken> tokens, ref int currentIndex, int finalIndex)
        {
            Expression expression = this.Term(tokens, ref currentIndex, finalIndex);

            while(this.IsOneOfTypes(new List<MSWTokenType> { MSWTokenType.GREATER, MSWTokenType.GREATER_EQUAL, MSWTokenType.LESS, MSWTokenType.LESS_EQUAL}, tokens, ref currentIndex, finalIndex))
            {
                MSWToken op = this.PeekPreviousToken(tokens, currentIndex);
                Expression right = this.Term(tokens, ref currentIndex, finalIndex);
                expression = new Binary(expression, op, right);
            }

            return expression;
        }

        private Expression Term(List<MSWToken> tokens, ref int currentIndex, int finalIndex)
        {
            Expression expression = this.Factor(tokens, ref currentIndex, finalIndex);

            while(this.IsOneOfTypes(new List<MSWTokenType> { MSWTokenType.MINUS, MSWTokenType.PLUS }, tokens, ref currentIndex, finalIndex))
            {
                MSWToken op = this.PeekPreviousToken(tokens, currentIndex);
                Expression right = this.Factor(tokens, ref currentIndex, finalIndex);
                expression = new Binary(expression, op, right);
            }

            return expression;
        }

        private Expression Factor(List<MSWToken> tokens, ref int currentIndex, int finalIndex)
        {
            Expression expression = this.Unary(tokens, ref currentIndex, finalIndex);

            while(this.IsOneOfTypes(new List<MSWTokenType> { MSWTokenType.MULTIPLY, MSWTokenType.DIVIDE }, tokens, ref currentIndex, finalIndex))
            {
                MSWToken op = this.PeekPreviousToken(tokens, currentIndex);
                Expression right = this.Unary(tokens, ref currentIndex, finalIndex);
                expression = new Binary(expression, op, right);
            }

            return expression;
        }

        private Expression Unary(List<MSWToken> tokens, ref int currentIndex, int finalIndex)
        {
            if(this.IsOneOfTypes(new List<MSWTokenType> { MSWTokenType.NOT, MSWTokenType.MINUS }, tokens, ref currentIndex, finalIndex))
            {
                MSWToken op = this.PeekPreviousToken(tokens, currentIndex);
                Expression right = this.Unary(tokens, ref currentIndex, finalIndex);
                return new Unary(op, right);
            }

            return this.Primary(tokens, ref currentIndex, finalIndex);
        }

        private Expression Primary(List<MSWToken> tokens, ref int currentIndex, int finalIndex)
        {
            if(this.IsOneOfTypes(new List<MSWTokenType> { MSWTokenType.FALSE }, tokens, ref currentIndex, finalIndex))
            {
                return new Literal(false);
            }
            if (this.IsOneOfTypes(new List<MSWTokenType> { MSWTokenType.TRUE }, tokens, ref currentIndex, finalIndex))
            {
                return new Literal(true);
            }
            if (this.IsOneOfTypes(new List<MSWTokenType> { MSWTokenType.NULL }, tokens, ref currentIndex, finalIndex))
            {
                return new Literal(null);
            }

            if (this.IsOneOfTypes(new List<MSWTokenType> { MSWTokenType.STRING, MSWTokenType.DOUBLE }, tokens, ref currentIndex, finalIndex))
            {
                return new Literal(this.PeekPreviousToken(tokens, currentIndex).literal);
            }

            if(this.IsOneOfTypes(new List<MSWTokenType> { MSWTokenType.IDENTIFIER }, tokens, ref currentIndex, finalIndex))
            {
                return new Variable(this.PeekPreviousToken(tokens, currentIndex));
            }

            if (this.IsOneOfTypes(new List<MSWTokenType> { MSWTokenType.LEFT_PARENTHESIS }, tokens, ref currentIndex, finalIndex))
            {
                Expression expression = this.Expression(tokens, ref currentIndex, finalIndex);
                this.ConsumeToken(MSWTokenType.RIGHT_PARENTHESIS, "Expect ')' after expression.", tokens, ref currentIndex, finalIndex);
                return new Grouping(expression);
            }

            if(this.IsOneOfTypes(new List<MSWTokenType> { MSWTokenType.EOL }, tokens, ref currentIndex, finalIndex))
            {
                return null;
            }

            throw ParseError(this.PeekToken(tokens, currentIndex, finalIndex), "Expect expression.");
        }

        #endregion

        #region STATEMENTS

        private Statement Declaration(List<MSWToken> tokens, ref int currentIndex, int finalIndex)
        {
            try
            {
                if (this.IsOfType(MSWTokenType.EOL, tokens, currentIndex, finalIndex))
                {
                    this.PopToken(tokens, ref currentIndex, finalIndex);
                }

                if (this.IsOfType(MSWTokenType.VAR, tokens, currentIndex, finalIndex))
                {
                    return this.VarDeclaration(tokens, ref currentIndex, finalIndex);
                }

                return this.Statement(tokens, ref currentIndex, finalIndex);
            }
            catch(MSWParseException)
            {
                Synchronise(tokens, ref currentIndex, finalIndex);
                return null;
            }
        }

        private Statement VarDeclaration(List<MSWToken> tokens, ref int currentIndex, int finalIndex)
        {
            this.PopToken(tokens, ref currentIndex, finalIndex);
            MSWToken token = this.ConsumeToken(MSWTokenType.IDENTIFIER, "Expect variable name.", tokens, ref currentIndex, finalIndex);

            Expression initialiser = null;
            if (this.IsOfType(MSWTokenType.EQUAL, tokens, currentIndex, finalIndex))
            {
                this.PopToken(tokens, ref currentIndex, finalIndex);
                initialiser = this.Expression(tokens, ref currentIndex, finalIndex);
            }

            this.ConsumeOneOfTokens(new List<MSWTokenType> { MSWTokenType.COMMA, MSWTokenType.EOL, MSWTokenType.EOF }, "Expect end of line after variable declaration.", tokens, ref currentIndex, finalIndex);
            return new VarDeclaration(token, initialiser);
        }

        private Statement Statement(List<MSWToken> tokens, ref int currentIndex, int finalIndex)
        {
            if (this.IsOfType(MSWTokenType.EOL, tokens, currentIndex, finalIndex))
            {
                this.PopToken(tokens, ref currentIndex, finalIndex);
            }

            if (this.IsOfType(MSWTokenType.FOR, tokens, currentIndex, finalIndex))
            {
                return this.ForStatement(tokens, ref currentIndex, finalIndex);
            }

            if (this.IsOfType(MSWTokenType.IF, tokens, currentIndex, finalIndex))
            {
                return this.IfStatement(tokens, ref currentIndex, finalIndex);
            }

            if (this.IsOfType(MSWTokenType.PRINT, tokens, currentIndex, finalIndex))
            {
                return this.PrintStatement(tokens, ref currentIndex, finalIndex);
            }

            if(this.IsOfType(MSWTokenType.WHILE, tokens, currentIndex, finalIndex))
            {
                return this.WhileStatement(tokens, ref currentIndex, finalIndex);
            }

            if (this.IsOfType(MSWTokenType.START, tokens, currentIndex, finalIndex))
            {
                return new Block(this.Block(tokens, ref currentIndex, finalIndex));
            }

            return this.ExpressionStatement(tokens, ref currentIndex, finalIndex);
        }

        private Statement PrintStatement(List<MSWToken> tokens, ref int currentIndex, int finalIndex)
        {
            this.PopToken(tokens, ref currentIndex, finalIndex);
            Expression value = this.Expression(tokens, ref currentIndex, finalIndex);
            this.ConsumeOneOfTokens(new List<MSWTokenType> { MSWTokenType.EOL, MSWTokenType.EOF }, "Expect end of line after value.", tokens, ref currentIndex, finalIndex);
            return new Print(value);
        }

        private Statement ExpressionStatement(List<MSWToken> tokens, ref int currentIndex, int finalIndex)
        {
            Expression value = this.Expression(tokens, ref currentIndex, finalIndex);
            this.ConsumeOneOfTokens(new List<MSWTokenType> { MSWTokenType.COMMA, MSWTokenType.EOL, MSWTokenType.EOF }, "Expect end of line after value.", tokens, ref currentIndex, finalIndex);
            return new StatementExpression(value);
        }

        private List<Statement> Block(List<MSWToken> tokens, ref int currentIndex, int finalIndex)
        {
            this.PopToken(tokens, ref currentIndex, finalIndex);
            this.PopToken(tokens, ref currentIndex, finalIndex);

            List<Statement> statements = new List<Statement>();

            while(!this.IsOfType(MSWTokenType.END, tokens, currentIndex, finalIndex))
            {
                statements.Add(this.Declaration(tokens, ref currentIndex, finalIndex));
            }

            this.ConsumeToken(MSWTokenType.END, "Expect END after block.", tokens, ref currentIndex, finalIndex);
            return statements;
        }

        private Statement IfStatement(List<MSWToken> tokens, ref int currentIndex, int finalIndex)
        {
            this.PopToken(tokens, ref currentIndex, finalIndex);

            Expression condition = this.Expression(tokens, ref currentIndex, finalIndex);

            this.ConsumeToken(MSWTokenType.COMMA, "Expect comma after if condition.", tokens, ref currentIndex, finalIndex);

            Statement thenBranch = this.Statement(tokens, ref currentIndex, finalIndex);
            Statement elseBranch = null;
            if(this.IsOfType(MSWTokenType.ELSE, tokens, currentIndex, finalIndex))
            {
                this.PopToken(tokens, ref currentIndex, finalIndex);
                if(this.IsOfType(MSWTokenType.COMMA, tokens, currentIndex, finalIndex)) // An additional comma after an else is not necessary, but may be used as a style choice.
                {
                    this.PopToken(tokens, ref currentIndex, finalIndex);
                }

                elseBranch = this.Statement(tokens, ref currentIndex, finalIndex);
            }

            return new If(condition, thenBranch, elseBranch);
        }

        private Statement WhileStatement(List<MSWToken> tokens, ref int currentIndex, int finalIndex)
        {
            this.PopToken(tokens, ref currentIndex, finalIndex);

            Expression condition = this.Expression(tokens, ref currentIndex, finalIndex);
            this.ConsumeToken(MSWTokenType.COMMA, "Expect comma after if condition.", tokens, ref currentIndex, finalIndex);
            Statement statement = this.Statement(tokens, ref currentIndex, finalIndex);

            return new While(condition, statement);
        }

        private Statement ForStatement(List<MSWToken> tokens, ref int currentIndex, int finalIndex)
        {
            this.PopToken(tokens, ref currentIndex, finalIndex);

            Statement initialiser;
            if(this.IsOfType(MSWTokenType.VAR, tokens, currentIndex, finalIndex))
            {
                initialiser = this.VarDeclaration(tokens, ref currentIndex, finalIndex);
            }
            else
            {
                this.PopToken(tokens, ref currentIndex, finalIndex);
                initialiser = this.ExpressionStatement(tokens, ref currentIndex, finalIndex);
            }

            Expression condition = this.Expression(tokens, ref currentIndex, finalIndex);
            this.ConsumeToken(MSWTokenType.COMMA, "Expect comma after loop condition.", tokens, ref currentIndex, finalIndex);

            Expression increment = this.Expression(tokens, ref currentIndex, finalIndex);
            this.ConsumeToken(MSWTokenType.COMMA, "Expect comma after loop increment.", tokens, ref currentIndex, finalIndex);

            Statement body = this.Statement(tokens, ref currentIndex, finalIndex);

            if(increment != null)
            {
                body = new Block(new List<Statement>() { body, new StatementExpression(increment) });
            }

            if(condition == null)
            {
                condition = new Literal(true);
            }
            body = new While(condition, body);

            if(initialiser != null)
            {
                body = new Block(new List<Statement>() { initialiser, body });
            }

            return body;
        }

        #endregion
    }
}
