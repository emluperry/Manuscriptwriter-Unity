using UnityEngine;

namespace MSW.Scripting
{
    public class MSWToken
    {
        private readonly MSWTokenType type;
        private readonly string lexeme;
        private readonly object literal;
        private readonly int line;

        public MSWToken(MSWTokenType type, string lexeme, object literal, int line)
        {
            this.type = type;
            this.lexeme = lexeme;
            this.literal = literal;
            this.line = line;
        }

        public override string ToString()
        {
            return $"{type} - {lexeme} - {literal}";
        }
    }
}
