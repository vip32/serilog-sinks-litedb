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
            var mapper = new BsonMapper();

            if (File.Exists(connectionString))
                File.Delete(connectionString);

            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.LiterateConsole(LogEventLevel.Debug)
                .WriteTo.LiteDB(connectionString)
                .Enrich.WithProperty("App", "Serilog.Sinks.LiteDB.ConsoleTest")
                .Enrich.FromLogContext()
                .CreateLogger();

            var customer = new Customer() { Name = "cust1", Id = 23, Culture = "en-GB" };

            Log.Information("datetime {date}, string {text}, integer {num}", DateTime.Now, "hallo", 123);
            Log.Information("new customer {@customer}", customer);
            Log.Information("integer {nummer}", 123);
            Log.Information("string {text}", "hallo");
            Log.Information("datetime {DateNow}", DateTime.Now);
            Log.Fatal(new ArgumentNullException("haha", new Exception("INNER")), "exception {msg}", "Exception!");

            using (var db = new LiteDatabase(connectionString))
            {
                var documents = db.GetCollection<BsonDocument>("log").FindAll();
                foreach(var document in documents)
                {
                    Console.WriteLine(document.ToString());

                    //var logEvent = new LogEvent(document["Timestamp"].AsDateTime, LogEventLevel.Information, null, new MessageTemplate(document["MessageTemplate"].AsDocument["Text"], document["MessageTemplate"].AsDocument["Tokens"].AsArray), null);
                }
            }
        }
    }

    public class Customer
    {
        public int Id { get; set; }
        public string Key { get; set; }
        public string Culture { get; set; }
        public string Name { get; set; }
        public string[] Phones { get; set; }
        public bool IsActive { get; set; }
        public PlatformID OS { get; set; }
    }
}
