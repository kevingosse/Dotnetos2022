using System.Runtime.InteropServices;

namespace lldb;

[CppObject]
[StructLayout(LayoutKind.Explicit, Size = 16)]
public unsafe partial struct SBModule
{
    public partial lldb.SBFileSpec GetFileSpec();
}