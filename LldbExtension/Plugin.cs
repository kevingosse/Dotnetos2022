using lldb;
using System.Runtime.InteropServices;

namespace LldbExtension;

public class Plugin
{
    [UnmanagedCallersOnly(EntryPoint = "_ZN4lldb16PluginInitializeENS_10SBDebuggerE")]
    public static unsafe bool PluginInitialize(SBDebugger* debugger)
    {
        var interpreter = debugger->GetCommandInterpreter();

        var name = Marshal.StringToHGlobalAnsi("mycommand");
        var help = Marshal.StringToHGlobalAnsi("Help text");

        var obj = NativeObjects.SBCommandPluginInterface.Wrap(new MyCommand());

        var command = interpreter.AddCommand((char*)name, obj.Object, (char*)help);

        return true;
    }
}