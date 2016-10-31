# Serilog.Sinks.LiteDB

[![Build status](https://ci.appveyor.com/api/projects/status/50a20wxfl1klrsra/branch/master?svg=true)](https://ci.appveyor.com/project/vip32/serilog-sinks-litedb/branch/master)

A Serilog sink that writes events as documents to [LiteDB](http://litedb.org).

**Package** - [Serilog.Sinks.LiteDB](http://nuget.org/packages/serilog.sinks.litedb)
| **Platforms** - .NET 4.6


In the example shown, the sink will write to the database `logs`. The default collection name is `log`, but a custom collection can be supplied with the optional `CollectionName` parameter.
The database and collection will be created if they do not exist.

```csharp
// basic usage defaults to writing to `log` collection
var log = new LoggerConfiguration()
    .WriteTo.LiteDB("c:\tmp\logs.db")
    .CreateLogger();

// creates custom collection `applog`
var log = new LoggerConfiguration()
    .WriteTo.LiteDB("c:\tmp\logs.db", collectionName: "applog")
    .CreateLogger();
```