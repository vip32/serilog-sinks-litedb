# Serilog.Sinks.LiteDB

[![NuGet](https://img.shields.io/nuget/v/serilog.sinks.litedb.svg)](https://www.nuget.org/packages/serilog.sinks.litedb/)

A Serilog sink that writes events as documents to [LiteDB](http://litedb.org).

**Package** - [Serilog.Sinks.LiteDB](http://nuget.org/packages/serilog.sinks.litedb)
| **Platforms** - net45, netstandard2.0, net6.0, net8.0


In the example shown, the sink will write to the database `logs`. The default collection name is `log`, but a custom collection can be supplied with the optional `CollectionName` parameter.
The database and collection will be created if they do not exist.

```csharp
// basic usage defaults to writing to `log` collection
var log = new LoggerConfiguration()
    .WriteTo.LiteDB(@"c:\tmp\logs.db")
    .CreateLogger();

// creates custom collection `applog`
var log = new LoggerConfiguration()
    .WriteTo.LiteDB(@"c:\tmp\logs.db", collectionName: "applog")
    .CreateLogger();
```
