namespace GarbageCollector;

[NativeObject]
public interface IUnknown
{
    HResult QueryInterface(in Guid guid, out IntPtr ptr);
    int AddRef();
    int Release();
}