using System.Runtime.InteropServices;

namespace lldb;

[CppObject]
[StructLayout(LayoutKind.Explicit, Size = 16)]
public unsafe partial struct SBTarget
{
    public partial bool IsValid();

    public partial uint GetNumModules();

    public partial lldb.SBModule GetModuleAtIndex(uint index);
}