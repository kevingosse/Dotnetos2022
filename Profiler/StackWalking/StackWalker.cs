using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Profiler.StackWalking;

internal unsafe class StackWalker
{
    private const string Unknown = "{unknown}";
    private record struct TypeDesc(string Assembly, string Type);
    private readonly ICorProfilerInfo2 _corProfilerInfo;

    public StackWalker(ICorProfilerInfo2 corProfilerInfo)
    {
        _corProfilerInfo = corProfilerInfo;
    }

    public string[] Walk()
    {
        _corProfilerInfo.GetCurrentThreadId(out var threadId);

        var buffer = new StackSnapshotBuffer();

        var result = _corProfilerInfo.DoStackSnapshot(threadId, &StackSnapshotCallback, COR_PRF_SNAPSHOT_INFO.COR_PRF_SNAPSHOT_DEFAULT, Unsafe.AsPointer(ref buffer), null, 0);

        if (!result.IsOK)
        {
            return null;
        }

        var stackTrace = new string[buffer.Count];

        for (int i = 0; i < buffer.Count; i++)
        {
            stackTrace[i] = Resolve((nint)buffer.InstructionPointers[i]);
        }

        return stackTrace;
    }

    [UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvStdcall) })]
    private static HResult StackSnapshotCallback(FunctionId functionId, nint ip, COR_PRF_FRAME_INFO frameInfo, uint contextSize, byte* context, void* clientData)
    {
        ref var buffer = ref Unsafe.AsRef<StackSnapshotBuffer>(clientData);

        return buffer.Add(ip) ? HResult.S_OK : HResult.E_FAIL;
    }

    private string Resolve(nint instructionPointer)
    {
        var hr = _corProfilerInfo.GetFunctionFromIP(instructionPointer, out var functionId);

        if (hr)
        {
            return GetManagedFrame(functionId);
        }

        return Unknown;
    }

    private string GetManagedFrame(FunctionId functionId)
    {

        if (!GetFunctionInfo(
            functionId,
            out var mdTokenFunc,
            out var classId,
            out var moduleId,
            out var genericParametersCount,
            out var genericParameters))
        {
            return Unknown;
        }

        if (!GetMetadataApi(moduleId, functionId, out var metaDataImport))
        {
            return Unknown;
        }

        try
        {
            var (methodName, mdTokenType) = GetMethodNameFromMetadata(metaDataImport, new MdMethodDef(mdTokenFunc));

            if (methodName.Length == 0)
            {
                return Unknown;
            }

            if (!GetTypeDesc(metaDataImport, classId, moduleId, mdTokenType, out var typeDesc))
            {
                return Unknown;
            }

            return $"{typeDesc.Assembly}!{typeDesc.Type}.{methodName}";
        }
        finally
        {
            metaDataImport.Release();
        }
    }

    private bool GetTypeDesc(IMetaDataImport2 metadataImport, ClassId classId, ModuleId moduleId, MdTypeDef mdTokenType, out TypeDesc typeDesc)
    {
        typeDesc = default;

        if (!GetAssemblyName(_corProfilerInfo, moduleId, out var assemblyName))
        {
            return false;
        }

        typeDesc.Assembly = assemblyName;

        var (ns, typeName) = GetTypeWithNamespace(metadataImport, mdTokenType);
        typeDesc.Type = $"{ns}.{typeName}";

        return true;
    }

    private static unsafe bool GetAssemblyName(ICorProfilerInfo2 info, ModuleId moduleId, out string assemblyName)
    {
        assemblyName = string.Empty;

        var hr = info.GetModuleInfo(moduleId, out _, 0, out _, null, out var assemblyId);

        if (!hr)
        {
            return false;
        }

        hr = info.GetAssemblyInfo(assemblyId, 0, out var nameCharCount, null, out _, out _);

        if (!hr)
        {
            return false;
        }

        Span<char> buffer = stackalloc char[(int)nameCharCount];

        fixed (char* b = buffer)
        {
            hr = info.GetAssemblyInfo(assemblyId, nameCharCount, out nameCharCount, b, out _, out _);

            if (!hr)
            {
                return false;
            }
        }

        assemblyName = new string(buffer);
        return true;
    }

    private static (string ns, string typeName) GetTypeWithNamespace(IMetaDataImport2 metadata, MdTypeDef mdTokenType)
    {
        var hr = metadata.GetNestedClassProps(mdTokenType, out var mdEnclosingType);
        var isNested = hr.IsOK && metadata.IsValidToken(new MdToken(mdEnclosingType.Value));

        var enclosingType = string.Empty;
        var ns = string.Empty;

        if (isNested)
        {
            (ns, enclosingType) = GetTypeWithNamespace(metadata, mdEnclosingType);
        }

        var typeName = GetTypeNameFromMetadata(metadata, mdTokenType);

        if (typeName.Length == 0)
        {
            typeName = "?";
        }

        if (isNested)
        {
            return (ns, $"{enclosingType}.{typeName}");
        }

        var pos = typeName.LastIndexOf('.');

        if (pos == -1)
        {
            return (string.Empty, typeName);
        }

        return (typeName.Substring(0, pos), typeName.Substring(pos + 1));
    }

    private static unsafe string GetTypeNameFromMetadata(IMetaDataImport2 metaData, MdTypeDef mdTokenType)
    {
        var hr = metaData.GetTypeDefProps(mdTokenType, null, 0, out var nameCharCount, out _, out _);

        if (!hr)
        {
            return string.Empty;
        }

        Span<char> buffer = stackalloc char[(int)nameCharCount];

        fixed (char* b = buffer)
        {
            hr = metaData.GetTypeDefProps(mdTokenType, b, nameCharCount, out nameCharCount, out _, out _);

            if (!hr)
            {
                return string.Empty;
            }
        }

        return new string(buffer);
    }

    private unsafe (string methodName, MdTypeDef typeDef) GetMethodNameFromMetadata(IMetaDataImport2 metaDataImport, MdMethodDef mdTokenFunc)
    {
        var hr = metaDataImport.GetMethodProps(mdTokenFunc, out _, null, 0, out var nameCharCount, out _, out _, out _, out _, out _);

        if (!hr.IsOK)
        {
            return (string.Empty, default);
        }

        MdTypeDef mdTokenType;
        Span<char> buffer = stackalloc char[(int)nameCharCount];

        fixed (char* b = buffer)
        {
            hr = metaDataImport.GetMethodProps(mdTokenFunc, out mdTokenType, b, nameCharCount, out nameCharCount, out _, out _, out _, out _, out _);

            if (!hr.IsOK)
            {
                return (string.Empty, default);
            }
        }

        return (new string(buffer), mdTokenType);
    }

    private unsafe bool GetFunctionInfo(
    FunctionId functionId,
    out MdToken mdTokenFunc,
    out ClassId classId,
    out ModuleId moduleId,
    out uint genericParametersCount,
    out ClassId[] genericParameters)
    {
        var hr = _corProfilerInfo.GetFunctionInfo2(
        functionId,
        default,
        out classId,
        out moduleId,
        out mdTokenFunc,
        0,
        out genericParametersCount,
        null);

        if (!hr.IsOK)
        {
            genericParameters = null;
            return false;
        }

        if (genericParametersCount > 0)
        {
            genericParameters = new ClassId[genericParametersCount];

            fixed (ClassId* p = genericParameters)
            {
                hr = _corProfilerInfo.GetFunctionInfo2(
                functionId,
                default,
                out _,
                out _,
                out _,
                genericParametersCount,
                out genericParametersCount,
                p);

                if (!hr.IsOK)
                {
                    return false;
                }
            }
        }
        else
        {
            genericParameters = null;
        }

        return true;
    }

    private unsafe bool GetMetadataApi(ModuleId moduleId, FunctionId functionId, out IMetaDataImport2 metaDataImport)
    {
        var hr = _corProfilerInfo.GetModuleMetaData(moduleId, CorOpenFlags.ofRead, KnownGuids.IMetaDataImport2, out var ppOut);

        if (!hr.IsOK)
        {
            hr = _corProfilerInfo.GetTokenAndMetaDataFromFunction(functionId, KnownGuids.IMetaDataImport2, out ppOut, out _);

            if (!hr.IsOK)
            {
                Console.WriteLine("Failed to get metadata API");
                metaDataImport = null;
                return false;
            }
        }

        metaDataImport = NativeObjects.IMetaDataImport2.Wrap((IntPtr)ppOut);
        return true;
    }
}