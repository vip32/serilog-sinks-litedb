using Serilog.Events;
using System;

namespace Serilog.Sinks.LiteDB.ConsoleTest
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.LiterateConsole(LogEventLevel.Debug)
                .WriteTo.LiteDB(@"c:\tmp\log1.db")
                .Enrich.WithProperty("App", "Serilog.Sinks.LiteDB.ConsoleTest")
                .Enrich.FromLogContext()
                .CreateLogger();

            Log.Information("datetime {date}, string {text}, integer {num}", DateTime.Now, "hallo", 123);
            Log.Information("integer {nummer}", 123);
            Log.Information("string {text}", "hallo");
            Log.Information("datetime {DateNow}", DateTime.Now);
        }
    }
}
