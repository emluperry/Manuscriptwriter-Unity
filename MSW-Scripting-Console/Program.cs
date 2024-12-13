using System.Collections.Generic;
using System.IO;
using MSW.Compiler;

namespace MSW.Console
{
    internal class Program
    {
        static void Main(string[] args)
        {
            //InterpretFile(args[0]);
            
            RunCompiler(args[0]);
        }

        static void LogError(string message)
        {
            System.Console.WriteLine(message);
        }

        static void InterpretFile(string filePath)
        {
            string data = File.ReadAllText(filePath);
            if (string.IsNullOrEmpty(data))
            {
                System.Console.WriteLine("File is empty.");
                return;
            }
            
            // Output the file for debugging.
            System.Console.WriteLine(data);
            System.Console.WriteLine("///////////////////////////////// OUTPUT FOLLOWS ////////////////////////////////");
            
            // Run the debug code.
            var runner = new Runner() { ErrorLogger = Program.LogError, DebugOutput = Program.LogError };
            runner.Run(data);
        }

        static void RunCompiler(string filePath)
        {
            string data = File.ReadAllText(filePath);
            if (string.IsNullOrEmpty(data))
            {
                System.Console.WriteLine("File is empty.");
                return;
            }

            var comp = new Compiler.Compiler()
            {
                ErrorLogger = LogError,
                FunctionLibrary = new List<object>() { new ConsoleDialogue() }
            };

            comp.Compile(data);
        }
    }
}
