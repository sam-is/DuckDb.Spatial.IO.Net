using NetTopologySuite.Geometries;

namespace DuckDb.Spatial.IO.Net.Tests.Fixture;

public class DataFixture
{
    public List<Geometry> DbGeometries { get; set; } =
    [
        new Point(30, 10),
        new LineString([new(30, 10), new(10, 30), new(40, 40)]),
        new Polygon(new LinearRing([new(30, 10), new(40, 40), new(20, 40), new(10, 20), new(30, 10)])),
        new MultiPoint(
        [
            new Point(10, 40),
            new Point(40, 30),
            new Point(20, 20),
            new Point(30, 10)
        ]),
        new MultiLineString(
        [
            new LineString([new(10, 10), new(20, 20), new(10, 40)]),
            new LineString([new(40, 40), new(30, 30), new(40, 20), new(30, 10)])
        ]),
        new MultiPolygon(
        [
            new Polygon(new LinearRing([new(30, 20), new(45, 40), new(10, 40), new(30, 20)])),
            new Polygon(new LinearRing([new(15, 5), new(40, 10), new(10, 20), new(5, 10), new(15, 5)]))
        ]),
        new GeometryCollection(
        [
            new Point(40, 10),
            new LineString([new(10, 10), new(20, 20), new(10, 40)]),
            new Polygon(new LinearRing([new(40, 40), new(20, 45), new(45, 30), new(40, 40)]))
        ]),
        new Polygon(
            new LinearRing([new(35, 10), new(45, 45), new(15, 40), new(10, 20), new(35, 10)]),
            [
                new LinearRing([new(20, 30), new(35, 35), new(30, 20), new(20, 30)])
            ]
        ),
        new Polygon(
            new LinearRing([new(35, 10), new(45, 45), new(15, 40), new(10, 20), new(35, 10)]),
            [
                new LinearRing([new(20, 30), new(35, 35), new(30, 20), new(20, 30)]),
                new LinearRing([new(4, 6), new(7, 7), new(6, 4), new(4, 6)]),
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
        new LineString([new(0, 0), new(0, 1), new(1, 1), new(1, 0)]),
        new Polygon(new LinearRing([new(0, 0), new(0, 10), new(10, 10), new(10, 0), new(0, 0)])),
        new Polygon
        (
            new LinearRing([new(0, 0), new(0, 10), new(10, 10), new(10, 0), new(0, 0)]),
            [
                new LinearRing([new(1, 1), new(1, 2), new(2, 2), new(2, 1), new(1, 1)]),
                new LinearRing([new(3, 3), new(3, 4), new(4, 4), new(4, 3), new(3, 3)])
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
            new LineString([new(0, 0), new(1, 1)]),
            new LineString([new(2, 2), new(3, 3)])
        ]),
        new MultiPolygon(
        [
            new Polygon(new LinearRing([new(0, 0), new(0, 1), new(1, 1), new(1, 0), new(0, 0)])),
            new Polygon(new LinearRing([new(2, 2), new(2, 3), new(3, 3), new(3, 2), new(2, 2)]))
        ]),
        new GeometryCollection(
        [
            new Point(0, 0),
            new LineString([new(1, 1), new(2, 2)]),
            new Polygon(new LinearRing([new(3, 3), new(3, 4), new(4, 4), new(4, 3), new(3, 3)]))
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
            new(0, 0),
            new(0, 10),
            new(10, 10),
            new(10, 0),
            new(0, 0)
        ]))
    ];
}
