using System;
using System.Collections.Generic;
using UnityEngine;

namespace MSW.Scripting
{
    public class MSWInterpreter
    {
        private void Run(string source)
        {
            MSWScanner scanner = new MSWScanner() { ReportError = Report };
            List<MSWToken> tokens = scanner.ScanTokens(source);

            foreach (MSWToken token in tokens)
            {
                Debug.Log(token);
            }
        }

        private void Report(int line, string where, string message)
        {
            Debug.Log($"[Line {line} - {where}]: {message}");
        }
    }
}
