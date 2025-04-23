using DuckDb.Spatial.Tests.Fixture;

namespace DuckDb.Spatial.Tests.GeometryReaderTests;

public class RawDataTest(DataFixture dataFixture) : IClassFixture<DataFixture>
{
    private readonly GeometryReader _reader = new();

    [Theory]
    [InlineData(0, "Point.bin")]
    [InlineData(1, "LineString.bin")]
    [InlineData(2, "Polygon.bin")]
    [InlineData(3, "MultiPoint.bin")]
    [InlineData(4, "MultiLineString.bin")]
    [InlineData(5, "MultiPolygon.bin")]
    [InlineData(6, "GeometryCollection.bin")]
    [InlineData(7, "Polygon with 1 hole.bin")]
    [InlineData(8, "Polygon with 2 hole.bin")]
    public void SimpleTestData(int id, string fileName)
    {
        var filePath = Path.Combine("Data", fileName);
        var expected = dataFixture.DbGeometries[id];

        var bytes = File.ReadAllBytes(filePath);
        var geometry = _reader.Read(bytes);

        Assert.NotNull(geometry);
        Assert.Equal(expected, geometry);
    }
}