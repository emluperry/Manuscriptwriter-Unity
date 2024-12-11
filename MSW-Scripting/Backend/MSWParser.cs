using System;
using System.Collections.Generic;

namespace MSW.Scripting
{
    public class MSWParser
    {
        public Action<MSWToken, string> ReportTokenError;
        
        private Queue<MSWToken> tokens;

        public MSWParser(Queue<MSWToken> tokens)
        {
            this.tokens = tokens;
        }

        public IEnumerable<Statement> Parse()
        {
            List<Statement> statements = new List<Statement>();
            while(!this.IsEndOfStack())
            {
                if (this.CurrentMatchesType(MSWTokenType.EOF))
                {
                    break;
                }

                if (this.TryConsumeToken(MSWTokenType.EOL, out MSWToken eol))
                {
                    continue;
                }

                var s = this.Declaration();

                if(s != null)
                {
                    statements.Add(s);
                }
            }

            return statements;
        }

        private MSWToken TryDequeueToken()
        {
            if (this.IsEndOfStack())
            {
                return null;
            }

            return this.tokens.Dequeue();
        }

        private MSWToken TryPeekToken()
        {
            if (this.IsEndOfStack())
            {
                return null;
            }

            return this.tokens.Peek();
        }

        private bool CurrentMatchesOneOfTypes(IEnumerable<MSWTokenType> tokenTypes)
        {
            foreach (MSWTokenType tokenType in tokenTypes)
            { 
                if(this.CurrentMatchesType(tokenType))
                {
                    return true;
                }
            }

            return false;
        }

        private bool CurrentMatchesType(MSWTokenType tokenType)
        {
            return TryPeekToken().type == tokenType;
        }

        private bool TryConsumeToken(MSWTokenType type, out MSWToken token)
        {
            if (this.CurrentMatchesType(type))
            {
                token = this.tokens.Dequeue();
                return true;
            }

            token = null;
            return false;
        }

        private bool TryConsumeOneOfTokens(IEnumerable<MSWTokenType> tokenTypes, out MSWToken token)
        {
            if (this.CurrentMatchesOneOfTypes(tokenTypes))
            {
                token = this.TryDequeueToken();
                return true;
            }

            token = null;
            return false;
        }

        private MSWToken ConsumeToken(MSWTokenType type, string message)
        {
            if(this.CurrentMatchesType(type))
            {
                return this.TryDequeueToken();
            }

            throw this.ParseError(this.TryPeekToken(), message);
        }

        private MSWToken ConsumeOneOfTokens(List<MSWTokenType> types, string message)
        {
            foreach (MSWTokenType tokenType in types)
            {
                if (this.CurrentMatchesType(tokenType))
                {
                    return this.TryDequeueToken();
                }
            }

            throw this.ParseError(this.TryPeekToken(), message);
        }

        private bool IsEndOfStack()
        {
            return this.tokens.Count <= 0;
        }

        #region ERROR HANDLING
        private MSWParseException ParseError(MSWToken token, string message)
        {
            ReportTokenError?.Invoke(token, message);

            return new MSWParseException();
        }

        private void Synchronise()
        {
            this.TryDequeueToken();

            while(!this.IsEndOfStack())
            {
                switch(this.TryPeekToken().type)
                {
                    case MSWTokenType.EOL:
                        this.TryDequeueToken();
                        return;
                    case MSWTokenType.EOF:
                        return;
                }

                this.TryDequeueToken();
            }
        }
        #endregion

        #region STATEMENTS

        private Statement Declaration()
        {
            try
            {
                if (this.TryConsumeToken(MSWTokenType.VAR, out MSWToken var))
                {
                    return this.VarDeclaration();
                }

                return this.Statement();
            }
            catch(MSWParseException)
            {
                Synchronise();
                return null;
            }
        }

