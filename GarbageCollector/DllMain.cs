using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace GarbageCollector;

public class DllMain
{
    [UnmanagedCallersOnly(EntryPoint = "Custom_GC_Initialize")]
    public static unsafe HResult GC_Initialize(IntPtr clrToGC, IntPtr* gcHeap, IntPtr* gcHandleManager, GcDacVars* gcDacVars)
    {
        Console.WriteLine("[GC] GC_Initialize");

        var gc = new GCHeap(NativeObjects.IGCToCLR.Wrap(clrToGC));

        *gcHeap = gc.Object;
        *gcHandleManager = gc.GCHandleManager;

        return HResult.S_OK;
    }

    [UnmanagedCallersOnly(EntryPoint = "Custom_GC_VersionInfo", CallConvs = new[] { typeof(CallConvCdecl) })]
    public static unsafe HResult GC_VersionInfo(VersionInfo* versionInfo)
    {
        Console.WriteLine("[GC] GC_VersionInfo");

        (*versionInfo).MajorVersion = 5;
        (*versionInfo).MinorVersion = 1;

        return HResult.S_OK;
    }

}
