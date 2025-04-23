namespace DuckDb.Spatial.ByteArray.Reader;

interface IReader
{
    public T Read<T>() where T : unmanaged;
    public void Reset();
    public void Skip(int bytes);
}
