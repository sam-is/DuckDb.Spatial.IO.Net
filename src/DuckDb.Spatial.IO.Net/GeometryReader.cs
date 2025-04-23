using DuckDb.Spatial.IO.Net.ByteArray.Reader;
using NetTopologySuite.Geometries;
using StreamReader = DuckDb.Spatial.IO.Net.ByteArray.Reader.StreamReader;

namespace DuckDb.Spatial.IO.Net;

public class GeometryReader(bool isLittleEndian = true)
{
    private const byte FlagHasZ = 0x01;
    private const byte FlagHasM = 0x02;
    private const byte FlagHasBbox = 0x04;
    private const byte FlagFormatV0 = 0x80;
    private const byte FlagFormatV1 = 0x40;
    private const byte GeometryVersion = 0;
    private const uint GeometryTypeMask = 0x000000FF;

    private bool _hasZ;
    private bool _hasM;
    private byte _geometryType;

    private IReader _reader = new ByteArrayReader(isLittleEndian);

    public Geometry Read(byte[] data)
    {
        if (data.Length < 5)
            throw new ArgumentException("Invalid format");

        if (_reader is not ByteArrayReader)
            _reader = new ByteArrayReader(isLittleEndian);

        ((ByteArrayReader)_reader).Load(data);

        ParseHeader();

        var geometry = ParseGeometry(_geometryType);

        return geometry;
    }

    public Geometry Read(Stream stream)
    {
        if (stream.Length < 5)
            throw new ArgumentException("Invalid format");

        if (_reader is not StreamReader)
            _reader = new StreamReader(isLittleEndian);

        ((StreamReader)_reader).Load(stream);

        ParseHeader();

        var geometry = ParseGeometry(_geometryType);

        return geometry;
    }

    private void ParseHeader()
    {
        _geometryType = _reader.Read<byte>();
        var flags = _reader.Read<byte>();

        _hasZ = (flags & FlagHasZ) != 0;
        _hasM = (flags & FlagHasM) != 0;

        var hasBBox = (flags & FlagHasBbox) != 0;
        var formatV0 = flags & FlagFormatV0;
        var formatV1 = flags & FlagFormatV1;

        if ((formatV0 | formatV1) != GeometryVersion)
        {
            throw new NotImplementedException(
                "This geometry seems to be written with a newer version of the DuckDB spatial library that is not compatible with this version");
        }

        _reader.Skip(6);

        if (hasBBox)
        {
            var bboxSize = 8 * (2 + Convert.ToInt32(_hasZ) + Convert.ToInt32(_hasM));
            _reader.Skip(bboxSize);
        }

        _reader.Skip(4);
    }

    private Geometry ParseGeometry(byte geometryType) => geometryType switch
    {
        0 => ParsePoint(),
        1 => ParseLineString(),
        2 => ParsePolygon(),
        3 => ParseMultiPoint(),
        4 => ParseMultiLineString(),
        5 => ParseMultiPolygon(),
        6 => ParseGeometryCollection(),
        _ => throw new NotSupportedException($"Geometry type {geometryType} not supported")
    };

    private Point ParsePoint()
    {
        var count = _reader.Read<uint>();

        if (count == 0)
            return Point.Empty;

        var coordinate = ParseCoordinate();
        return new Point(coordinate);
    }

    private LineString ParseLineString()
    {
        var count = _reader.Read<uint>();

        if (count == 0)
            return LineString.Empty;

        var coordinates = new Coordinate[count];

        for (var i = 0; i < count; i++)
            coordinates[i] = ParseCoordinate();

        return new LineString(coordinates);
    }

    private Polygon ParsePolygon()
    {
        var ringCount = _reader.Read<uint>();

        if (ringCount == 0)
            return Polygon.Empty;

        var countCoordinate = ParseLinearRingsCountCoordinate(ringCount);

        var shell = ParseLinearRing(countCoordinate[0]);
        var holes = new LinearRing[ringCount - 1];

        for (var i = 0; i < holes.Length; i++)
            holes[i] = ParseLinearRing(countCoordinate[i + 1]);

        return new Polygon(shell, holes);
    }

    private uint[] ParseLinearRingsCountCoordinate(uint ringCount)
    {
        var linearRingsCountCoordinate = new uint[ringCount];

        for (var i = 0; i < ringCount; i++)
            linearRingsCountCoordinate[i] = _reader.Read<uint>();

        if (ringCount % 2 == 1)
            _reader.Skip(4);

        return linearRingsCountCoordinate;
    }

    private LinearRing ParseLinearRing(uint count)
    {
        if (count < 3)
            throw new InvalidOperationException("Points must form a closed linestring");

        var coordinates = new Coordinate[count];

        for (var i = 0; i < count; i++)
            coordinates[i] = ParseCoordinate();

        return new LinearRing(coordinates);
    }

    private MultiPoint ParseMultiPoint()
    {
        var count = _reader.Read<uint>();

        if (count == 0)
            return MultiPoint.Empty;

        var points = new Point[count];

        for (var i = 0; i < count; i++)
        {
            _reader.Skip(4);
            points[i] = ParsePoint();
        }

        return new MultiPoint(points);
    }

    private MultiLineString ParseMultiLineString()
    {
        var count = _reader.Read<uint>();

        if (count == 0)
            return MultiLineString.Empty;

        var lineStrings = new LineString[count];

        for (var i = 0; i < count; i++)
        {
            _reader.Skip(4);
            lineStrings[i] = ParseLineString();
        }

        return new MultiLineString(lineStrings);
    }

    private MultiPolygon ParseMultiPolygon()
    {
        var count = _reader.Read<uint>();

        if (count == 0)
            return MultiPolygon.Empty;

        var polygons = new Polygon[count];

        for (var i = 0; i < count; i++)
        {
            _reader.Skip(4);
            polygons[i] = ParsePolygon();
        }

        return new MultiPolygon(polygons);
    }

    private GeometryCollection ParseGeometryCollection()
    {
        var count = _reader.Read<uint>();

        if (count == 0)
            return GeometryCollection.Empty;

        var geometries = new Geometry[count];

        for (var i = 0; i < count; i++)
        {
            var geometryType = _reader.Read<uint>();
            geometries[i] = ParseGeometry((byte)(geometryType & GeometryTypeMask));
        }

        return new GeometryCollection(geometries);
    }

    private Coordinate ParseCoordinate()
    {
        var x = _reader.Read<double>();
        var y = _reader.Read<double>();

        var z = _hasZ ? _reader.Read<double>() : Coordinate.NullOrdinate;
        var m = _hasM ? _reader.Read<double>() : Coordinate.NullOrdinate;

        if (_hasZ && _hasM)
            return new CoordinateZM(x, y, z, m);

        if (_hasZ)
            return new CoordinateZ(x, y, z);

        if (_hasM)
            return new CoordinateM(x, y, m);

        return new Coordinate(x, y);
    }
}