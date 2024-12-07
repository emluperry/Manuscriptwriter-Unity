namespace MSW.Scripting
{
    public enum MSWTokenType
    {
        // Single-Character
        COLON, COMMA, HASH,
        
        // -- Mathematics
        MINUS, PLUS, MULTIPLY, DIVIDE,

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
        WHEN,
        
        // Functions
        PRINT,
        
        EOF, EOL, UNIDENTIFIED
    }
}
