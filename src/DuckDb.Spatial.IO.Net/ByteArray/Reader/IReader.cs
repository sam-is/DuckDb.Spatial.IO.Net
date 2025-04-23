namespace DuckDb.Spatial.IO.Net.ByteArray.Reader;

internal interface IReader
{
    public T Read<T>() where T : unmanaged;
    public void Reset();
    public void Skip(int bytes);
}
