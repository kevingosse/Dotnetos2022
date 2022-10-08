using System.Runtime.InteropServices;

namespace lldb;

[CppObject]
[StructLayout(LayoutKind.Explicit, Size = 16)]
public unsafe partial struct SBFileSpec
{
    public partial IntPtr GetFilename();
    public partial IntPtr GetDirectory();
}