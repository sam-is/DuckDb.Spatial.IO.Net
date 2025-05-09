﻿namespace DuckDb.Spatial.IO.Net.ByteArray.Writer;

internal interface IWriter : IDisposable
{
    public void Write<T>(T value) where T : unmanaged;
    public void WriteBytes(byte[] data);
    public void Reset();
}
