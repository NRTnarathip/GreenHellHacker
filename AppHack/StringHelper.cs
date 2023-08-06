using System.Diagnostics;

public static class StringHelper
{
    public static string hex(this int number)
    {
        return number.ToString("X");
    }
    public static string hex(this IntPtr addr)
    {
        return addr.ToString("X");
    }
    public static string hex(this Int64 addr) => addr.ToString("X");
    public static string hex(this byte addr) => addr.ToString("X2");
    public static void log(this ProcessModule module)
    {
        var baseAddr = module.BaseAddress;
        Console.WriteLine($"[Module] name: {module.ModuleName} address: {baseAddr.hex()} memory_size: {module.ModuleMemorySize}");
    }
}