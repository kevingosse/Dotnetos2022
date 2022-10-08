using Profiler.StackWalking;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Profiler;

internal unsafe class CorProfilerCallback : ICorProfilerCallback2
{
    private ICorProfilerInfo3 _corProfilerInfo;
    public NativeObjects.ICorProfilerCallback Object {get;}

    public CorProfilerCallback()
    {
        Object = NativeObjects.ICorProfilerCallback.Wrap(this);
    }

    public int QueryInterface(in Guid guid, out IntPtr ptr)
    {
        Console.WriteLine("ICorProfilerCallback QueryInterface");

        if (guid == KnownGuids.ICorProfilerCallback2)
        {
            Console.WriteLine("[Profiler] Returning instance of ICorProfilerCallback2");
            ptr = Object;

            return HResult.S_OK;
        }

        ptr = default;
        return 0;
    }

    public int AddRef()
    {
        Console.WriteLine("ICorProfilerCallback AddRef");
        return 0;
    }

    public int Release()
    {
        Console.WriteLine("ICorProfilerCallback Release");
        return 0;
    }

    public HResult Initialize(IntPtr pICorProfilerInfoUnk)
    {
        Console.WriteLine("ICorProfilerCallback Initialize");

        var iunknown = NativeObjects.IUnknown.Wrap(pICorProfilerInfoUnk);

        int result = iunknown.QueryInterface(in KnownGuids.ICorProfilerInfo3, out var ptr);

        if (result == 0)
        {
            Console.WriteLine("Success: - " + ptr);
            _corProfilerInfo = NativeObjects.ICorProfilerInfo3.Wrap(ptr);
        }
        else
        {
            Console.WriteLine("Failure");

            return HResult.E_FAIL;
        }

        _corProfilerInfo.SetEventMask(CorPrfMonitor.COR_PRF_MONITOR_EXCEPTIONS | CorPrfMonitor.COR_PRF_ENABLE_STACK_SNAPSHOT);

        return HResult.S_OK;
    }

    public HResult Shutdown()
    {
        return default;
    }

    public HResult AppDomainCreationStarted(AppDomainId appDomainId)
    {
        return default;
    }

    public HResult AppDomainCreationFinished(AppDomainId appDomainId, HResult hrStatus)
    {
        return default;
    }

    public HResult AppDomainShutdownStarted(AppDomainId appDomainId)
    {
        return default;
    }

    public HResult AppDomainShutdownFinished(AppDomainId appDomainId, HResult hrStatus)
    {
        return default;
    }

    public HResult AssemblyLoadStarted(AssemblyId assemblyId)
    {
        return default;
    }

    public HResult AssemblyLoadFinished(AssemblyId assemblyId, HResult hrStatus)
    {
        return default;
    }

    public HResult AssemblyUnloadStarted(AssemblyId assemblyId)
    {
        return default;
    }

    public HResult AssemblyUnloadFinished(AssemblyId assemblyId, HResult hrStatus)
    {
        return default;
    }

    public HResult ModuleLoadStarted(ModuleId moduleId)
    {
        return default;
    }

    public HResult ModuleLoadFinished(ModuleId moduleId, HResult hrStatus)
    {
        return default;
    }

    public HResult ModuleUnloadStarted(ModuleId moduleId)
    {
        return default;
    }

    public HResult ModuleUnloadFinished(ModuleId moduleId, HResult hrStatus)
    {
        return default;
    }

    public HResult ModuleAttachedToAssembly(ModuleId moduleId, AssemblyId assemblyId)
    {
        return default;
    }

    public HResult ClassLoadStarted(ClassId classId)
    {
        return default;
    }

    public HResult ClassLoadFinished(ClassId classId, HResult hrStatus)
    {
        return default;
    }

    public HResult ClassUnloadStarted(ClassId classId)
    {
        return default;
    }

    public HResult ClassUnloadFinished(ClassId classId, HResult hrStatus)
    {
        return default;
    }

    public HResult FunctionUnloadStarted(FunctionId functionId)
    {
        return default;
    }

    public HResult JITCompilationStarted(FunctionId functionId, bool fIsSafeToBlock)
    {
        return default;
    }

    public HResult JITCompilationFinished(FunctionId functionId, HResult hrStatus, bool fIsSafeToBlock)
    {
        return default;
    }

