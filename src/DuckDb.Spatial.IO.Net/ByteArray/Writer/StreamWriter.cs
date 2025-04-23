using System.Buffers;
using System.Runtime.InteropServices;
using DuckDb.Spatial.IO.Net.Extension;

namespace DuckDb.Spatial.IO.Net.ByteArray.Writer;

internal class StreamWriter : IWriter
{
    private Stream? _stream;
    private readonly bool _isLittleEndian;
    private readonly byte[] _buffer;
    private readonly ArrayPool<byte> _pool;

    private int _offset;

    public StreamWriter(bool isLittleEndian = true, int bufferSize = 4096, ArrayPool<byte>? pool = null)
    {
        _isLittleEndian = isLittleEndian;
        _pool = pool ?? ArrayPool<byte>.Shared;
        _buffer = _pool.Rent(bufferSize);
    }

    public void Write<T>(T value) where T : unmanaged
    {
        var size = Marshal.SizeOf<T>();
        EnsureBuffer(size);

        if (BitConverter.IsLittleEndian != _isLittleEndian)
            value = ByteArrayExtensions.ReverseEndian(value);

        MemoryMarshal.Write(_buffer.AsSpan(_offset, size), in value);
        _offset += size;
    }

    public void WriteBytes(byte[] data)
    {
        EnsureBuffer(data.Length);

        if (BitConverter.IsLittleEndian != _isLittleEndian)
            data = ByteArrayExtensions.ReverseBytes(data);

        Buffer.BlockCopy(data, 0, _buffer, _offset, data.Length);
        _offset += data.Length;
    }

    public void FlushBuffer()
    {
        _stream?.Write(_buffer, 0, _offset);
        _offset = 0;
    }

    public void Load(Stream stream)
    {
        _offset = 0;
        _stream = stream;
    }

    public void Reset()
    {
        _offset = 0;
        _stream = null;
    }

    public void Dispose()
    {
        _pool.Return(_buffer);
    }

    private void EnsureBuffer(int additionalBytes)
    {
        if (_offset + additionalBytes > _buffer.Length)
        {
            FlushBuffer();
        }
    }
}
