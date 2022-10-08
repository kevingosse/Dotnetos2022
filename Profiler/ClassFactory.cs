namespace Profiler;

public unsafe class ClassFactory : IClassFactory
{
    private static CorProfilerCallback Callback;

    public NativeObjects.IClassFactory Object { get; }

    public ClassFactory()
    {
        Object = NativeObjects.IClassFactory.Wrap(this);
    }

    public int QueryInterface(in Guid guid, out IntPtr ptr)
    {
        Console.WriteLine("QueryInterface");
        ptr = IntPtr.Zero;
        return 0;
    }

    public int AddRef()
    {
        Console.WriteLine("AddRef");
        return 1;
    }

    public int Release()
    {
        Console.WriteLine("Release");
        return 1;
    }

    public int CreateInstance(IntPtr outer, in Guid guid, out IntPtr instance)
    {
        Console.WriteLine("CreateInstance");

        Callback = new CorProfilerCallback();

        instance = Callback.Object;
            
        return 0;
    }

    public int LockServer(bool @lock)
    {
        Console.WriteLine("LockServer");
        return 0;
    }
}