using NetTopologySuite.Geometries;

namespace DuckDb.Spatial.IO.Net.Tests.Fixture;

public class DataFixture
{
    public List<Geometry> DbGeometries { get; } =
    [
        new Point(30, 10),
        new LineString([new Coordinate(30, 10), new Coordinate(10, 30), new Coordinate(40, 40)]),
        new Polygon(new LinearRing([new Coordinate(30, 10), new Coordinate(40, 40), new Coordinate(20, 40), new Coordinate(10, 20), new Coordinate(30, 10)])),
        new MultiPoint(
        [
            new Point(10, 40),
            new Point(40, 30),
            new Point(20, 20),
            new Point(30, 10)
        ]),
        new MultiLineString(
        [
            new LineString([new Coordinate(10, 10), new Coordinate(20, 20), new Coordinate(10, 40)]),
            new LineString([new Coordinate(40, 40), new Coordinate(30, 30), new Coordinate(40, 20), new Coordinate(30, 10)])
        ]),
        new MultiPolygon(
        [
            new Polygon(new LinearRing([new Coordinate(30, 20), new Coordinate(45, 40), new Coordinate(10, 40), new Coordinate(30, 20)])),
            new Polygon(new LinearRing([new Coordinate(15, 5), new Coordinate(40, 10), new Coordinate(10, 20), new Coordinate(5, 10), new Coordinate(15, 5)]))
        ]),
        new GeometryCollection(
        [
            new Point(40, 10),
            new LineString([new Coordinate(10, 10), new Coordinate(20, 20), new Coordinate(10, 40)]),
            new Polygon(new LinearRing([new Coordinate(40, 40), new Coordinate(20, 45), new Coordinate(45, 30), new Coordinate(40, 40)]))
        ]),
        new Polygon(
            new LinearRing([new Coordinate(35, 10), new Coordinate(45, 45), new Coordinate(15, 40), new Coordinate(10, 20), new Coordinate(35, 10)]),
            [
                new LinearRing([new Coordinate(20, 30), new Coordinate(35, 35), new Coordinate(30, 20), new Coordinate(20, 30)])
            ]
        ),
        new Polygon(
            new LinearRing([new Coordinate(35, 10), new Coordinate(45, 45), new Coordinate(15, 40), new Coordinate(10, 20), new Coordinate(35, 10)]),
            [
                new LinearRing([new Coordinate(20, 30), new Coordinate(35, 35), new Coordinate(30, 20), new Coordinate(20, 30)]),
                new LinearRing([new Coordinate(4, 6), new Coordinate(7, 7), new Coordinate(6, 4), new Coordinate(4, 6)]),
            ]
        ),
        Point.Empty,
        Polygon.Empty,
        new Point(new CoordinateZM(1, 1, 5, 60))
    ];

    public List<Geometry> Geometries { get; set; } =
    [
        new Point(1.2, 3.4),
        new Point(new CoordinateZM(1.2, 3.4, 5.6, 7.8)),
        new LineString([new Coordinate(0, 0), new Coordinate(0, 1), new Coordinate(1, 1), new Coordinate(1, 0)]),
        new Polygon(new LinearRing([new Coordinate(0, 0), new Coordinate(0, 10), new Coordinate(10, 10), new Coordinate(10, 0), new Coordinate(0, 0)])),
        new Polygon
        (
            new LinearRing([new Coordinate(0, 0), new Coordinate(0, 10), new Coordinate(10, 10), new Coordinate(10, 0), new Coordinate(0, 0)]),
            [
                new LinearRing([new Coordinate(1, 1), new Coordinate(1, 2), new Coordinate(2, 2), new Coordinate(2, 1), new Coordinate(1, 1)]),
                new LinearRing([new Coordinate(3, 3), new Coordinate(3, 4), new Coordinate(4, 4), new Coordinate(4, 3), new Coordinate(3, 3)])
            ]
        ),
        new MultiPoint(
        [
            new Point(0, 0),
            new Point(1, 1),
            new Point(2, 2)
        ]),
        new MultiLineString(
        [
            new LineString([new Coordinate(0, 0), new Coordinate(1, 1)]),
            new LineString([new Coordinate(2, 2), new Coordinate(3, 3)])
        ]),
        new MultiPolygon(
        [
            new Polygon(new LinearRing([new Coordinate(0, 0), new Coordinate(0, 1), new Coordinate(1, 1), new Coordinate(1, 0), new Coordinate(0, 0)])),
            new Polygon(new LinearRing([new Coordinate(2, 2), new Coordinate(2, 3), new Coordinate(3, 3), new Coordinate(3, 2), new Coordinate(2, 2)]))
        ]),
        new GeometryCollection(
        [
            new Point(0, 0),
            new LineString([new Coordinate(1, 1), new Coordinate(2, 2)]),
            new Polygon(new LinearRing([new Coordinate(3, 3), new Coordinate(3, 4), new Coordinate(4, 4), new Coordinate(4, 3), new Coordinate(3, 3)]))
        ]),
        Point.Empty,
        Polygon.Empty,
        LineString.Empty,
        new Polygon(new LinearRing(
        [
            new CoordinateZM(0, 0, 1, 2),
            new CoordinateZM(0, 10, 1, 2),
            new CoordinateZM(10, 10, 1, 2),
            new CoordinateZM(10, 0, 1, 2),
            new CoordinateZM(0, 0, 1, 2)
        ]))
    ];

    public List<Geometry> SpecialGeometries { get; set; } =
    [
        new Point(new CoordinateZM(double.MaxValue, double.MinValue, double.NaN, double.PositiveInfinity)),
        new Polygon(new LinearRing(
        [
            new Coordinate(0, 0),
            new Coordinate(0, 10),
            new Coordinate(10, 10),
            new Coordinate(10, 0),
            new Coordinate(0, 0)
        ]))
    ];
}