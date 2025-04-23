namespace DuckDb.Spatial.IO.Net.Extension;

public static class ByteArrayExtensions
{
    public static T ReverseEndian<T>(T value) where T : unmanaged => typeof(T) switch
    {
        Type t when t == typeof(uint) =>
            (T)(object)BitConverter.ToUInt32(ReverseBytes(BitConverter.GetBytes((uint)(object)value))),

        Type t when t == typeof(int) =>
            (T)(object)BitConverter.ToInt32(ReverseBytes(BitConverter.GetBytes((int)(object)value))),

        Type t when t == typeof(double) =>
            (T)(object)BitConverter.ToDouble(ReverseBytes(BitConverter.GetBytes((double)(object)value))),

        Type t when t == typeof(float) =>
            (T)(object)BitConverter.ToSingle(ReverseBytes(BitConverter.GetBytes((float)(object)value))),

        _ => throw new NotSupportedException($"Type {typeof(T)} is not supported for endian reversal")
    };

    public static byte[] ReverseBytes(byte[] bytes)
    {
        Array.Reverse(bytes);
        return bytes;
    }
}