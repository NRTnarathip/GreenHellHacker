using System.Diagnostics;
public static partial class Memory
{
    public static string[] DumpBuffer(IntPtr baseAddr, byte[] buffer, int colume)
    {
        List<string> lines = new();
        var index = 0;
        var bufferLength = buffer.Length;
        int row = (int)Math.Ceiling(bufferLength / (float)colume);
        for (int y = 0; y < row; y++)
        {
            var addr = baseAddr + index;
            var line = "";
            line += $"address: {addr:x} | ";

            for (int x = 0; x < colume; x++)
            {
                byte b = 0;
                if (index < bufferLength)
                {
                    b = buffer[index];
                }

                var hex = $"{b}:x";
                line += hex + " ";

                //end 
                index++;
            }
            lines.Add(line);
        }
        return lines.ToArray();
    }
    public static void PrintDumpBuffer(IntPtr baseAddr, byte[] buffer, int colume)
    {
        var lines = DumpBuffer(baseAddr, buffer, colume);
        foreach (var line in lines)
            Console.WriteLine(line);
    }
    public static void print(string s) => Console.WriteLine(s);
    public static IntPtr[] AOBScan(Process proc, IntPtr startAddr, IntPtr endAddr, string[] signatureString)
    {
        Console.WriteLine($"app base addr {proc.MainModule.BaseAddress:x}");
        var foundAddresses = new List<IntPtr>();
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
        for (IntPtr addr = startAddr; addr < endAddr; addr++)
        {
            var found = true;
            for (int sigIndex = 0; sigIndex < signatureString.Length; sigIndex++)
            {
                IntPtr currentAddress = addr + sigIndex;
                if (currentAddress >= endAddr)
                {
                    found = false;
                    break;
                }
                //var b = buffer[currentIndex];
                var b = readByte(proc.Handle, currentAddress);
                var sigInt = signatureNumer[sigIndex];
                if (currentAddress % 0xfffff == 0)
                {
                    print($"current addr {currentAddress:x}");
                }
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
            var foundAddr = startAddr + addr;
            foundAddresses.Add(foundAddr);
        }
        st.Stop();
        if (foundAddresses.Count > 0)
        {
            Console.WriteLine($"[AOB Scan] Done Time: {st.ElapsedMilliseconds * 0.001f}s, found address list: {foundAddresses.Count}");
        }
        return foundAddresses.ToArray();
    }
    public static IntPtr readPointerOffset(Process proc, IntPtr startAddr, IntPtr[] offsets)
    {
        IntPtr currentPtr = startAddr;
        foreach (var offset in offsets)
        {
            var addrTarget = currentPtr + offset;
            var ptr = readIntPtr(proc, addrTarget);
            Console.WriteLine($"Read address: {addrTarget:X} || value: {ptr:X}");
            if (ptr == IntPtr.Zero)
            {
                Console.WriteLine($"Error not fonud ptr {currentPtr:X}");
                return ptr;
            }
            currentPtr = ptr;
        }
        return currentPtr;
    }
}