        private Statement VarDeclaration()
        {
            MSWToken token = this.ConsumeToken(MSWTokenType.IDENTIFIER, "Expect variable name.");

            Expression initialiser = null;
            if (this.TryConsumeToken(MSWTokenType.EQUAL, out MSWToken equals))
            {
                initialiser = this.Expression();
            }

            this.ConsumeOneOfTokens(new List<MSWTokenType> { MSWTokenType.COMMA, MSWTokenType.EOL, MSWTokenType.EOF }, "Expect end of line after variable declaration.");
            return new VarDeclaration(token, initialiser);
        }

        private Statement Statement()
        {
            if (this.TryConsumeToken(MSWTokenType.FOR, out MSWToken forToken))
            {
                return this.ForStatement();
            }

            if (this.TryConsumeToken(MSWTokenType.IF, out MSWToken ifToken))
            {
                return this.IfStatement();
            }

            if(this.TryConsumeToken(MSWTokenType.WHILE, out MSWToken whileToken))
            {
                return this.WhileStatement();
            }
            
            if (this.TryConsumeToken(MSWTokenType.PRINT, out MSWToken printToken))
            {
                return this.PrintStatement();
            }

            return this.ExpressionStatement();
        }

        private Statement ExpressionStatement()
        {
            Expression value = this.Expression();
            this.ConsumeOneOfTokens(new List<MSWTokenType> { MSWTokenType.COMMA, MSWTokenType.EOL, MSWTokenType.EOF }, "Expect end of line after value.");
            return new StatementExpression(value);
        }

        private Statement IfStatement()
        {
            Expression condition = this.Expression();

            this.ConsumeToken(MSWTokenType.COMMA, "Expect comma after if condition.");

            Statement thenBranch = this.Statement();
            Statement elseBranch = null;
            if(this.TryConsumeToken(MSWTokenType.ELSE, out MSWToken elseToken))
            {
                this.TryConsumeToken(MSWTokenType.COMMA, out MSWToken commaToken); // An additional comma after an else is not necessary, but may be used as a style choice.

                elseBranch = this.Statement();
            }

            return new If(condition, thenBranch, elseBranch);
        }

        private Statement WhileStatement()
        {
            Expression condition = this.Expression();
            this.ConsumeToken(MSWTokenType.COMMA, "Expect comma after if condition.");
            Statement statement = this.Statement();

            return new While(condition, statement);
        }

        private Statement ForStatement()
        {
            Statement initialiser = null;
            if(this.TryConsumeToken(MSWTokenType.VAR, out MSWToken var))
            {
                initialiser = this.VarDeclaration();
            }
            else
            {
                 this.TryDequeueToken();
                 initialiser = this.ExpressionStatement();
            }

            Expression condition = this.Expression();
            this.ConsumeToken(MSWTokenType.COMMA, "Expect comma after loop condition.");

            Expression increment = this.Expression();
            this.ConsumeToken(MSWTokenType.COMMA, "Expect comma after loop increment.");

            Statement body = this.Statement();

            if(increment != null)
            {
                body = new Block(new List<Statement>() { body, new StatementExpression(increment) });
            }

            condition ??= new Literal(true);
            
            body = new While(condition, body);

            if(initialiser != null)
            {
                body = new Block(new List<Statement>() { initialiser, body });
            }

            return body;
        }
        
        private Statement PrintStatement()
        {
            Expression value = this.Expression();
            this.ConsumeOneOfTokens(new List<MSWTokenType> { MSWTokenType.EOL, MSWTokenType.EOF }, "Expect end of line after value.");
            return new Print(value);
        }

        #endregion
        
        #region EXPRESSIONS
        private Expression Expression()
        {
            return this.Assignment();
        }

        private Expression Assignment()
        {
            Expression expression = this.Or();

            if(this.TryConsumeToken(MSWTokenType.EQUAL, out MSWToken equals))
            {
                Expression value = this.Assignment();

                if(expression is Variable var)
                {
                    MSWToken token = var.token;
                    return new Assign(token, value);
                }

                this.ParseError(equals, "Invalid assignment target.");
            }

            return expression;
        }

