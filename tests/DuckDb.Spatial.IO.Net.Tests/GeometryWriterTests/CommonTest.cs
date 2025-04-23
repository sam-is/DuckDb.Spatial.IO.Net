using DuckDb.Spatial.IO.Net.Tests.Fixture;

namespace DuckDb.Spatial.IO.Net.Tests.GeometryWriterTests;

public class CommonTest(ServiceFixture fixture, DataFixture dataFixture) : IClassFixture<ServiceFixture>, IClassFixture<DataFixture>
{
    private readonly GeometryWriter _writer = fixture.GeometryWriter;
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
    [InlineData(10)]
    [InlineData(11)]
    [InlineData(12)]

    public void SimpleDataTest(int index)
    {
        var geometry = dataFixture.Geometries[index];

        var bytes = _writer.Write(geometry);
        var result = _reader.Read(bytes);

        Assert.Equal(geometry, result);
    }
}