    public HResult JITCachedFunctionSearchStarted(FunctionId functionId, out bool pbUseCachedFunction)
    {
        pbUseCachedFunction = false;

        return default;
    }

    public HResult JITCachedFunctionSearchFinished(FunctionId functionId, COR_PRF_JIT_CACHE result)
    {
        return default;
    }

    public HResult JITFunctionPitched(FunctionId functionId)
    {
        return default;
    }

    public HResult JITInlining(FunctionId callerId, FunctionId calleeId, out bool pfShouldInline)
    {
        pfShouldInline = false;

        return default;
    }

    public HResult ThreadCreated(ThreadId threadId)
    {
        return default;
    }

    public HResult ThreadDestroyed(ThreadId threadId)
    {
        return default;
    }

    public HResult ThreadAssignedToOSThread(ThreadId managedThreadId, int osThreadId)
    {
        return default;
    }

    public HResult RemotingClientInvocationStarted()
    {
        return default;
    }

    public HResult RemotingClientSendingMessage(in Guid pCookie, bool fIsAsync)
    {
        return default;
    }

    public HResult RemotingClientReceivingReply(in Guid pCookie, bool fIsAsync)
    {
        return default;
    }

    public HResult RemotingClientInvocationFinished()
    {
        return default;
    }

    public HResult RemotingServerReceivingMessage(in Guid pCookie, bool fIsAsync)
    {
        return default;
    }

    public HResult RemotingServerInvocationStarted()
    {
        return default;
    }

    public HResult RemotingServerInvocationReturned()
    {
        return default;
    }

    public HResult RemotingServerSendingReply(in Guid pCookie, bool fIsAsync)
    {
        return default;
    }

    public HResult UnmanagedToManagedTransition(FunctionId functionId, COR_PRF_TRANSITION_REASON reason)
    {
        return default;
    }

    public HResult ManagedToUnmanagedTransition(FunctionId functionId, COR_PRF_TRANSITION_REASON reason)
    {
        return default;
    }

    public HResult RuntimeSuspendStarted(COR_PRF_SUSPEND_REASON suspendReason)
    {
        return default;
    }

    public HResult RuntimeSuspendFinished()
    {
        return default;
    }

    public HResult RuntimeSuspendAborted()
    {
        return default;
    }

    public HResult RuntimeResumeStarted()
    {
        return default;
    }

    public HResult RuntimeResumeFinished()
    {
        return default;
    }

    public HResult RuntimeThreadSuspended(ThreadId threadId)
    {
        return default;
    }

    public HResult RuntimeThreadResumed(ThreadId threadId)
    {
        return default;
    }

    public unsafe HResult MovedReferences(uint cMovedObjectIDRanges, ObjectId* oldObjectIDRangeStart, ObjectId* newObjectIDRangeStart, uint* cObjectIDRangeLength)
    {
        return default;
    }

    public HResult ObjectAllocated(ObjectId objectId, ClassId classId)
    {
        return default;
    }

    public unsafe HResult ObjectsAllocatedByClass(uint cClassCount, ClassId* classIds, uint* cObjects)
    {
        return default;
    }

    public unsafe HResult ObjectReferences(ObjectId objectId, ClassId classId, uint cObjectRefs, ObjectId* objectRefIds)
    {
        return default;
    }

    public unsafe HResult RootReferences(uint cRootRefs, ObjectId* rootRefIds)
    {
        return default;
    }

    public HResult ExceptionThrown(ObjectId thrownObjectId)
    {
        Console.WriteLine("ExceptionThrown");

        _corProfilerInfo.GetClassFromObject(thrownObjectId, out var classId);
        _corProfilerInfo.GetClassIdInfo(classId, out var moduleId, out var typeDef);
        _corProfilerInfo.GetModuleMetaData(moduleId, CorOpenFlags.ofRead, KnownGuids.IMetaDataImport, out IntPtr ppOut);

        var metaDataImport = NativeObjects.IMetaDataImport.Wrap(ppOut);

        metaDataImport.GetTypeDefProps(typeDef, null, 0, out var nameCharCount, out _, out _);

        Span<char> buffer = stackalloc char[(int)nameCharCount];

        fixed (char* p = buffer)
        {
            metaDataImport.GetTypeDefProps(typeDef, p, nameCharCount, out _, out _, out _);
        }
            
        Console.WriteLine("[Profiler] An exception was thrown: " + new string(buffer));

        var stackWalker = new StackWalker(_corProfilerInfo);
        var stackTrace = stackWalker.Walk();

        if (stackTrace == null)
        {
            Console.WriteLine("Error while walking the stack");
        }
        else
        {
            Console.WriteLine(string.Join(Environment.NewLine, stackTrace));
        }

        return HResult.S_OK;
    }

