using PeNet.Header.Pe;
using System.Runtime.InteropServices;

public static class Program
{

    delegate IntPtr IntPtrFuncDeletgate();
    const string MonoDllName = "mono-2.0-bdwgc.dll";
    //[DllImport(MonoDllName, CallingConvention = CallingConvention.Cdecl)]
    //public static extern IntPtr mono_get_root_domain();

    [DllImport(MonoDllName, CallingConvention = CallingConvention.Cdecl)]
    public static extern IntPtr mono_thread_attach(IntPtr domain);

    static void Main(string[] args)
    {
        //var hack = new GreenHellHack();
        //hack.LaunchApp();
        var game = new ProcessHack(GreenHellHack.GameProcessName);
        if (!game.Setup()) return;
        var monoModule = game.getModule("mono-2.0-bdwgc.dll");
        //not work i dont know why
        //var rawMonoDll = new byte[monoModule.ModuleMemorySize];
        //game.readBuffer(monoModule.BaseAddress, rawMonoDll);
        //var peMono = new PeNet.PeFile(rawMonoDll);
        var peMono = new PeNet.PeFile(@"C:\Program Files (x86)\Steam\steamapps\common\Green Hell\MonoBleedingEdge\EmbedRuntime\mono-2.0-bdwgc.dll");
        var mapFuncs = new Dictionary<string, ExportFunction>();
        var mapFuncAddress = new Dictionary<string, IntPtr>();
        foreach (var func in peMono.ExportedFunctions)
        {
            mapFuncs[func.Name] = func;
            mapFuncAddress[func.Name] = monoModule.BaseAddress + (IntPtr)func.Address;
            Console.WriteLine($"Name:{func.Name} offs: {func.Address:x}");
        }
        //var kernalModule = game.getModule("KERNEL32.DLL");
        //byte[] kernalModuleBuffer = new byte[kernalModule.ModuleMemorySize];
        //game.readBuffer(kernalModule.BaseAddress, kernalModuleBuffer);
        //var kernalPE = new PeNet.PeFile(kernalModuleBuffer);
        //IntPtr writeConsoleVTAddr = 0;
        //foreach (var func in kernalPE.ExportedFunctions)
        //{
        //    var vtAddr = kernalModule.BaseAddress + (IntPtr)func.Address;
        //    //Console.WriteLine($"func name:{func.Name} addr:{vtAddr:x}");
        //    if (func.Name == "WriteConsoleA")
        //    {
        //        writeConsoleVTAddr = vtAddr;
        //    }
        //}

        IntPtr rootDomain = 0;
        IntPtrFuncDeletgate mono_get_root_domain = Marshal.GetDelegateForFunctionPointer<IntPtrFuncDeletgate>(mapFuncAddress["mono_get_root_domain"]);
        rootDomain = mono_get_root_domain();

        //rootDomain = funcDel();
        //mono_thread_attach(rootDomain);
        Console.WriteLine($"root domain vt addresss: {rootDomain:x}");
    }
}
