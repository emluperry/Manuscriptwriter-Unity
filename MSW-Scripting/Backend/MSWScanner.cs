using System;
using System.Collections.Generic;

namespace MSW.Scripting
{
    public class MSWScanner
    {
        private static readonly Dictionary<string, MSWTokenType> keywords = new Dictionary<string, MSWTokenType>()
        {
            { "end", MSWTokenType.END },
            { "start", MSWTokenType.START },
            { "true", MSWTokenType.TRUE },
            { "false", MSWTokenType.FALSE },
            { "not", MSWTokenType.NOT },
            { "null", MSWTokenType.NULL },
            { "print", MSWTokenType.PRINT },
            { "var", MSWTokenType.VAR },
        };
        
        public Action<int, string, string> ReportError;
        public MSWScanner()
        {
        }

        public List<MSWToken> ScanTokens(string source)
        {
            List<MSWToken> tokens = new List<MSWToken>();

            int startIndex = 0;
            int currentIndex = 0;
            int finalIndex = source.Length;
            int line = 1;

            while (currentIndex < finalIndex)
            {
                startIndex = currentIndex;
                var token = ScanToken(source, ref startIndex, ref currentIndex, finalIndex, ref line);

                if(token.type != MSWTokenType.UNIDENTIFIED)
                {
                    tokens.Add(token);
                }
            }

            tokens.Add(new MSWToken(MSWTokenType.EOF, "", null, line));
            return tokens;
        }

        private MSWToken ScanToken(string source, ref int startIndex, ref int currentIndex, int finalIndex, ref int line)
        {
            char c = this.NextCharacter(source, ref currentIndex);
            string lexeme = "";
            object literal = null;

            MSWTokenType type = MSWTokenType.UNIDENTIFIED;
            switch(c)
            {
                case '(':
                    type = MSWTokenType.LEFT_PARENTHESIS;
                    lexeme += c;
                    break;
                case ')':
                    type = MSWTokenType.RIGHT_PARENTHESIS;
                    lexeme += c;
                    break;
                case '[':
                    type = MSWTokenType.LEFT_SQUARE;
                    lexeme += c;
                    break;
                case ']':
                    type = MSWTokenType.RIGHT_SQUARE;
                    lexeme += c;
                    break;
                case ',':
                    type = MSWTokenType.COMMA;
                    lexeme += c;
                    break;
                case ':':
                    type = MSWTokenType.COLON;
                    lexeme += c;
                    break;

                case '-':
                    type = MSWTokenType.MINUS;
                    lexeme += c;
                    break;
                case '+':
                    type = MSWTokenType.PLUS;
                    lexeme += c;
                    break;
                case '*':
                    type = MSWTokenType.MULTIPLY;
                    lexeme += c;
                    break;
                case '/':
                    type = MSWTokenType.LEFT_PARENTHESIS;
                    lexeme += c;
                    break;

                case '!':
                    if(CheckNextCharacter(ref currentIndex, finalIndex, source, '='))
                    {
                        type = MSWTokenType.NOT_EQUAL;
                    }
                    else
                    {
                        type = MSWTokenType.NOT;
                    }
                    break;

                case '=':
                    if (CheckNextCharacter(ref currentIndex, finalIndex, source, '='))
                    {
                        type = MSWTokenType.EQUAL_EQUAL;
                    }
                    else
                    {
                        type = MSWTokenType.EQUAL;
                    }
                    break;

                case '<':
                    if (CheckNextCharacter(ref currentIndex, finalIndex, source, '='))
                    {
                        type = MSWTokenType.LESS_EQUAL;
                    }
                    else
                    {
                        type = MSWTokenType.LESS;
                    }
                    break;

                case '>':
                    if (CheckNextCharacter(ref currentIndex, finalIndex, source, '='))
                    {
                        type = MSWTokenType.GREATER_EQUAL;
                    }
                    else
                    {
                        type = MSWTokenType.GREATER;
                    }
                    break;

                case '#': // Remove comments.
                    while (currentIndex < finalIndex && this.PeekNextCharacter(source, currentIndex, finalIndex) != '\n')
                    {
                        this.NextCharacter(source, ref currentIndex);
                    }

                    this.NextCharacter(source, ref currentIndex); // skip past the EOL
                    break;

                case ' ': // Ignore whitespace
                case '\r':
                case '\t':
                    break;

                case '\n':
                    ++line;
                    type = MSWTokenType.EOL;
                    break;
                case '\0':
                    type = MSWTokenType.EOF;
                    break;

                default:
                    if(IsDigit(c))
                    {
                        type = this.GetDouble(source, ref currentIndex, startIndex, finalIndex, out double d);
                        literal = d;
                        lexeme = d.ToString();
                    }
                    else if(IsAlphabetic(c))
                    {
                        type = GetIdentifier(source, ref currentIndex, startIndex, finalIndex, out lexeme);

                    }
                    else
                    {
                        ReportError?.Invoke(line, $"character {startIndex - currentIndex}", "Unexpected character.");
                    }

                    break;
            }

            if(string.IsNullOrEmpty(lexeme))
            {
                int length = currentIndex - startIndex;
                lexeme = source.Substring(startIndex, length);
            }

            return new MSWToken(type, lexeme, literal, line);
        }

