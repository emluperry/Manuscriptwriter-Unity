using System.Collections.Generic;

namespace MSW.Scripting
{
    public class Environment
    {
        private readonly Dictionary<string, object> variables = new Dictionary<string, object>();
        private readonly Environment enclosing;

        public Environment()
        {
            enclosing = null;
        }

        public Environment(Environment enclosing)
        {
            this.enclosing = enclosing;
        }

        public void Define(string name, object value)
        {
            variables[name] = value;
        }

        public object Get(MSWToken token)
        {
            if(variables.ContainsKey(token.lexeme))
            {
                return variables[token.lexeme];
            }

            if(this.enclosing != null)
            {
                return enclosing.Get(token);
            }
            
            throw new MSWRuntimeException(token, $"Undefined variable {token.lexeme}.");
        }

        public object Get(string lexeme)
        {
            if(variables.ContainsKey(lexeme))
            {
                return variables[lexeme];
            }

            if(this.enclosing != null)
            {
                return enclosing.Get(lexeme);
            }


            throw new MSWRuntimeException(null, $"Undefined variable {lexeme}.");
        }

        public void Assign(MSWToken token, object value)
        {
            if(variables.ContainsKey(token.lexeme))
            {
                variables[token.lexeme] = value;
                return;
            }

            if(enclosing != null)
            {
                enclosing.Assign(token, value);
                return;
            }

            throw new MSWRuntimeException(token, $"Undefined variable {token.lexeme}.");
        }
    }
}
