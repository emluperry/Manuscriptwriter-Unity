namespace MSW.Scripting
{
    internal enum TokenType
    {
        // Single-Character
        COLON, COMMA, HASH,
        
        // -- Mathematics
        MINUS, PLUS, MULTIPLY, DIVIDE, NEGATE,

        // Conditionals
        EQUAL, EQUAL_EQUAL, NOT, NOT_EQUAL,
        GREATER, GREATER_EQUAL, LESS, LESS_EQUAL,

        AND, OR,
        
        // Literals and Types
        IDENTIFIER, STRING, DOUBLE,

        // -- Keywords
        // Values
        TRUE, FALSE, NULL,

        // Declarations
        VAR,
        
        // Conditionals
        IF, ELSE,
        WHILE, FOR,
        
        // Event definition
        GIVEN, WHEN,
        
        // Functions
        PRINT,
        
        EOF, EOL, FUNCTION, UNIDENTIFIED, ERROR
    }
}