        private char PeekNextCharacter(string source, int currentIndex, int finalIndex)
        {
            if (currentIndex >= finalIndex)
            {
                return '\0';
            }
            return source[currentIndex];
        }

        private char NextCharacter(string source, ref int currentIndex)
        {
            var output = source[currentIndex];
            ++currentIndex;
            return output;
        }

        private bool CheckNextCharacter(ref int currentIndex, int finalIndex, string source, char expectedCharacter)
        {
            if(this.PeekNextCharacter(source, currentIndex, finalIndex) != expectedCharacter)
            {
                return false;
            }

            ++currentIndex;
            return true;
        }

        #region Types
        // Current implementation assumes "" is necessary to define a string.
        private string GetString(string source, ref int currentIndex, int startIndex, int finalIndex, ref int line)
        {
            char nextCharacter = PeekNextCharacter(source, currentIndex, finalIndex);
            while (nextCharacter != '"' && currentIndex + 1 < finalIndex)
            {
                if (nextCharacter == '\n')
                {
                    ++line;
                }

                NextCharacter(source, ref currentIndex);
                nextCharacter = PeekNextCharacter(source, currentIndex, finalIndex);
            }

            if(PeekNextCharacter(source, currentIndex, finalIndex) == '"')
            {
                NextCharacter(source, ref currentIndex);
            }

            int length = (currentIndex - 1) - (startIndex + 1);
            return source.Substring(startIndex + 1, length);
        }

        private MSWTokenType GetDouble(string source, ref int currentIndex, int startIndex, int finalIndex, out double value)
        {
            char nextCharacter = PeekNextCharacter(source, currentIndex, finalIndex);
            while (IsDigit(nextCharacter) && currentIndex < finalIndex)
            {
                NextCharacter(source, ref currentIndex);

                nextCharacter = PeekNextCharacter(source, currentIndex, finalIndex);
            }

            if(nextCharacter == '.' && IsDigit(PeekNextCharacter(source, currentIndex, finalIndex)))
            {
                NextCharacter(source, ref currentIndex);
                nextCharacter = PeekNextCharacter(source, currentIndex, finalIndex);

                while (IsDigit(nextCharacter) && currentIndex < finalIndex)
                {
                    NextCharacter(source, ref currentIndex);

                    nextCharacter = PeekNextCharacter(source, currentIndex, finalIndex);
                }
            }

            int length = currentIndex - startIndex;
            string str = source.Substring(startIndex, length);
            if(Double.TryParse(str, out double result))
            {
                value = result;
                return MSWTokenType.DOUBLE;
            }

            value = 0;
            return MSWTokenType.UNIDENTIFIED;
        }

        private MSWTokenType GetIdentifier(string source, ref int currentIndex, int startIndex, int finalIndex, out string value)
        {
            while(IsAlphanumeric(PeekNextCharacter(source, currentIndex, finalIndex)))
            {
                NextCharacter(source, ref currentIndex);
            }

            int length = currentIndex - startIndex;
            string identifier = source.Substring(startIndex, length).ToLowerInvariant();
            MSWTokenType type = MSWTokenType.IDENTIFIER;
            if (keywords.TryGetValue(identifier, out MSWTokenType retrievedType)) 
            {
                type = retrievedType;
            }

            value = identifier;
            return type;
        }
        #endregion

        #region Helpers
        private bool IsDigit(char c)
        {
            return c >= '0' && c <= '9';
        }

        private bool IsAlphabetic(char c)
        {
            return (c >= 'A' && c <= 'Z') || (c >= 'a' && c <= 'z');
        }

        private bool IsAlphanumeric(char c)
        {
            return IsDigit(c) || IsAlphabetic(c);
        }
        #endregion
    }
}
