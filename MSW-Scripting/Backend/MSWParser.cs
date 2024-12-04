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

        public Expression Parse(List<MSWToken> tokens)
        {
            int finalIndex = tokens.Count;
            int currentIndex = 0;

            try
            {
                return this.Expression(tokens, ref currentIndex, finalIndex);
            }
            catch(MSWParseException e)
            {
                return null;
            }
        }

        private MSWToken NextToken(List<MSWToken> tokens, ref int currentIndex, int finalIndex)
        {
            var token = tokens[currentIndex];
            if(currentIndex >= finalIndex)
            {
                ++currentIndex;
            }

            return token;
        }

        private MSWToken PeekNextToken(List<MSWToken> tokens, int currentIndex, int finalIndex)
        {
            if (currentIndex + 1 >= finalIndex)
            {
                return null;
            }

            return tokens[currentIndex + 1];
        }

        private MSWToken PeekPreviousToken(List<MSWToken> tokens, int currentIndex)
        {
            if(currentIndex - 1 <= 0)
            {
                return null;
            }

            return tokens[currentIndex - 1];
        }

        private bool NextIsOneOfTypes(List<MSWTokenType> tokenTypes, List<MSWToken> tokens, ref int currentIndex, int finalIndex)
        {
            foreach (MSWTokenType tokenType in tokenTypes)
            { 
                if(this.NextIsType(tokenType, tokens, currentIndex, finalIndex))
                {
                    this.NextToken(tokens, ref currentIndex, finalIndex);
                    return true;
                }
            }

            return false;
        }

        private bool NextIsType(MSWTokenType tokenType, List<MSWToken> tokens, int currentIndex, int finalIndex)
        {
            if(currentIndex >= finalIndex)
            {
                return false;
            }

            return PeekNextToken(tokens, currentIndex, finalIndex).type == tokenType;
        }

        private MSWToken ConsumeToken(MSWTokenType type, string message, List<MSWToken> tokens, ref int currentIndex, int finalIndex)
        {
            if(this.NextIsType(type, tokens, currentIndex, finalIndex))
            {
                return this.NextToken(tokens, ref currentIndex, finalIndex);
            }

            throw this.ParseError(this.PeekNextToken(tokens, currentIndex, finalIndex), message);
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
                if(this.PeekPreviousToken(tokens, currentIndex).type == MSWTokenType.EOL)
                {
                    return;
                }

                switch(this.PeekNextToken(tokens, currentIndex, finalIndex).type)
                {
                    case MSWTokenType.END:
                        return;
                }

                this.NextToken(tokens, ref currentIndex, finalIndex);
            }
        }
        #endregion

        private Expression Expression(List<MSWToken> tokens, ref int currentIndex, int finalIndex)
        {
            return this.Equality(tokens, ref currentIndex, finalIndex);
        }

        private Expression Equality(List<MSWToken> tokens, ref int currentIndex, int finalIndex)
        {
            Expression expression = this.Comparison(tokens, ref currentIndex, finalIndex);

            while(this.NextIsOneOfTypes(new List<MSWTokenType> { MSWTokenType.NOT_EQUAL, MSWTokenType.EQUAL }, tokens, ref currentIndex, finalIndex))
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

            while(this.NextIsOneOfTypes(new List<MSWTokenType> { MSWTokenType.GREATER, MSWTokenType.GREATER_EQUAL, MSWTokenType.LESS, MSWTokenType.LESS_EQUAL}, tokens, ref currentIndex, finalIndex))
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

            while(this.NextIsOneOfTypes(new List<MSWTokenType> { MSWTokenType.MINUS, MSWTokenType.PLUS }, tokens, ref currentIndex, finalIndex))
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

            while(this.NextIsOneOfTypes(new List<MSWTokenType> { MSWTokenType.MULTIPLY, MSWTokenType.DIVIDE }, tokens, ref currentIndex, finalIndex))
            {
                MSWToken op = this.PeekPreviousToken(tokens, currentIndex);
                Expression right = this.Unary(tokens, ref currentIndex, finalIndex);
                expression = new Binary(expression, op, right);
            }

            return expression;
        }

        private Expression Unary(List<MSWToken> tokens, ref int currentIndex, int finalIndex)
        {
            if(this.NextIsOneOfTypes(new List<MSWTokenType> { MSWTokenType.NOT, MSWTokenType.MINUS }, tokens, ref currentIndex, finalIndex))
            {
                MSWToken op = this.PeekPreviousToken(tokens, currentIndex);
                Expression right = this.Unary(tokens, ref currentIndex, finalIndex);
                return new Unary(op, right);
            }

            return this.Primary(tokens, ref currentIndex, finalIndex);
        }

        private Expression Primary(List<MSWToken> tokens, ref int currentIndex, int finalIndex)
        {
            if(this.NextIsOneOfTypes(new List<MSWTokenType> { MSWTokenType.FALSE }, tokens, ref currentIndex, finalIndex))
            {
                return new Literal(false);
            }
            if (this.NextIsOneOfTypes(new List<MSWTokenType> { MSWTokenType.TRUE }, tokens, ref currentIndex, finalIndex))
            {
                return new Literal(true);
            }
            if (this.NextIsOneOfTypes(new List<MSWTokenType> { MSWTokenType.NULL }, tokens, ref currentIndex, finalIndex))
            {
                return new Literal(null);
            }

            if (this.NextIsOneOfTypes(new List<MSWTokenType> { MSWTokenType.STRING, MSWTokenType.DOUBLE }, tokens, ref currentIndex, finalIndex))
            {
                return new Literal(this.PeekPreviousToken(tokens, currentIndex).literal);
            }

            if (this.NextIsOneOfTypes(new List<MSWTokenType> { MSWTokenType.LEFT_PARENTHESIS }, tokens, ref currentIndex, finalIndex))
            {
                Expression expression = this.Expression(tokens, ref currentIndex, finalIndex);
                this.ConsumeToken(MSWTokenType.RIGHT_PARENTHESIS, "Expect ')' after expression.", tokens, ref currentIndex, finalIndex);
                return new Grouping(expression);
            }

            throw ParseError(this.PeekNextToken(tokens, currentIndex, finalIndex), "Expect expression.");
        }
    }
}
