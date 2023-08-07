using Kernal32;

public static partial class Memory
{
    public static bool writeFloat(IntPtr m_handle, IntPtr address, float value)
    {
        var bytes = BitConverter.GetBytes(value);
        return Kernal.WriteProcessMemory(m_handle, address, bytes, bytes.Length, out _);
    }
}