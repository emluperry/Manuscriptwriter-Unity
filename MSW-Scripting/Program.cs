using MSW.Scripting;

class Program
{
    static void Main(string[] args)
    {
        string data = File.ReadAllText(args[0]);
        if(string.IsNullOrEmpty(data))
        {
            Console.WriteLine("File is empty.");
            return;
        }

        Console.WriteLine(data);

        var runner = new MSWRunner() { ErrorLogger = Program.LogError, DebugOutput = Program.LogError };
        runner.Run(data);
    }

    static void LogError(string message)
    {
        Console.WriteLine(message);
    }
}