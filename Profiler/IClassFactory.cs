namespace Profiler;

[NativeObject]
public interface IClassFactory : IUnknown
{
    int CreateInstance(IntPtr outer, in Guid guid, out IntPtr instance);

    int LockServer(bool @lock);

}