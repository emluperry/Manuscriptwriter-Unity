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

        private MSWToken NextToken(List<MSWToken> tokens, ref int currentIndex, int finalIndex)
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
                    this.NextToken(tokens, ref currentIndex, finalIndex);
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
                return this.NextToken(tokens, ref currentIndex, finalIndex);
            }

            throw this.ParseError(this.PeekToken(tokens, currentIndex, finalIndex), message);
        }

        private MSWToken ConsumeOneOfTokens(List<MSWTokenType> types, string message, List<MSWToken> tokens, ref int currentIndex, int finalIndex)
        {
            foreach (MSWTokenType tokenType in types)
            {
                if (this.IsOfType(tokenType, tokens, currentIndex, finalIndex))
                {
                    return this.NextToken(tokens, ref currentIndex, finalIndex);
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
            this.NextToken(tokens, ref currentIndex, finalIndex);

            while(currentIndex < finalIndex)
            {
                switch(this.PeekToken(tokens, currentIndex, finalIndex).type)
                {
                    case MSWTokenType.END:
                    case MSWTokenType.EOL:
                        this.NextToken(tokens, ref currentIndex, finalIndex);
                        return;
                    case MSWTokenType.EOF:
                        return;
                }

                this.NextToken(tokens, ref currentIndex, finalIndex);
            }
        }
        #endregion

        #region EXPRESSIONS
        private Expression Expression(List<MSWToken> tokens, ref int currentIndex, int finalIndex)
        {
            return this.Equality(tokens, ref currentIndex, finalIndex);
        }

        private Expression Equality(List<MSWToken> tokens, ref int currentIndex, int finalIndex)
        {
            Expression expression = this.Comparison(tokens, ref currentIndex, finalIndex);

            while(this.IsOneOfTypes(new List<MSWTokenType> { MSWTokenType.NOT_EQUAL, MSWTokenType.EQUAL }, tokens, ref currentIndex, finalIndex))
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

            throw ParseError(this.PeekToken(tokens, currentIndex, finalIndex), "Expect expression.");
        }

        #endregion

        #region STATEMENTS

        private Statement Declaration(List<MSWToken> tokens, ref int currentIndex, int finalIndex)
        {
            try
            {
                if(this.IsOfType(MSWTokenType.VAR, tokens, currentIndex, finalIndex))
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
            this.NextToken(tokens, ref currentIndex, finalIndex);
            MSWToken token = this.ConsumeToken(MSWTokenType.IDENTIFIER, "Expect variable name.", tokens, ref currentIndex, finalIndex);

            Expression initialiser = null;
            if (this.IsOfType(MSWTokenType.EQUAL, tokens, currentIndex, finalIndex))
            {
                this.NextToken(tokens, ref currentIndex, finalIndex);
                initialiser = this.Expression(tokens, ref currentIndex, finalIndex);
            }

            this.ConsumeOneOfTokens(new List<MSWTokenType> { MSWTokenType.EOL, MSWTokenType.EOF }, "Expect end of line after variable declaration.", tokens, ref currentIndex, finalIndex);
            return new VarDeclaration(token, initialiser);
        }

        private Statement Statement(List<MSWToken> tokens, ref int currentIndex, int finalIndex)
        {
            if (this.IsOfType(MSWTokenType.PRINT, tokens, currentIndex, finalIndex))
            {
                return this.PrintStatement(tokens, ref currentIndex, finalIndex);
            }

            return this.ExpressionStatement(tokens, ref currentIndex, finalIndex);
        }

        private Statement PrintStatement(List<MSWToken> tokens, ref int currentIndex, int finalIndex)
        {
            this.NextToken(tokens, ref currentIndex, finalIndex);
            Expression value = this.Expression(tokens, ref currentIndex, finalIndex);
            this.ConsumeOneOfTokens(new List<MSWTokenType> { MSWTokenType.EOL, MSWTokenType.EOF }, "Expect end of line after value.", tokens, ref currentIndex, finalIndex);
            return new Print(value);
        }

        private Statement ExpressionStatement(List<MSWToken> tokens, ref int currentIndex, int finalIndex)
        {
            Expression value = this.Expression(tokens, ref currentIndex, finalIndex);
            this.ConsumeOneOfTokens(new List<MSWTokenType> { MSWTokenType.EOL, MSWTokenType.EOF }, "Expect end of line after value.", tokens, ref currentIndex, finalIndex);
            return new StatementExpression(value);
        }

        #endregion
    }
}
