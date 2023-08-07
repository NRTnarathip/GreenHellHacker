using System.Diagnostics;
public static partial class ProcessHelper
{
    public static IntPtr readIntPtr(Process proc, IntPtr addr) => Memory.readIntPtr(proc, addr);
}
