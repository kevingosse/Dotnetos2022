using Microsoft.Diagnostics.Runtime.DbgEng;
using Microsoft.Diagnostics.Runtime;
using System.Runtime.InteropServices;

namespace WindbgExtension;

public unsafe class Plugin
{
    [UnmanagedCallersOnly(EntryPoint = "DebugExtensionInitialize")]
    public static unsafe int DebugExtensionInitialize(uint* version, uint* flags)
    {
        *version = (1 & 0xffff) << 16;
        *flags = 0;
        return 0;
    }

    [UnmanagedCallersOnly(EntryPoint = "mycommand")]
    public static int MyCommand(IntPtr client, IntPtr argsPtr)
    {
        var args = Marshal.PtrToStringAnsi(argsPtr);

        var refCount = new RefCountedFreeLibrary(client);
        var debugControl = new DebugControl(refCount, client, new DebugSystemObjects(refCount, client));

        debugControl.ControlledOutput(1, 1, "Hello " + args);

        return 0;
    }
}