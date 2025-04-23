using System.Buffers;
using DuckDb.Spatial.IO.Net.ByteArray.Writer;
using DuckDb.Spatial.IO.Net.Extension;
using NetTopologySuite.Geometries;
using StreamWriter = DuckDb.Spatial.IO.Net.ByteArray.Writer.StreamWriter;

namespace DuckDb.Spatial.IO.Net;

public sealed class GeometryWriter(bool isLittleEndian = true, int initialCapacity = 4096, ArrayPool<byte>? pool = null) : IDisposable
{
    private const byte FlagHasZ = 0x01;
    private const byte FlagHasM = 0x02;
    private const byte FlagHasBbox = 0x04;
    private const byte GeometryVersion = 0x00;

    private IWriter _writer = new ByteArrayWriter(isLittleEndian, initialCapacity, pool);

    private static readonly byte[] ZeroPadding = new byte[4];

    private bool _hasZ;
    private bool _hasM;

    public byte[] Write(Geometry geometry)
    {
        if (_writer is not ByteArrayWriter)
        {
            _writer.Dispose();
            _writer = new ByteArrayWriter(isLittleEndian, initialCapacity, pool);
        }

        ((ByteArrayWriter)_writer).Reset();

        var flags = GetFlags(geometry);
        WriteHeader(geometry, flags);
        WriteGeometry(geometry);

        return ((ByteArrayWriter)_writer).GetByteArray();
    }

    public void Write(Geometry geometry, Stream stream)
    {
        if (_writer is not StreamWriter)
        {
            _writer.Dispose();
            _writer = new StreamWriter(isLittleEndian, initialCapacity, pool);
        }

        ((StreamWriter)_writer).Load(stream);

        var flags = GetFlags(geometry);
        WriteHeader(geometry, flags);
        WriteGeometry(geometry);

        ((StreamWriter)_writer).FlushBuffer();
    }

    private void WriteHeader(Geometry geometry, byte flags)
    {
        var geometryType = GetGeometryType(geometry);

        _writer.Write((byte)geometryType);
        _writer.Write(flags);
        _writer.WriteBytes(new byte[6]);

        if (HasBBox(flags))
            WriteBBox(geometry);
    }

    private void WriteGeometry(Geometry geometry)
    {
        switch (geometry)
        {
            case Point point:
                WritePoint(point);
                break;
            case LineString lineString:
                WriteLineString(lineString);
                break;
            case Polygon polygon:
                WritePolygon(polygon);
                break;
            case MultiPoint multiPoint:
                WriteMultiPoint(multiPoint);
                break;
            case MultiLineString multiLine:
                WriteMultiLineString(multiLine);
                break;
            case MultiPolygon multiPolygon:
                WriteMultiPolygon(multiPolygon);
                break;
            case GeometryCollection collection:
                WriteGeometryCollection(collection);
                break;
            default:
                throw new NotSupportedException($"Geometry type {geometry.GetType()} not supported");
        }
    }

    private static uint GetGeometryType(Geometry geometry) => geometry switch
    {
        Point => 0,
        LineString => 1,
        Polygon => 2,
        MultiPoint => 3,
        MultiLineString => 4,
        MultiPolygon => 5,
        GeometryCollection => 6,
        _ => throw new NotSupportedException($"Unsupported geometry type: {geometry.GetType()}")
    };

    private byte GetFlags(Geometry geometry)
    {
        byte flags = 0;
        _hasZ = HasZ(geometry);
        _hasM = HasM(geometry);

        if (_hasZ) flags |= FlagHasZ;
        if (_hasM) flags |= FlagHasM;

        if (!geometry.IsEmpty && geometry.OgcGeometryType != OgcGeometryType.Point)
            flags |= FlagHasBbox;

        flags |= GeometryVersion;
        return flags;
    }

    private static bool HasZ(Geometry geometry) =>
        !geometry.IsEmpty && geometry.Coordinate?.Z is not Coordinate.NullOrdinate and not double.NaN;

    private static bool HasM(Geometry geometry) =>
        !geometry.IsEmpty && geometry.Coordinate?.M is not Coordinate.NullOrdinate and not double.NaN;

    private static bool HasBBox(byte flags) => (flags & FlagHasBbox) != 0;

