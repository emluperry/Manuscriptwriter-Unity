using System.Collections.Generic;

namespace MSW.Scripting.NativeFunctions
{
    public class RunDialogue : ICallable
    {
        public int Arity()
        {
            return 2;
        }

        public object Call(MSWInterpreter interpreter, List<object> arguments)
        {
            System.Console.WriteLine($"{arguments[0]} says: {arguments[1]}");
            return null;
        }
    }
}