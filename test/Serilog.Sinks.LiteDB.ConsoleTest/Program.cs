using LiteDB;
using Serilog.Events;
using System;
using System.IO;

namespace Serilog.Sinks.LiteDB.ConsoleTest
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var connectionString = @"c:\tmp\log1.db";

            if (File.Exists(connectionString))
                File.Delete(connectionString);

            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.LiterateConsole(LogEventLevel.Debug)
                .WriteTo.LiteDB(connectionString)
                .Enrich.WithProperty("App", "Serilog.Sinks.LiteDB.ConsoleTest")
                .Enrich.FromLogContext()
                .CreateLogger();

            Log.Information("datetime {date}, string {text}, integer {num}", DateTime.Now, "hallo", 123);
            Log.Information("integer {nummer}", 123);
            Log.Information("string {text}", "hallo");
            Log.Information("datetime {DateNow}", DateTime.Now);
            Log.Fatal(new ArgumentNullException("haha"), "exception {msg}", "Exception!");

            using (var db = new LiteDatabase(connectionString))
            {
                var logEvents = db.GetCollection<BsonDocument>("log").FindAll();
                foreach(var logEvent in logEvents)
                {
                    Console.WriteLine(logEvent.ToString());
                }
            }
        }
    }
}
