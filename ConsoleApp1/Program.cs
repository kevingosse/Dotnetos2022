
Console.WriteLine("Hello, World!");
Console.ReadLine();


ThrowException();

GC.Collect();

void ThrowException()
{
    try
    {
        throw new Exception("Hello form the profiler!");
    }
    catch { }

}

