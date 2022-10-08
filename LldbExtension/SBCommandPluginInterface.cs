using lldb;

namespace LldbExtension;

[NativeObject]
internal unsafe interface SBCommandPluginInterface
{
    void Destructor1();
    void Destructor2();
    bool DoExecute(SBDebugger* debugger, char** command, lldb.SBCommandReturnObject* result);
}