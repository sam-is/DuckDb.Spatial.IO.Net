using DuckDb.Spatial.Extension;
using System.Runtime.InteropServices;

namespace DuckDb.Spatial.ByteArray.Reader;

class StreamReader(bool isLittleEndian) : IReader
{
    private Stream? _stream;

    private byte[] _buffer = new byte[64];

    public T Read<T>() where T : unmanaged
    {
        if (_stream is null)
            throw new InvalidOperationException("Not load stream");

        var size = Marshal.SizeOf<T>();

        if (size > _buffer.Length)
            Array.Resize(ref _buffer, size);

        _stream.ReadExactly(_buffer, 0, size);

        T value = MemoryMarshal.Read<T>(_buffer);

        if (BitConverter.IsLittleEndian != isLittleEndian)
            value = ByteArrayExtensions.ReverseEndian(value);

        return value;
    }

    public void Load(Stream? stream)
    {
        _stream = stream;
    }

    public void Reset() => Load(null);

    public void Skip(int bytes)
    {
        if (_stream is null)
            return;

        if (_stream.CanSeek)
        {
            _stream.Position += bytes;
        }
        else
        {
            _stream.ReadExactly(_buffer, 0, bytes);
        }
    }
}
