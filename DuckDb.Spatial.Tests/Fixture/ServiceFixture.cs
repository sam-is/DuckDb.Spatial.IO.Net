using DuckDB.NET.Data;

namespace DuckDb.Spatial.Tests.Fixture;

public class ServiceFixture : IDisposable
{
    private readonly string _dbPath = Path.Combine("Data", "testdb");
    public DuckDBConnection Connection { get; }

    public GeometryWriter GeometryWriter { get; }
    public ServiceFixture()
    {
        Connection = new DuckDBConnection($"Data Source={_dbPath}");
        Connection.Open();
        InitializeDatabase();

        GeometryWriter = new GeometryWriter();
    }

    private void InitializeDatabase()
    {
        using var command = Connection.CreateCommand();
        command.CommandText = "SELECT COUNT(*) FROM duckdb_extensions() WHERE extension_name = 'spatial'";
        var count = (long)command.ExecuteScalar()!;

        if (count != 0)
            return;

        using var installCommand = Connection.CreateCommand();
        command.CommandText = "INSTALL spatial; LOAD spatial;";
        command.ExecuteNonQuery();
    }

    public void Dispose()
    {
        Connection.Close();
        Connection.Dispose();

        GeometryWriter.Dispose();
    }
}
