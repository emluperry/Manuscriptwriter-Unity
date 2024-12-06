using System.Collections.Generic;

namespace MSW.Scripting
{
    public class Environment
    {
        private readonly Dictionary<string, object> variables = new Dictionary<string, object>();

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


            throw new MSWRuntimeException(token, $"Undefined variable {token.lexeme}.");
        }
    }
}
