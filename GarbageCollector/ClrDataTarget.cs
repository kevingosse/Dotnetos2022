using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace GarbageCollector;

public unsafe class ClrDataTarget : ICLRDataTarget2
{
    private readonly NativeObjects.ICLRDataTarget _clrDataTargetStub;

    public ClrDataTarget()
    {
        _clrDataTargetStub = NativeObjects.ICLRDataTarget.Wrap(this);
    }

    public IntPtr ICLRDataTargetObject => _clrDataTargetStub;

    public HResult QueryInterface(in Guid guid, out IntPtr ptr)
    {
        if (guid == KnownGuids.IClrDataTarget2)
        {
            ptr = _clrDataTargetStub;
            return HResult.S_OK;
        }

        ptr = default;
        return HResult.E_NOINTERFACE;
    }

    public int AddRef()
    {
        return 1;
    }

    public int Release()
    {
        return 1;
    }

    public HResult GetMachineType(out uint machine)
    {
        machine = 0x8664; // IMAGE_FILE_MACHINE_AMD64
        return HResult.S_OK;
    }

    public HResult GetPointerSize(out uint size)
    {
        size = (uint)IntPtr.Size;
        return HResult.S_OK;
    }

    public HResult GetImageBase(char* moduleName, out CLRDATA_ADDRESS baseAddress)
    {
        // TODO: do it properly
        var name = new string(moduleName);

        foreach (ProcessModule module in Process.GetCurrentProcess().Modules)
        {
            if (module.ModuleName == name)
            {
                baseAddress = new CLRDATA_ADDRESS(module.BaseAddress.ToInt64());
                return HResult.S_OK;
            }
        }

        baseAddress = default;
        return HResult.E_NOTIMPL;
    }

    public HResult ReadVirtual(CLRDATA_ADDRESS address, byte* buffer, uint size, out uint done)
    {
        Unsafe.CopyBlock(buffer, (void*)(IntPtr)address.Value, size);
        done = size;
        return HResult.S_OK;
    }

    public HResult WriteVirtual(CLRDATA_ADDRESS address, byte* buffer, uint size, out uint done)
    {
        done = default;
        return HResult.E_NOTIMPL;
    }

    public HResult GetTLSValue(uint threadID, uint index, out CLRDATA_ADDRESS value)
    {
        value = default;
        return HResult.E_NOTIMPL;
    }

    public HResult SetTLSValue(uint threadID, uint index, CLRDATA_ADDRESS value)
    {
        return HResult.E_NOTIMPL;
    }

    public HResult GetCurrentThreadID(out uint threadID)
    {
        Console.WriteLine("ClrDataTarget - GetCurrentThreadID");
        threadID = default;
        return HResult.E_NOTIMPL;
    }

    public HResult GetThreadContext(uint threadID, uint contextFlags, uint contextSize, byte* context)
    {
        Console.WriteLine("ClrDataTarget - GetThreadContext");
        return HResult.E_NOTIMPL;
    }

    public HResult SetThreadContext(uint threadID, uint contextSize, byte* context)
    {
        Console.WriteLine("ClrDataTarget - SetThreadContext");
        return HResult.E_NOTIMPL;
    }

    public HResult Request(uint reqCode, uint inBufferSize, byte* inBuffer, uint outBufferSize, byte* outBuffer)
    {
        Console.WriteLine("ClrDataTarget - Request");
        return HResult.E_NOTIMPL;
    }

    public HResult AllocVirtual(CLRDATA_ADDRESS addr, uint size, uint typeFlags, uint protectFlags, out CLRDATA_ADDRESS virt)
    {
        Console.WriteLine("ClrDataTarget - AllocVirtual");
        virt = default;
        return HResult.E_NOTIMPL;
    }

    public HResult FreeVirtual(CLRDATA_ADDRESS addr, uint size, uint typeFlags)
    {
        Console.WriteLine("ClrDataTarget - FreeVirtual");
        return HResult.E_NOTIMPL;
    }
}