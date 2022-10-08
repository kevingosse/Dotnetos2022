using System.Runtime.InteropServices;
using static GarbageCollector.Log;

namespace GarbageCollector;

public unsafe class GCHandleStore : IGCHandleStore
{
    private readonly nint* _store;
    private int _handleCount;

    public GCHandleStore()
    {
        _store = (nint*)Marshal.AllocHGlobal(sizeof(nint) * 65535);

        for (int i = 0; i < 65535; i++)
        {
            _store[i] = 0;
        }

        Object = NativeObjects.IGCHandleStore.Wrap(this);
    }

    public NativeObjects.IGCHandleStore Object { get; }

    public void Uproot()
    {
        Console.WriteLine("GCHandleStore Uproot");
    }

    public bool ContainsHandle(OBJECTHANDLE handle)
    {
        Console.WriteLine("GCHandleStore ContainsHandle");
        return false;
    }

    public void DumpHandles(ISOSDacInterface dac)
    {
        Console.WriteLine($"Store location: {(nint)_store:x2}");

        var knownReferences = new HashSet<nint>();

        for (int i = 0; i < _handleCount; i++)
        {
            var target = *(_store + i);

            if (target == 0)
            {
                continue;
            }

            DumpObject(target, dac, knownReferences);
        }
    }

    private void DumpObject(nint target, ISOSDacInterface dac, HashSet<nint> knownReferences, int depth = 0)
    {
        if (!knownReferences.Add(target))
        {
            return;
        }

        var mtPtr = *(nint*)target;

        dac.GetMethodTableData(new CLRDATA_ADDRESS(mtPtr), out var mtData);

        var typeName = dac.GetObjectTypeName(new CLRDATA_ADDRESS(target));

        Write($"{new string(' ', depth * 2)} - {typeName ?? "{null}"}");

        if (!mtData.bContainsPointers)
        {
            return;
        }

        var entries = *(nint*)(mtPtr - sizeof(nint));

        if (entries < 0)
        {
            entries *= -1;
        }

        var slots = 1 + entries * 2;

        var buffer = (byte*)(mtPtr - slots * IntPtr.Size);

        var gcDesc = new GCDesc(buffer, (int)slots * IntPtr.Size);

        var size = mtData.BaseSize;

        if (mtData.ComponentSize != 0)
        {
            var length = *(int*)(target + IntPtr.Size);
            size += length * mtData.ComponentSize;
        }

        foreach (var reference in gcDesc.WalkObject((IntPtr)target, size))
        {
            DumpObject(reference.ReferencedObject, dac, knownReferences, depth + 1);
        }
    }

    public unsafe OBJECTHANDLE CreateHandleOfType(GCObject* obj, HandleType type)
    {
        var handle = _store + _handleCount;

        *handle = (nint)obj;

        _handleCount++;

        return new OBJECTHANDLE((nint)handle);
    }

    public unsafe OBJECTHANDLE CreateHandleOfType2(GCObject* obj, HandleType type, int heapToAffinitizeTo)
    {
        Console.WriteLine($"GCHandleStore CreateHandleOfType2 - {(nint)obj:x2}");

        var handle = _store + _handleCount;

        *handle = (nint)obj;

        var result = new OBJECTHANDLE((nint)handle);
        _handleCount++;

        return result;
    }

    public unsafe OBJECTHANDLE CreateHandleWithExtraInfo(GCObject* obj, HandleType type, void* pExtraInfo)
    {
        Console.WriteLine("GCHandleStore CreateHandleWithExtraInfo");

        return new OBJECTHANDLE((nint)_store + (_handleCount++));
    }

    public unsafe OBJECTHANDLE CreateDependentHandle(GCObject* primary, GCObject* secondary)
    {
        Console.WriteLine("GCHandleStore CreateDependentHandle");

        return new OBJECTHANDLE((nint)_store + (_handleCount++));
    }

    public void Destructor()
    {
        Console.WriteLine("Destructor");
    }
}
