# DuckDb.Spatial.IO.Net

This library provides a seamless way to convert geometries between DuckDB's internal geometry format and NetTopologySuite (NTS) geometries in C#. It supports reading and writing geometries from/to byte arrays and streams, making it easy to integrate with DuckDB databases and work with spatial data in .NET applications.

[![Nuget](https://img.shields.io/nuget/v/DuckDb.Spatial.IO.Net)](https://www.nuget.org/packages/DuckDb.Spatial.IO.Net/)

## Installation

You can install the library via NuGet:

```bash
dotnet add package DuckDb.Spatial.IO.Net
```

---

## Usage

Below are examples demonstrating how to use the library for common operations: reading from and writing to byte arrays and streams.

### 1. Reading Geometry from a Byte Array

To convert a DuckDB geometry stored as a byte array into an NTS geometry:

```csharp
using DuckDb.Spatial.IO.Net;

...

var reader = new GeometryReader();
var geometry = reader.Read(bytes);
```

---

### 2. Reading Geometry from a Stream

If your DuckDB geometry is stored in a stream, you can read it directly:

```csharp
using DuckDb.Spatial.IO.Net;

...

var reader = new GeometryReader();
var geometry = reader.Read(stream);
```

---

### 3. Writing Geometry to a Byte Array

To convert an NTS geometry into a DuckDB-compatible byte array:

```csharp
using DuckDb.Spatial.IO.Net;

...

var writer = new GeometryWriter();
var bytes = writer.Write(geometry);
```

---

### 4. Writing Geometry to a Stream

If you need to write an NTS geometry to a stream in DuckDB-compatible format:

```csharp
using DuckDb.Spatial.IO.Net;

...

var writer = new GeometryWriter();
writer.Write(geometry, stream);
```

---

## Supported Geometry Types

The library supports all geometry types compatible with both DuckDB and NetTopologySuite, including:

- Point
- LineString
- Polygon
- MultiPoint
- MultiLineString
- MultiPolygon
- GeometryCollection

---

## Contributing

Contributions are welcome! If you find any issues or have suggestions for improvements, please open an issue or submit a pull request.

---

## License

This project is licensed under the MIT License. See the [LICENSE](LICENSE) file for details.
