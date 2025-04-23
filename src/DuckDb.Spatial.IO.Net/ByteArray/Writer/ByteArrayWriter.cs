using System.Buffers;
using System.Runtime.InteropServices;
using DuckDb.Spatial.IO.Net.Extension;

namespace DuckDb.Spatial.IO.Net.ByteArray.Writer;

internal class ByteArrayWriter : IWriter
{
    private readonly bool _isLittleEndian;
    private byte[] _buffer;
    private readonly int _initialCapacity;
    private readonly ArrayPool<byte> _pool;

    private int Offset { get; set; }

    public ByteArrayWriter(bool isLittleEndian, int initialCapacity, ArrayPool<byte>? pool = null)
    {
        _isLittleEndian = isLittleEndian;
        _initialCapacity = initialCapacity;

        _pool = pool ?? ArrayPool<byte>.Shared;
        _buffer = _pool.Rent(initialCapacity);

        Offset = 0;
    }

    public void Write<T>(T value) where T : unmanaged
    {
        var size = Marshal.SizeOf<T>();

        EnsureCapacity(size);

        if (BitConverter.IsLittleEndian != _isLittleEndian)
            value = ByteArrayExtensions.ReverseEndian(value);

        MemoryMarshal.Write(new Span<byte>(_buffer, Offset, size), in value);
        Offset += size;
    }

    public void WriteBytes(byte[] data)
    {
        EnsureCapacity(data.Length);

        if (BitConverter.IsLittleEndian != _isLittleEndian)
            data = ByteArrayExtensions.ReverseBytes(data);

        Buffer.BlockCopy(data, 0, _buffer, Offset, data.Length);
        Offset += data.Length;
    }

    public void Reset() => Offset = 0;

    public byte[] GetByteArray()
    {
        var result = new byte[Offset];
        Buffer.BlockCopy(_buffer, 0, result, 0, Offset);

        Reset();

        return result;
    }

    public void Dispose()
    {
        _pool.Return(_buffer);
    }

    private void EnsureCapacity(int additionalBytes)
    {
        if (Offset + additionalBytes <= _buffer.Length) return;

        var newCapacity = Math.Max(_buffer.Length * 2, Offset + additionalBytes);
        newCapacity = Math.Max(newCapacity, _initialCapacity);

        var newBuffer = _pool.Rent(newCapacity);
        Buffer.BlockCopy(_buffer, 0, newBuffer, 0, Offset);
        _pool.Return(_buffer);
        _buffer = newBuffer;
    }
}
