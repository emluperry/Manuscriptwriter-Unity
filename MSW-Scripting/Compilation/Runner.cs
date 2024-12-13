using System;
using System.Collections.Generic;

using MSW.Scripting;
namespace MSW.Compiler
{
    public class Runner
    {
        public readonly List<object> FunctionLibrary;
        
        public Action<string> ErrorLogger;
        public Action<string> DebugOutput;

        private bool hasError = false;

        public Runner(List<object> functions = null)
        {
            FunctionLibrary = functions;
        }
        
        public void Run(string source)
        {
            hasError = false;

            Scanner scanner = new Scanner(source);
            List<Token> tokens = scanner.ScanLines();

            Parser parser = new Parser(tokens, FunctionLibrary) { ReportTokenError = ReportTokenError };
            IEnumerable<Statement> statements = parser.Parse();

            if(hasError)
            {
                return;
            }

            Interpreter interpreter = new Interpreter() { ReportRuntimeError = ReportRuntimeError };
            interpreter.Interpret(statements);
        }

        private void ReportTokenError(Token token, string message)
        {
            hasError = true;
            if(token?.type == TokenType.EOF)
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
