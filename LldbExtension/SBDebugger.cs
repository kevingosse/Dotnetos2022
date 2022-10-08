using System.Runtime.InteropServices;

namespace lldb;

[CppObject]
[StructLayout(LayoutKind.Explicit, Size = 16)]
public unsafe partial struct SBDebugger
{
    public partial SBCommandInterpreter GetCommandInterpreter();

    public partial SBTarget GetSelectedTarget();
}
