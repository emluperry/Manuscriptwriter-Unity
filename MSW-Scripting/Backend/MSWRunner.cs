using System;
using System.Collections.Generic;

namespace MSW.Scripting
{
    public class MSWRunner
    {
        public Action<string> ErrorLogger;
        public Action<string> DebugOutput;

        private bool hasError = false;
        public void Run(string source)
        {
            hasError = false;

            MSWScanner scanner = new MSWScanner(source) { ReportError = ReportScannerError };
            Queue<MSWToken> tokens = scanner.ScanTokens();

            MSWParser parser = new MSWParser(tokens) { ReportTokenError = ReportTokenError };
            IEnumerable<Statement> statements = parser.Parse();

            if(hasError)
            {
                return;
            }

            MSWInterpreter interpreter = new MSWInterpreter() { ReportRuntimeError = ReportRuntimeError };
            interpreter.Interpret(statements);
        }

        private void ReportTokenError(MSWToken token, string message)
        {
            hasError = true;
            if(token?.type == MSWTokenType.EOF)
            {
                Report(token.line, "at end", message);
            }
            else
            {
                Report(token?.line ?? 0, $"at '{token?.lexeme}'", message);
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
            ErrorLogger?.Invoke($"[Line {line} - {where}]: {message}");
        }
    }
}
