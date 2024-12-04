using System.IO;

namespace MSW.Scripting.Console
{
    internal class Program
    {
        static void Main(string[] args)
        {
            string data = File.ReadAllText(args[0]);
            if (string.IsNullOrEmpty(data))
            {
                System.Console.WriteLine("File is empty.");
                return;
            }

            System.Console.WriteLine(data);

            var runner = new MSWRunner() { ErrorLogger = Program.LogError, DebugOutput = Program.LogError };
            runner.Run(data);
        }

        static void LogError(string message)
        {
            System.Console.WriteLine(message);
        }
    }
}
