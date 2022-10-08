using System.Runtime.InteropServices;

namespace lldb
{
    [StructLayout(LayoutKind.Explicit, Size = 16)]
    [CppObject]
    public unsafe partial struct SBCommandInterpreter
    {
        public partial SBCommand AddCommand(char* name, IntPtr impl, char* help);

    }
}
