using MSW.Reflection;

namespace MSW.Console
{
    public class ConsoleDialogue
    {
        [MSWFunction("{0}: {1}")]
        public object RunDialogue(string name, string line)
        {
            System.Console.WriteLine($"{name} says: {line}");
            return null;
        }
    }
}