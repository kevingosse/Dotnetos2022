namespace Profiler;

[NativeObject]
public interface IUnknown
{
    int QueryInterface(in Guid guid, out IntPtr ptr);

    int AddRef();

    int Release();
}