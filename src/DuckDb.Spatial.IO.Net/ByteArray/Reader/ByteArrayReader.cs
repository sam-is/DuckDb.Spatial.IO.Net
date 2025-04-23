using System.Runtime.InteropServices;
using DuckDb.Spatial.IO.Net.Extension;

namespace DuckDb.Spatial.IO.Net.ByteArray.Reader;

public class ByteArrayReader(bool isLittleEndian) : IReader
{
    private int _offset = 0;
    private byte[] _data = [];

    public T Read<T>() where T : unmanaged
    {
        var size = Marshal.SizeOf<T>();

        if (_offset + size > _data.Length)
            throw new ArgumentOutOfRangeException(nameof(_offset));

        var value = MemoryMarshal.Read<T>(new ReadOnlySpan<byte>(_data, _offset, size));
        _offset += size;

        return BitConverter.IsLittleEndian != isLittleEndian ? ByteArrayExtensions.ReverseEndian(value) : value;
    }

    public void Load(byte[] newData)
    {
        _data = newData;
        _offset = 0;
    }

    public void Reset() => Load([]);

    public void Skip(int bytes) => _offset += bytes;
}
