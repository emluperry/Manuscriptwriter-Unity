using UnityEngine;

namespace MSW.Scripting
{
    public enum MSWTokenType
    {
        // Single-Character
        LEFT_PARENTHESIS, RIGHT_PARENTHESIS,
        LEFT_SQUARE, RIGHT_SQUARE,
        COLON, COMMA, HASH,

        // One/Two Character Tokens
        // EX: ! (BANG), != (BANG_EQUAL) etc.

        // Literals
        IDENTIFIER, STRING, NUMBER,

        // Keywords
        TRUE, FALSE, END,

        EOF, UNIDENTIFIED
    }
}
