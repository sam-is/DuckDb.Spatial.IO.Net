using DuckDb.Spatial.Tests.Fixture;
using DuckDB.NET.Data;

namespace DuckDb.Spatial.Tests.GeometryReaderTests;

public class ByteArrayDbDataTest(ServiceFixture fixture, DataFixture dataFixture) : IClassFixture<ServiceFixture>, IClassFixture<DataFixture>
{
    private readonly GeometryReader _reader = new();

    [Theory]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(3)]
    [InlineData(4)]
    [InlineData(5)]
    [InlineData(6)]
    [InlineData(7)]
    [InlineData(8)]
    [InlineData(9)]

    public async Task SimpleDataTest(int id)
    {
        var expected = dataFixture.DbGeometries[id - 1];

        var bytes = await ReadGeometry(id);
        var geometry = _reader.Read(bytes);

        Assert.NotNull(geometry);
        Assert.Equal(expected, geometry);
    }

    private async Task<byte[]> ReadGeometry(int id)
    {
        const string query = "SELECT geom FROM test_table WHERE id=$Id";
        using var command = new DuckDBCommand(query, fixture.Connection);

        var parameter = command.CreateParameter();
        parameter.ParameterName = "Id";
        parameter.Value = id;
        command.Parameters.Add(parameter);

        await using var stream = (UnmanagedMemoryStream?)await command.ExecuteScalarAsync() ?? throw new InvalidOperationException("Invalid data in column");

        using var memoryStream = new MemoryStream();
        await stream.CopyToAsync(memoryStream);

        return memoryStream.ToArray();
    }
}
