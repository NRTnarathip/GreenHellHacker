using Kernal32;
using System.Diagnostics;
public sealed class ProcessHack
{
    public Process proc;
    public IntPtr procHandle;
    public ProcessHack(string procName)
    {
        var processes = Process.GetProcessesByName(procName);
        if (processes.Length == 0) return;
        proc = processes[0];
    }
    public bool Setup()
    {
        if (proc == null)
        {
            Console.WriteLine("Error not fonud process game");
            return false;
        }
        Console.WriteLine($"process ");
        Console.WriteLine($"process name: {proc.Id}");

        procHandle = Kernal.OpenProcess(proc, ProcessAccessFlags.VirtualMemoryRead |
           ProcessAccessFlags.VirtualMemoryWrite
            | ProcessAccessFlags.VirtualMemoryOperation);
        Console.WriteLine($"Start {DateTime.Now}");
        Console.WriteLine($"Try Create new Process Hack: {proc.ProcessName}");
        Console.WriteLine($"process handle: {procHandle}");


        //scan AOB

        return true;
    }
    public Int32 readInt32(IntPtr addr) => Memory.readInt32(procHandle, addr);
    public Int64 readInt64(IntPtr addr) => Memory.readInt64(procHandle, addr);
    public float readFloat(IntPtr addr) => Memory.readFloat(procHandle, addr);
    public bool writeFloat(IntPtr addr, float value) => Memory.writeFloat(procHandle, addr, value);
    public ProcessModule getModule(string name)
    {
        foreach (ProcessModule module in proc.Modules)
        {
            if (module.ModuleName == name) return module;
        }
        Console.WriteLine("Not found module " + name);
        return null;
    }
    //stop 0x00007fffffffffff
    public IntPtr[] aobScan(string[] signatureString, ProcessModule module)
    {
        var foundAddresses = new List<IntPtr>();
        var baseAddr = module.BaseAddress;
        byte[] buffer = new byte[module.ModuleMemorySize];
        Kernal.ReadProcessMemory(procHandle, baseAddr, buffer, buffer.Length, out _);
        var bufferLength = buffer.Length;
        var signatureNumer = new int[signatureString.Length];
        for (int i = 0; i < signatureString.Length; i++)
        {
            var sig = signatureString[i];
            if (sig == "??")
            {
                signatureNumer[i] = -1;
                continue;
            }
            signatureNumer[i] = Convert.ToInt32(sig, 16);
        }
        var st = Stopwatch.StartNew();

        for (int bufferIndex = 0; bufferIndex < bufferLength; bufferIndex++)
        {
            var found = true;
            for (int sigIndex = 0; sigIndex < signatureString.Length; sigIndex++)
            {
                var currentIndex = bufferIndex + sigIndex;
                if (currentIndex >= bufferLength)
                {
                    found = false;
                    break;
                }
                var b = buffer[currentIndex];
                var sigInt = signatureNumer[sigIndex];
                //found byte
                if (sigInt == -1 || sigInt == b)
                {
                    continue;
                }

                //not found
                found = false;
                break;
            }
            if (!found) continue;

            //found
            var foundAddr = baseAddr + bufferIndex;
            foundAddresses.Add(foundAddr);
        }
        st.Stop();
        if (foundAddresses.Count > 0)
        {
            Console.WriteLine($"[AOB Scan] Done Time: {st.ElapsedMilliseconds * 0.001f}s, found address list: {foundAddresses.Count}");
            Console.WriteLine($"[AOB Scan] Found!! with in module {module.ModuleName}");
        }
        return foundAddresses.ToArray();
    }
    public void readBuffer(IntPtr addr, byte[] buffer)
    {
        Kernal.ReadProcessMemory(proc.Handle, addr, buffer, buffer.Length, out _);
    }
}
