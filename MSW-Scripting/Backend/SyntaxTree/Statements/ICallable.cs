using System.Collections.Generic;

namespace MSW.Scripting
{
    public interface ICallable
    {
        int Arity();
        object Call(MSWInterpreter interpreter, List<object> arguments);
    }
}