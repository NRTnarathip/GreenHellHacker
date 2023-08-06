using Kernal32;
using System.Diagnostics;

public static partial class Memory
{
    static byte[] bufferByte = new byte[1] { 0 };
    public static byte readByte(IntPtr procHandle, IntPtr dwAddress)
    {
        //byte[] buffer = new byte[1];
        Kernal.ReadProcessMemory(procHandle, dwAddress, bufferByte, sizeof(byte), out _);
        return bufferByte[0];
    }
    public static Int32 readInt32(IntPtr m_handle, IntPtr dwAddress)
    {
        byte[] buffer = new byte[sizeof(Int32)];
        Kernal.ReadProcessMemory(m_handle, dwAddress, buffer, buffer.Length, out _);
        return BitConverter.ToInt32(buffer, 0);
    }
    public static Int64 readInt64(IntPtr m_handle, IntPtr dwAddress)
    {
        byte[] buffer = new byte[sizeof(Int64)];
        Kernal.ReadProcessMemory(m_handle, dwAddress, buffer, sizeof(Int64), out _);
        return BitConverter.ToInt64(buffer, 0);
    }
    public static IntPtr readIntPtr(Process proc, IntPtr address)
    {
        var procHandle = proc.Handle;
        if (IntPtr.Size == sizeof(Int64))
            return (IntPtr)readInt64(procHandle, address);

        return readInt32(procHandle, address);

    }
    public static float readFloat(IntPtr m_handle, IntPtr dwAddress)
    {
        byte[] buffer = new byte[sizeof(float)];
        Kernal.ReadProcessMemory(m_handle, dwAddress, buffer, sizeof(float), out _);
        return BitConverter.ToSingle(buffer, 0);
    }
}
