namespace MSW.Scripting
{
    public enum MSWTokenType
    {
        // Single-Character
        LEFT_PARENTHESIS, RIGHT_PARENTHESIS,
        LEFT_SQUARE, RIGHT_SQUARE,
        COLON, COMMA, HASH,
        MINUS, PLUS, MULTIPLY, DIVIDE,

        // One/Two Character Tokens
        EQUAL, EQUAL_EQUAL, NOT, NOT_EQUAL,
        GREATER, GREATER_EQUAL, LESS, LESS_EQUAL,

        // Literals
        IDENTIFIER, STRING, DOUBLE,

        // -- Keywords
        // Values
        TRUE, FALSE, NULL,

        // Declarations
        VAR,
        
        // Functions
        PRINT,

        // Conditionals
        IF, ELSE, WHEN,
        WHILE, FOR,

        AND, OR,
        
        // Start and end blocks
        START, END,

        EOF, EOL, UNIDENTIFIED
    }
}
