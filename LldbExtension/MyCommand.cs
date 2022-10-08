using lldb;

namespace LldbExtension;

public class MyCommand : SBCommandPluginInterface
{
    public void Destructor1()
    {
    }

    public void Destructor2()
    {
    }

    public unsafe bool DoExecute(SBDebugger* debugger, char** command, SBCommandReturnObject* result)
    {
        Console.WriteLine("OK");

        return true;
    }
}