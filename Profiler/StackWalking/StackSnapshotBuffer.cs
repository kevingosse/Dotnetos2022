using System.Text;

namespace Profiler.StackWalking;

internal unsafe struct StackSnapshotBuffer
{
    public const int Size = 1024;

    public int Count { get; private set; }

    public fixed long InstructionPointers[1024];

    public bool Add(nint ip)
    {
        if (Count >= Size)
        {
            return false;
        }

        InstructionPointers[Count++] = ip;

        return true;
    }

    public override string ToString()
    {
        var sb = new StringBuilder();

        sb.AppendLine($"{Count} frames:");

        for (int i = 0; i < Count; i++)
        {
            sb.AppendLine($" - {InstructionPointers[i]:x2}");
        }

        return sb.ToString();
    }
}