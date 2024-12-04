using System;
using System.Collections.Generic;
using UnityEngine;

namespace MSW.Scripting
{
    public class MSWRunner
    {
        private bool hasError = false;
        public void Run(string source)
        {
            hasError = false;

            MSWScanner scanner = new MSWScanner() { ReportError = ReportScannerError };
            List<MSWToken> tokens = scanner.ScanTokens(source);

            MSWParser parser = new MSWParser() { ReportTokenError = ReportTokenError };
            Expression expression = parser.Parse(tokens);

            if(hasError)
            {
                return;
            }

            MSWInterpreter interpreter = new MSWInterpreter();
            interpreter.Interpret(expression);

            if(hasError)
            {
                return;
            }
        }

        private void ReportTokenError(MSWToken token, string message)
        {
            hasError = true;
            if(token.type == MSWTokenType.EOF)
            {
                Report(token.line, "at end", message);
            }
            else
            {
                Report(token.line, $"at '{token.lexeme}'", message);
            }
        }

        private void ReportScannerError(int line, string where, string message)
        {
            hasError = true;
            this.Report(line, where, message);
        }

        private void ReportRuntimeError(MSWRuntimeException ex)
        {
            hasError = true;
            this.Report(ex.operatorToken.line, "", ex.Message);
        }

        private void Report(int line, string where, string message)
        {
            Debug.Log($"[Line {line} - {where}]: {message}");
        }
    }
}
