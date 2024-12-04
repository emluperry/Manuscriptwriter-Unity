using System;
using System.Collections.Generic;

namespace MSW.Scripting
{
    public class MSWScanner
    {
        private static readonly Dictionary<string, MSWTokenType> keywords = new Dictionary<string, MSWTokenType>()
        {
            { "End", MSWTokenType.END },
            { "True", MSWTokenType.TRUE },
            { "False", MSWTokenType.FALSE }
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
                tokens.Add(ScanToken(source, ref startIndex, ref currentIndex, finalIndex, ref line));
            }

            tokens.Add(new MSWToken(MSWTokenType.EOF, "", null, 0));
            return tokens;
        }

        private MSWToken ScanToken(string source, ref int startIndex, ref int currentIndex, int finalIndex, ref int line)
        {
            ++currentIndex;
            char c = this.NextCharacter(source, ref currentIndex);
            string lexeme = "";

            MSWTokenType type = MSWTokenType.UNIDENTIFIED;
            switch(c)
            {
                case '(':
                    type = MSWTokenType.LEFT_PARENTHESIS;
                    lexeme += c;
                    break;

                // Not currently syntax, but this is testing code to make sure I'm understanding everything.
                case '!':
                    if(CheckNextCharacter(ref currentIndex, finalIndex, source, '='))
                    {
                        // type is !=
                    }
                    else
                    {
                        // type is just !
                    }

                    type = MSWTokenType.UNIDENTIFIED;
                    break;

                case '#': // Remove comments.
                    while (this.PeekNextCharacter(source, currentIndex, finalIndex) != '\n' && currentIndex + 1 < finalIndex)
                    {
                        this.NextCharacter(source, ref currentIndex);
                    }
                    break;

                case ' ': // Ignore whitespace
                case '\r':
                case '\t':
                    break;

                case '\n':
                    ++line;
                    break;

                default:
                    if(IsAlphabetic(c))
                    {
                        type = GetIdentifier(source, ref currentIndex, startIndex, finalIndex, out lexeme);

                    }
                    else
                    {
                        ReportError?.Invoke(line, $"character {startIndex - currentIndex}", "Unexpected character.");
                    }

                    break;
            }

            if(lexeme == string.Empty)
            {
                lexeme = source.Substring(startIndex, currentIndex);
            }

            return new MSWToken(type, lexeme, null, line);
        }

        private char PeekNextCharacter(string source, int currentIndex, int finalIndex)
        {
            if (currentIndex + 1 >= finalIndex)
            {
                return '\0';
            }
            return source[currentIndex + 1];
        }

        private char NextCharacter(string source, ref int currentIndex)
        {
            return source[++currentIndex];
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

            return source.Substring(startIndex + 1, currentIndex - 1);
        }

        private double GetDouble(string source, ref int currentIndex, int startIndex, int finalIndex)
        {
            char nextCharacter = PeekNextCharacter(source, currentIndex, finalIndex);
            while (IsDigit(nextCharacter) && currentIndex + 1 < finalIndex)
            {
                NextCharacter(source, ref currentIndex);

                nextCharacter = PeekNextCharacter(source, currentIndex, finalIndex);
            }

            if(nextCharacter == '.' && IsDigit(PeekNextCharacter(source, currentIndex, finalIndex)))
            {
                NextCharacter(source, ref currentIndex);
                nextCharacter = PeekNextCharacter(source, currentIndex, finalIndex);

                while (IsDigit(nextCharacter) && currentIndex + 1 < finalIndex)
                {
                    NextCharacter(source, ref currentIndex);

                    nextCharacter = PeekNextCharacter(source, currentIndex, finalIndex);
                }
            }

            string str = source.Substring(startIndex, currentIndex);
            if(Double.TryParse(str, out double result))
            {
                return result;
            }

            return 0;
        }

        private MSWTokenType GetIdentifier(string source, ref int currentIndex, int startIndex, int finalIndex, out string value)
        {
            while(IsAlphanumeric(PeekNextCharacter(source, currentIndex, finalIndex)))
            {
                NextCharacter(source, ref currentIndex);
            }

            string identifier = source.Substring(startIndex, currentIndex);
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
