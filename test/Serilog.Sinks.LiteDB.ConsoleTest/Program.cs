using LiteDB;
using Serilog.Events;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace Serilog.Sinks.LiteDB.ConsoleTest
{
    public static class TestFileRoller
    {
        private const string connStr = "filename=c:\\tmp\\file.db";
        public static void TestDaily()
        {
            var expected = "filename=c:\\tmp\\file_1950-01-12.db";
            var file = FileRoller.GetFilename(connStr, RollingPeriod.Daily, new DateTime(1950, 01, 12));
            Debug.Assert(expected == file);
        }

        public static void TestQuarterly()
        {
            const RollingPeriod period = RollingPeriod.Quarterly;
            
            var expected = "filename=c:\\tmp\\file_1950-01-12-1000.db";
            var file = FileRoller.GetFilename(connStr, period, new DateTime(1950, 01, 12, 10, 4, 0));
            Debug.Assert(expected == file);
            expected = "filename=c:\\tmp\\file_1950-01-12-1000.db";
            file = FileRoller.GetFilename(connStr, period, new DateTime(1950, 01, 12, 10, 14, 0));
            Debug.Assert(expected == file);

            expected = "filename=c:\\tmp\\file_1950-01-12-1015.db";
            file = FileRoller.GetFilename(connStr, period, new DateTime(1950, 01, 12, 10, 15, 0));
            Debug.Assert(expected == file);

            expected = "filename=c:\\tmp\\file_1950-01-12-1030.db";
            file = FileRoller.GetFilename(connStr, period, new DateTime(1950, 01, 12, 10, 30, 0));
            Debug.Assert(expected == file);
        }

        public static void TestHourly()
        {
            var expected = "filename=c:\\tmp\\file_1950-01-12-10.db";
            var file = FileRoller.GetFilename(connStr, RollingPeriod.Hourly, new DateTime(1950, 01, 12, 10, 4, 30));
            Debug.Assert(expected == file);

            expected = "filename=c:\\tmp\\file_1950-01-12-10.db";
            file = FileRoller.GetFilename(connStr, RollingPeriod.Hourly, new DateTime(1950, 01, 12, 10, 31, 0));
            Debug.Assert(expected == file);
        }
        public static void TestHalfHourly()
        {
            var expected = "filename=c:\\tmp\\file_1950-01-12-1000.db";
            var file = FileRoller.GetFilename(connStr, RollingPeriod.HalfHour, new DateTime(1950, 01, 12, 10, 4, 30));
            Debug.Assert(expected == file);

            expected = "filename=c:\\tmp\\file_1950-01-12-1030.db";
            file = FileRoller.GetFilename(connStr, RollingPeriod.HalfHour, new DateTime(1950, 01, 12, 10, 31, 0));
            Debug.Assert(expected == file);
        }
    }
    public class Program
    {
        private static readonly ILogger _logger = Log.ForContext<Program>();

        public static void Main(string[] args)
        {
            TestFileRoller.TestDaily();
            TestFileRoller.TestQuarterly();
            TestFileRoller.TestHalfHourly();
            TestFileRoller.TestHourly();
            var connectionString = @"filename=c:\tmp\log1.db";
            var mapper = new BsonMapper();

            if (File.Exists(connectionString))
                File.Delete(connectionString);

            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.LiterateConsole(LogEventLevel.Debug)
                .WriteTo.LiteDB(connectionString, rollingFilePeriod: RollingPeriod.Quarterly)
                .Enrich.WithProperty("app", "Serilog.Sinks.LiteDB.ConsoleTest")
                .Enrich.FromLogContext()
                .CreateLogger();

            var customer = new Customer() { Name = "cust1", Id = 23, Culture = "en-GB" };

            var logger = Log.ForContext<Program>();
            logger.Debug("TEST1");
            logger.Debug("datetime {date}, string {text}, integer {nummer}", DateTime.Now, "hallo", 105);
            logger.Debug("new customer {@customer}", customer);
            logger.Debug("integer {nummer}", 123);
            logger.Debug("string {text}", "hallo");
            logger.Debug("datetime {date}", DateTime.Now.AddHours(-3));
            logger.Fatal(new ArgumentNullException("haha", new Exception("INNER")), "exception {msg}", "Exception!");

            System.Threading.Thread.Sleep(6000);

            using (var db = new LiteDatabase(FileRoller.GetFilename(connectionString, RollingPeriod.Quarterly, DateTime.UtcNow)))
            {
                var result1 = db.GetCollection<BsonDocument>("log").FindOne(Query.Contains("_m", "Test"));
                Console.WriteLine(result1.ToString());
                Console.WriteLine("");

                var result2 = db.GetCollection<BsonDocument>("log").Find(Query.EQ("app", "Serilog.Sinks.LiteDB.ConsoleTest"));
                Console.WriteLine(result2.Count());
                Console.WriteLine("");

                var result3 = db.GetCollection<BsonDocument>("log").Find(Query.GTE("nummer", 100));
                Console.WriteLine(result3.Count());
                Console.WriteLine("");

                var result4 = db.GetCollection<BsonDocument>("log").Find(Query.GTE("date", DateTime.Now.AddHours(-1)));
                Console.WriteLine(result4.Count());
                Console.WriteLine("");

                //Console.WriteLine(db.Run("db.info").ToString());

                var coll = db.GetCollection<BsonDocument>("log");
                coll.Insert(new BsonDocument(new System.Collections.Generic.Dictionary<string, BsonValue>
                    {
                        { "name", new BsonValue("name1") },
                        { "created", new BsonValue(DateTime.Now) },
                    }
                ));

                var a = db.Run("db.log.find limit 10");
                foreach(var a1 in a.AsArray)
                {
                    var jsonString = JsonSerializer.Serialize(a1, true);
                    Console.WriteLine(jsonString);
                }

                var documents = coll.FindAll();
                foreach (var document in documents)
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
