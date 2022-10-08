using System.Runtime.InteropServices;
using System.Text;

namespace LldbExtension
{
    internal class MethodLocator
    {
        private const int PT_DYNAMIC = 2;
        private const int DT_STRTAB = 5;
        private const int DT_SYMTAB = 6;
        private const int DT_SYMENT = 11;

        private static readonly List<(string module, Dictionary<string, nint>)> Methods = new();

        static unsafe MethodLocator()
        {
            dl_iterate_phdr(&dl_iterate_callback, IntPtr.Zero);
        }

        public static nint Find(string module, string method)
        {
            var item = Methods.Single(i => i.module.Contains(module));

            method = Mangle(method);

            try
            {
                return item.Item2.Single(m => m.Key.Contains(method)).Value;
            }
            catch (InvalidOperationException)
            {
                Console.WriteLine($"Available methods in {item.module}:");

                foreach (var m in item.Item2)
                {
                    Console.WriteLine(m.Key);
                }

                throw;
            }
        }

        private static string Mangle(string name)
        {
            var parts = name.Split('.');

            var result = new StringBuilder();

            foreach (var part in parts)
            {
                result.Append(part.Length);
                result.Append(part);
            }

            return result.ToString();
        }

        [UnmanagedCallersOnly]
        private static unsafe int dl_iterate_callback(dl_phdr_info* info, nint s, IntPtr data)
        {
            var name = Marshal.PtrToStringAnsi((IntPtr)info->dlpi_name);

            var methods = new Dictionary<string, nint>();

            for (int j = 0; j < info->dlpi_phnum; j++)
            {
                if (info->dlpi_phdr[j].p_type == PT_DYNAMIC)
                {
                    Elf64_Sym* symtab = null;
                    byte* strtab = null;
                    nint symentries = 0;
                    var dyn = (Elf64_Dyn*)(info->dlpi_addr + info->dlpi_phdr[j].p_vaddr);

                    for (ulong k = 0; k < info->dlpi_phdr[j].p_memsz / (ulong)sizeof(Elf64_Dyn); ++k)
                    {
                        if (dyn[k].d_tag == DT_SYMTAB)
                        {
                            symtab = (Elf64_Sym*)dyn[k].d_ptr;
                        }
                        if (dyn[k].d_tag == DT_STRTAB)
                        {
                            strtab = (byte*)dyn[k].d_ptr;
                        }
                        if (dyn[k].d_tag == DT_SYMENT)
                        {
                            symentries = dyn[k].d_ptr;
                        }
                    }

                    var size = strtab - (byte*)symtab;

                    for (int k = 0; k < size / symentries; ++k)
                    {
                        try
                        {
                            var sym = symtab[k];

                            var str = Marshal.PtrToStringAnsi((IntPtr)(strtab + sym.st_name));

                            if (string.IsNullOrEmpty(str))
                            {
                                continue;
                            }

                            methods[str] = sym.st_value + info->dlpi_addr;
                        }
                        catch (NullReferenceException)
                        {
                            continue;
                        }
                    }
                }
            }

            Methods.Add((name, methods));

            return 0;
        }

        [DllImport("libdl.so")]
        private static extern unsafe void dl_iterate_phdr(delegate* unmanaged<dl_phdr_info*, nint, IntPtr, int> callback, IntPtr data);

        private unsafe struct Elf64_Sym
        {
            public uint st_name;
            public byte st_info;
            public byte st_other;
            public ushort st_shndx;
            public nint st_value;
            public ulong st_size;
        }

        private unsafe struct Elf64_Phdr
        {
            public uint p_type;
            public uint p_flags;
            public ulong p_offset;
            public nint p_vaddr;
            public nint p_paddr;
            public ulong p_filesz;
            public ulong p_memsz;
            public ulong p_align;
        }

        private unsafe struct Elf64_Dyn
        {
            public ulong d_tag;
            public nint d_ptr;
        }

        private unsafe struct dl_phdr_info
        {
            public nint dlpi_addr;
            public char* dlpi_name;
            public Elf64_Phdr* dlpi_phdr;
            public short dlpi_phnum;
            public ulong dlpi_adds;
            public ulong dlpi_subs;
            public nint dlpi_tls_modid;
            public IntPtr dlpi_tls_data;
        }

    }
}
