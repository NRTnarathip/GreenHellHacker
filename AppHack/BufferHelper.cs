using Kernal32;
using System.Diagnostics;
using System.Text;

public class Buffer
{
    byte[] buffer;
    IntPtr bytesRead;
    public Buffer(Process proc, ProcessModule module)
    {
        var newBuffer = new byte[module.ModuleMemorySize];
        Kernal.ReadProcessMemory(proc.Handle,
            module.BaseAddress,
            newBuffer, newBuffer.Length,
            out bytesRead);
        this.buffer = newBuffer;
    }
    public Int32 readInt32(Int32 startIndex)
    {
        var buff = new byte[4]
        {
            buffer[startIndex],
            buffer[startIndex+1],
            buffer[startIndex+2],
            buffer[startIndex+3]
        };
        return BitConverter.ToInt32(buff);
    }
    public Int64 readInt64(Int64 index)
    {
        if (index + 8 >= bytesRead)
        {
            Console.WriteLine($"error byte out {index} {bytesRead}");
            return 0;
        }

        var buff = new byte[8]
        {
            buffer[index],
            buffer[index+1],
            buffer[index+2],
            buffer[index+3],
            buffer[index+4],
            buffer[index+5],
            buffer[index+6],
            buffer[index+7],
        };
        return BitConverter.ToInt64(buff);
    }
    public IntPtr readIntPtr(IntPtr startIndex)
    {
        if (IntPtr.Size == sizeof(Int64))
            return (IntPtr)readInt64(startIndex);
        return readInt32((Int32)startIndex);
    }
    public string readAsciiString(int start = 0)
    {
        if (start < 0) return "";

        var length = buffer.Skip(start).TakeWhile(b => b != 0).Count();
        //Console.WriteLine($"start {start} length {length} byteyread {bytesRead} buff length {buffer.Length}");
        if (start + length >= buffer.Length) return "";

        return Encoding.ASCII.GetString(buffer, start, length);
    }
}