    private void WriteBBox(Geometry geometry)
    {
        var envelope = geometry.EnvelopeInternal;

        _writer.Write(FloatConverter.DoubleToFloatDown(envelope.MinX));
        _writer.Write(FloatConverter.DoubleToFloatDown(envelope.MinY));
        _writer.Write(FloatConverter.DoubleToFloatUp(envelope.MaxX));
        _writer.Write(FloatConverter.DoubleToFloatUp(envelope.MaxY));

        if (HasZ(geometry))
        {
            var (minZ, maxZ) = GetZExtent(geometry);
            _writer.Write(FloatConverter.DoubleToFloatDown(minZ));
            _writer.Write(FloatConverter.DoubleToFloatUp(maxZ));
        }

        if (HasM(geometry))
        {
            var (minM, maxM) = GetMExtent(geometry);
            _writer.Write(FloatConverter.DoubleToFloatDown(minM));
            _writer.Write(FloatConverter.DoubleToFloatUp(maxM));
        }
    }

    private static (double min, double max) GetZExtent(Geometry geometry)
    {
        var min = double.MaxValue;
        var max = double.MinValue;

        foreach (var coordinate in geometry.Coordinates)
        {
            if (double.IsNaN(coordinate.Z) || coordinate.Z is Coordinate.NullOrdinate) continue;
            min = Math.Min(min, coordinate.Z);
            max = Math.Max(max, coordinate.Z);
        }

        return (min, max);
    }

    private static (double min, double max) GetMExtent(Geometry geometry)
    {
        var min = double.MaxValue;
        var max = double.MinValue;

        foreach (var coordinate in geometry.Coordinates)
        {
            if (double.IsNaN(coordinate.M) || coordinate.M is Coordinate.NullOrdinate) continue;
            min = Math.Min(min, coordinate.M);
            max = Math.Max(max, coordinate.M);
        }

        return (min, max);
    }

    private void WritePoint(Point point)
    {
        _writer.Write(GetGeometryType(point));
        _writer.Write(point.IsEmpty ? 0 : 1);
        WriteCoordinate(point.Coordinate);
    }

    private void WriteLineString(LineString line)
    {
        _writer.Write(GetGeometryType(line));
        _writer.Write((uint)line.Coordinates.Length);

        foreach (var coordinate in line.Coordinates)
            WriteCoordinate(coordinate);
    }

    private void WritePolygon(Polygon polygon)
    {
        _writer.Write(GetGeometryType(polygon));

        List<LinearRing> rings = [.. new[] { polygon.Shell }.Concat(polygon.Holes).Where(ring => !ring.IsEmpty)];

        _writer.Write((uint)rings.Count);

        foreach (var ring in rings)
        {
            _writer.Write((uint)ring.Coordinates.Length);
        }

        if (rings.Count % 2 == 1)
            _writer.WriteBytes(ZeroPadding);

        foreach (var ring in rings)
        {
            foreach (var coordinate in ring.Coordinates)
            {
                WriteCoordinate(coordinate);
            }
        }
    }

    private void WriteMultiPoint(MultiPoint multiPoint)
    {
        _writer.Write(GetGeometryType(multiPoint));
        _writer.Write((uint)multiPoint.NumGeometries);

        foreach (var geom in multiPoint.Geometries)
        {
            WritePoint((Point)geom);
        }
    }

    private void WriteMultiLineString(MultiLineString multiLine)
    {
        _writer.Write(GetGeometryType(multiLine));
        _writer.Write((uint)multiLine.NumGeometries);

        foreach (var geom in multiLine.Geometries)
        {
            WriteLineString((LineString)geom);
        }
    }

    private void WriteMultiPolygon(MultiPolygon multiPolygon)
    {
        _writer.Write(GetGeometryType(multiPolygon));
        _writer.Write((uint)multiPolygon.NumGeometries);

        foreach (var geom in multiPolygon.Geometries)
        {
            WritePolygon((Polygon)geom);
        }
    }

    private void WriteGeometryCollection(GeometryCollection collection)
    {
        _writer.Write(GetGeometryType(collection));
        _writer.Write((uint)collection.NumGeometries);

        foreach (var geom in collection.Geometries)
        {
            WriteGeometry(geom);
        }
    }

    private void WriteCoordinate(Coordinate? coordinate)
    {
        if (coordinate is null)
            return;

        _writer.Write(coordinate.X);
        _writer.Write(coordinate.Y);

        if (_hasZ)
            _writer.Write(coordinate.Z);

        if (_hasM)
            _writer.Write(coordinate.M);
    }

    public void Dispose()
    {
        _writer.Dispose();
    }
}
