using DuckDB.NET.Data;
using DuckDb.Spatial.IO.Net.Tests.Fixture;

namespace DuckDb.Spatial.IO.Net.Tests.GeometryReaderTests;

public class StreamDbDataTest(ServiceFixture fixture, DataFixture dataFixture) : IClassFixture<ServiceFixture>, IClassFixture<DataFixture>
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

        await using var stream = await ReadGeometryStream(id);
        var geometry = _reader.Read(stream);

        Assert.NotNull(geometry);
        Assert.Equal(expected, geometry);
    }

    private async Task<Stream> ReadGeometryStream(int id)
    {
        const string query = "SELECT geom FROM test_table WHERE id=$Id";
        using var command = new DuckDBCommand(query, fixture.Connection);

        var parameter = command.CreateParameter();
        parameter.ParameterName = "Id";
        parameter.Value = id;
        command.Parameters.Add(parameter);

        var stream = (UnmanagedMemoryStream?)await command.ExecuteScalarAsync() ?? throw new InvalidOperationException("Invalid data in column");

        return stream;
    }
}
