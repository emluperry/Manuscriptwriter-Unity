using System;
using System.Collections.Generic;

namespace MSW.Scripting
{
    public class MSWScanner
    {
        private static readonly Dictionary<string, MSWTokenType> keywords = new Dictionary<string, MSWTokenType>()
        {
            { "true", MSWTokenType.TRUE },
            { "false", MSWTokenType.FALSE },
            { "not", MSWTokenType.NOT },
            { "is", MSWTokenType.EQUAL_EQUAL },
            { "null", MSWTokenType.NULL },
            { "print", MSWTokenType.PRINT },
            { "var", MSWTokenType.VAR },
            { "if", MSWTokenType.IF },
            { "else", MSWTokenType.ELSE },
            { "when", MSWTokenType.WHEN },
            { "while", MSWTokenType.WHILE },
            { "for", MSWTokenType.FOR },
            { "otherwise", MSWTokenType.ELSE },
            { "and", MSWTokenType.AND },
            { "or", MSWTokenType.OR },
        };
        
        public Action<int, string, string> ReportError;

        private string source;
        private List<MSWToken> tokens;
        private int startIndex = 0;
        private int currentIndex = 0;
        private int finalIndex;
        private int line = 1;
        
        public MSWScanner(string source)
        {
            this.source = source;
        }

        public Queue<MSWToken> ScanTokens()
        {
            tokens = new List<MSWToken>();
            finalIndex = source.Length;

            while (!this.IsEndOfFile())
            {
                startIndex = currentIndex;
                this.ScanToken();
            }

            tokens.Add(new MSWToken(MSWTokenType.EOF, "", null, line));

            return new Queue<MSWToken>(tokens);
        }

        private void AddToken(MSWTokenType token, char lexeme, object literal = null) => this.AddToken(token, $"{lexeme}", literal);
        private void AddToken(MSWTokenType token, string lexeme, object literal = null)
        {
            if (token == MSWTokenType.UNIDENTIFIED)
            {
                return;
            }
            
            this.tokens.Add(new MSWToken(token, lexeme, literal, line));
        }
        
        private char PeekNextCharacter()
        {
            if (this.IsEndOfFile())
            {
                return '\0';
            }
            return source[currentIndex];
        }

        private char NextCharacter()
        {
            var output = source[currentIndex];
            ++currentIndex;
            return output;
        }

        private bool ConsumeNextCharacterIfMatching(char expectedCharacter)
        {
            if(this.PeekNextCharacter() != expectedCharacter)
            {
                return false;
            }

            ++currentIndex;
            return true;
        }

        private void ScanToken()
        {
            char c = this.NextCharacter();
            switch(c)
            {
                case ',': this.AddToken(MSWTokenType.COMMA, c);
                    break;
                case ':': this.AddToken(MSWTokenType.COLON, c);

                    if (this.ConsumeNextCharacterIfMatching(' '))
                    {
                        this.AddToken(MSWTokenType.STRING, this.GetString('\n'), this.GetString('\n'));
                    }
                    break;
                case '"': this.AddToken(MSWTokenType.STRING, this.GetString('\n'), this.GetString('\n'));
                    break;

                case '-': this.AddToken(MSWTokenType.MINUS, c);
                    break;
                case '+': this.AddToken(MSWTokenType.PLUS, c);
                    break;
                case '*': this.AddToken(MSWTokenType.MULTIPLY, c);
                    break;
                case '/': this.AddToken(MSWTokenType.DIVIDE, c);
                    break;

                case '!':
                    if(ConsumeNextCharacterIfMatching('='))
                    {
                        this.AddToken(MSWTokenType.NOT_EQUAL, "!=");
                    }
                    else
                    {
                        this.AddToken(MSWTokenType.NOT, c);
                    }
                    break;

                case '=':
                    if (ConsumeNextCharacterIfMatching('='))
                    {
                        this.AddToken(MSWTokenType.EQUAL_EQUAL, "==");
                    }
                    else
                    {
                        this.AddToken(MSWTokenType.EQUAL, c);
                    }
                    break;

                case '<':
                    if (ConsumeNextCharacterIfMatching('='))
                    {
                        this.AddToken(MSWTokenType.LESS_EQUAL, "<=");
                    }
                    else
                    {
                        this.AddToken(MSWTokenType.LESS, c);
                    }
                    break;

                case '>':
                    if (ConsumeNextCharacterIfMatching('='))
                    {
                        this.AddToken(MSWTokenType.GREATER_EQUAL, ">=");
                    }
                    else
                    {
                        this.AddToken(MSWTokenType.GREATER, c);
                    }
                    break;

                case '#': // Remove comments.
                    while (!this.IsEndOfFile() && this.PeekNextCharacter() != '\n')
                    {
                        this.NextCharacter();
                    }

                    this.NextCharacter(); // skip past the EOL
                    break;

                case ' ': // Ignore whitespace
                case '\r':
                case '\t':
                    break;

                case '\n': this.AddToken(MSWTokenType.EOL, c);
                    ++line;
                    break;
                case '\0': this.AddToken(MSWTokenType.EOF, c);
                    break;

                default:
                    if(IsDigit(c))
                    {
                        this.AddToken(this.GetDouble(out double d), d.ToString(), d);
                    }
                    else if(IsAlphabetic(c))
                    {
                        this.AddToken(this.GetIdentifier(out string l), l, l);
                    }
                    else
                    {
                        ReportError?.Invoke(line, $"character {startIndex - currentIndex}", "Unexpected character.");
                    }

                    break;
            }
        }

        #region Types
        private string GetString(char endChar = '"')
        {
            char nextCharacter = PeekNextCharacter();
            while (nextCharacter != endChar && currentIndex + 1 < finalIndex)
            {
                if (nextCharacter == '\n')
                {
                    ++line;
                }

                NextCharacter();
                nextCharacter = PeekNextCharacter();
            }

            if(PeekNextCharacter() == '"')
            {
                NextCharacter();
            }

            int length = (currentIndex - 1) - (startIndex + 1);
            return source.Substring(startIndex + 1, length);
        }

        private MSWTokenType GetDouble(out double value)
        {
            char nextCharacter = PeekNextCharacter();
            while (IsDigit(nextCharacter) && !this.IsEndOfFile())
            {
                NextCharacter();

                nextCharacter = PeekNextCharacter();
            }

            if(nextCharacter == '.' && IsDigit(PeekNextCharacter()))
            {
                NextCharacter();
                nextCharacter = PeekNextCharacter();

                while (IsDigit(nextCharacter) && !this.IsEndOfFile())
                {
                    NextCharacter();

                    nextCharacter = PeekNextCharacter();
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

        private MSWTokenType GetIdentifier(out string value)
        {
            while(IsAlphanumeric(PeekNextCharacter()))
            {
                NextCharacter();
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
        private bool IsEndOfFile()
        {
            return currentIndex >= finalIndex;
        }
        
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
