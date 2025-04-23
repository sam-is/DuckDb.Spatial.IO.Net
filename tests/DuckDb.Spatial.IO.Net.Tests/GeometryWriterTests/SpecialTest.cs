using DuckDb.Spatial.IO.Net.Tests.Fixture;
using NetTopologySuite.Geometries;

namespace DuckDb.Spatial.IO.Net.Tests.GeometryWriterTests;

public class SpecialTest(ServiceFixture fixture, DataFixture dataFixture) : IClassFixture<ServiceFixture>, IClassFixture<DataFixture>
{
    private readonly GeometryWriter _writer = fixture.GeometryWriter;
    private readonly GeometryReader _reader = new();

    [Fact]
    public void LargeLineStringTest()
    {
        var coordinates = new Coordinate[1000];
        for (int i = 0; i < coordinates.Length; i++)
        {
            coordinates[i] = new Coordinate(i, i);
        }

        var geometry = new LineString(coordinates);
        var bytes = _writer.Write(geometry);
        var result = _reader.Read(bytes);

        Assert.NotNull(result);
        Assert.Equal(geometry, result);
    }

    [Fact]
    public void ExtremeValuesTest()
    {
        var geometry = dataFixture.SpecialGeometries[0];
        var bytes = _writer.Write(geometry);
        var result = _reader.Read(bytes) as Point;

        Assert.NotNull(result);
        Assert.Equal(double.MaxValue, result.X);
        Assert.Equal(double.MinValue, result.Y);
        Assert.Equal(double.NaN, result.Z);
        Assert.Equal(double.PositiveInfinity, result.M);
    }

    [Fact]
    public void BBoxTest()
    {
        var geometry = dataFixture.SpecialGeometries[1];

        var bytes = _writer.Write(geometry);
        var result = _reader.Read(bytes);

        var envelope = geometry.EnvelopeInternal;
        var resultEnvelope = result.EnvelopeInternal;

        Assert.Equal(envelope.MinX, resultEnvelope.MinX);
        Assert.Equal(envelope.MaxX, resultEnvelope.MaxX);
        Assert.Equal(envelope.MinY, resultEnvelope.MinY);
        Assert.Equal(envelope.MaxY, resultEnvelope.MaxY);
    }
}
