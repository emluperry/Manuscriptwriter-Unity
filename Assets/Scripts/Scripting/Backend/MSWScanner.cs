using System;
using System.Collections.Generic;

namespace MSW.Scripting
{
    public class MSWScanner
    {
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

            MSWTokenType type = MSWTokenType.UNIDENTIFIED;
            switch(c)
            {
                case '(':
                    type = MSWTokenType.LEFT_PARENTHESIS;
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
                    ReportError?.Invoke(line, $"character {startIndex - currentIndex}", "Unexpected character.");
                    break;
            }

            return new MSWToken(type, source.Substring(startIndex, currentIndex), null, line);
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
    }
}