        private Expression Or()
        {
            Expression expression = this.And();

            while(this.TryConsumeToken(MSWTokenType.OR, out MSWToken op))
            {
                Expression right = this.And();
                expression = new Logical(expression, op, right);
            }

            return expression;
        }

        private Expression And()
        {
            Expression expression = this.Equality();

            while (this.TryConsumeToken(MSWTokenType.AND, out MSWToken op))
            {
                Expression right = this.Equality();
                expression = new Logical(expression, op, right);
            }

            return expression;
        }

        private Expression Equality()
        {
            Expression expression = this.Comparison();

            while(this.TryConsumeOneOfTokens(new List<MSWTokenType> { MSWTokenType.NOT_EQUAL, MSWTokenType.EQUAL_EQUAL }, out MSWToken op))
            {
                Expression right = this.Comparison();
                expression = new Binary(expression, op, right);
            }

            return expression;
        }

        private Expression Comparison()
        {
            Expression expression = this.Term();

            while(this.TryConsumeOneOfTokens(new List<MSWTokenType> { MSWTokenType.GREATER, MSWTokenType.GREATER_EQUAL, MSWTokenType.LESS, MSWTokenType.LESS_EQUAL}, out MSWToken op))
            {
                Expression right = this.Term();
                expression = new Binary(expression, op, right);
            }

            return expression;
        }

        private Expression Term()
        {
            Expression expression = this.Factor();

            while(this.TryConsumeOneOfTokens(new List<MSWTokenType> { MSWTokenType.MINUS, MSWTokenType.PLUS }, out MSWToken op))
            {
                Expression right = this.Factor();
                expression = new Binary(expression, op, right);
            }

            return expression;
        }

        private Expression Factor()
        {
            Expression expression = this.Unary();

            while(this.TryConsumeOneOfTokens(new List<MSWTokenType> { MSWTokenType.MULTIPLY, MSWTokenType.DIVIDE }, out MSWToken op))
            {
                Expression right = this.Unary();
                expression = new Binary(expression, op, right);
            }

            return expression;
        }

        private Expression Unary()
        {
            if(this.TryConsumeOneOfTokens(new List<MSWTokenType> { MSWTokenType.NOT, MSWTokenType.MINUS }, out MSWToken op))
            {
                Expression right = this.Unary();
                return new Unary(op, right);
            }

            return this.Call();
        }

        private Expression Call()
        {
            Expression expression = this.Primary();
            
            // How are we going to pull functions from external "libraries"?
            if (this.TryConsumeToken(MSWTokenType.COLON, out MSWToken op))
            {
                 // Dialogue
                 Expression dialogue = this.Primary();
                 
                 // Need to make this into an expression -> best to use call I think, but need to figure how best to do so.
                 return new Call("RunDialogue", op, new List<Expression>() { expression, dialogue });
            }
            
            return expression;
        }

        private Expression Primary()
        {
            if(this.TryConsumeToken(MSWTokenType.FALSE, out MSWToken opf))
            {
                return new Literal(false);
            }
            if (this.TryConsumeToken(MSWTokenType.TRUE, out MSWToken opt))
            {
                return new Literal(true);
            }
            if (this.TryConsumeToken(MSWTokenType.NULL, out MSWToken opn))
            {
                return new Literal(null);
            }

            if (this.TryConsumeOneOfTokens(new List<MSWTokenType> { MSWTokenType.STRING, MSWTokenType.DOUBLE }, out MSWToken opl))
            {
                return new Literal(opl.literal);
            }

            if(this.TryConsumeToken(MSWTokenType.IDENTIFIER, out MSWToken opv))
            {
                return new Variable(opv);
            }

            throw ParseError(this.TryPeekToken(), "Expect expression.");
        }

        #endregion
    }
}
