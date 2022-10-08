using System.Runtime.InteropServices;

namespace Profiler;

public class DllMain
{
    private static ClassFactory Factory;

    [UnmanagedCallersOnly(EntryPoint = "DllGetClassObject")]
    public static unsafe int DllGetClassObject(Guid* rclsid, Guid* riid, IntPtr* ppv)
    {
        Console.WriteLine("DllGetClassObject");

        Factory = new();

        *ppv = Factory.Object;

        return 0;
    }

    [UnmanagedCallersOnly]
    public static unsafe int QueryInterface(IntPtr self, Guid* guid, IntPtr* ptr)
    {
        Console.WriteLine("QueryInterface");
        *ptr = IntPtr.Zero;
        return 0;
    }

    [UnmanagedCallersOnly]
    public static int AddRef(IntPtr self)
    {
        Console.WriteLine("AddRef");
        return 1;
    }

    [UnmanagedCallersOnly]
    public static int Release(IntPtr self)
    {
        Console.WriteLine("Release");
        return 1;
    }

    [UnmanagedCallersOnly]
    public static unsafe int CreateInstance(IntPtr self, IntPtr outer, Guid* guid, IntPtr* instance)
    {
        Console.WriteLine("CreateInstance");
        Console.WriteLine(self.ToString("x2"));

            
        return 0;
    }

    [UnmanagedCallersOnly]
    public static int LockServer(IntPtr self, bool @lock)
    {
        Console.WriteLine("LockServer");
        return 0;
    }
}