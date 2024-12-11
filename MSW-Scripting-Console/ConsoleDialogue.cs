using MSW.Reflection;

namespace MSW.Scripting.Console
{
    public class ConsoleDialogue
    {
        [MSWFunction("{0}: {1}")]
        public void SayLine(string name, string dialogue)
        {
            System.Console.WriteLine($"{name} says: {dialogue}");
        }
    }
}