    private void WalkStack()
    {
        _corProfilerInfo.GetCurrentThreadId(out var threadId);

        var buffer = new StackSnapshotBuffer();

        var result = _corProfilerInfo.DoStackSnapshot(threadId, &StackSnapshotCallback, COR_PRF_SNAPSHOT_INFO.COR_PRF_SNAPSHOT_DEFAULT, Unsafe.AsPointer(ref buffer), null, 0);

        Console.WriteLine("WalkStack result: " + result);
        Console.WriteLine(buffer);
    }

    [UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvStdcall) })]
    private static HResult StackSnapshotCallback(FunctionId functionId, nint ip, COR_PRF_FRAME_INFO frameInfo, uint contextSize, byte* context, void* clientData)
    {
        ref var buffer = ref Unsafe.AsRef<StackSnapshotBuffer>(clientData);

        return buffer.Add(ip) ? HResult.S_OK : HResult.E_FAIL;
    }


    public HResult ExceptionSearchFunctionEnter(FunctionId functionId)
    {
        return default;
    }

    public HResult ExceptionSearchFunctionLeave()
    {
        return default;
    }

    public HResult ExceptionSearchFilterEnter(FunctionId functionId)
    {
        return default;
    }

    public HResult ExceptionSearchFilterLeave()
    {
        return default;
    }

    public HResult ExceptionSearchCatcherFound(FunctionId functionId)
    {
        return default;
    }

    public unsafe HResult ExceptionOSHandlerEnter(nint* __unused)
    {
        return default;
    }

    public unsafe HResult ExceptionOSHandlerLeave(nint* __unused)
    {
        return default;
    }

    public HResult ExceptionUnwindFunctionEnter(FunctionId functionId)
    {
        return default;
    }

    public HResult ExceptionUnwindFunctionLeave()
    {
        return default;
    }

    public HResult ExceptionUnwindFinallyEnter(FunctionId functionId)
    {
        return default;
    }

    public HResult ExceptionUnwindFinallyLeave()
    {
        return default;
    }

    public HResult ExceptionCatcherEnter(FunctionId functionId, ObjectId objectId)
    {
        return default;
    }

    public HResult ExceptionCatcherLeave()
    {
        return default;
    }

    public unsafe HResult COMClassicVTableCreated(ClassId wrappedClassId, in Guid implementedIID, void* pVTable, uint cSlots)
    {
        return default;
    }

    public unsafe HResult COMClassicVTableDestroyed(ClassId wrappedClassId, in Guid implementedIID, void* pVTable)
    {
        return default;
    }

    public HResult ExceptionCLRCatcherFound()
    {
        return default;
    }

    public HResult ExceptionCLRCatcherExecute()
    {
        return default;
    }

    public unsafe HResult ThreadNameChanged(ThreadId threadId, uint cchName, char* name)
    {
        return default;
    }

    public unsafe HResult GarbageCollectionStarted(int cGenerations, bool* generationCollected, COR_PRF_GC_REASON reason)
    {
        return default;
    }

    public unsafe HResult SurvivingReferences(uint cSurvivingObjectIDRanges, ObjectId* objectIDRangeStart, uint* cObjectIDRangeLength)
    {
        return default;
    }

    public HResult GarbageCollectionFinished()
    {
        return default;
    }

    public HResult FinalizeableObjectQueued(int finalizerFlags, ObjectId objectID)
    {
        return default;
    }

    public unsafe HResult RootReferences2(uint cRootRefs, ObjectId* rootRefIds, COR_PRF_GC_ROOT_KIND* rootKinds, COR_PRF_GC_ROOT_FLAGS* rootFlags, uint* rootIds)
    {
        return default;
    }

    public HResult HandleCreated(GCHandleId handleId, ObjectId initialObjectId)
    {
        return default;
    }

    public HResult HandleDestroyed(GCHandleId handleId)
    {
        return default;
    }
}