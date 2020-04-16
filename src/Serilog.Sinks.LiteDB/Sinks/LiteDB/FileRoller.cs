using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Serilog.Sinks.LiteDB
{
    /// <summary>
    /// Auxiliary class to make sure filename is named using rolling configuration.
    /// </summary>
    public static class FileRoller
    {
        /// <summary>
        /// Parses the connection string and resolves the filename by using rolling period.
        /// </summary>
        /// <param name="connectionString">Connection string to bre parsed</param>
        /// <param name="period">Period</param>
        /// <param name="date">Date</param>
        /// <returns></returns>
        public static string GetFilename(string connectionString, RollingPeriod period, DateTime date)
        {
            var c = ConnectionStringParser.Parse(connectionString);
            var fullpath = c["filename"];

            var filename = Path.GetFileName(fullpath);
            var extension = Path.GetExtension(filename);
            var file = Path.GetFileNameWithoutExtension(filename);
            var folderPart = Path.GetDirectoryName(fullpath) ?? ("." + Path.DirectorySeparatorChar);
            var dateFormat = "yyyy-MM-dd-HHmm";

            switch (period)
            {
                case RollingPeriod.Never:
                    return fullpath;
                case RollingPeriod.HalfHour:
                case RollingPeriod.Quarterly:
                    var itv = period == RollingPeriod.Quarterly ? 15 : 30;
                    var dateMinutes = date.Minute / itv * itv;
                    date = date.AddMinutes(-date.Minute).AddMinutes(dateMinutes);
                    break;
                case RollingPeriod.Weekly:
                case RollingPeriod.Daily:
                    dateFormat = "yyyy-MM-dd";
                    break;
                case RollingPeriod.Monthly:

                    dateFormat = "yyyy-MM";
                    break;
                case RollingPeriod.Hourly:
                    dateFormat = "yyyy-MM-dd-HH";
                    break;

                default:
                    throw new InvalidOperationException($"period cannot be parsed: {period}");

            }

            file = file + "_" + date.ToString(dateFormat);
            c["filename"] = Path.Combine(folderPart, $"{file}{extension}");
            return ConnectionStringParser.Create(c);

        }

        private static class ConnectionStringParser
        {
            public static IDictionary<string, string> Parse(string str)
            {
                if (str.IndexOf(";", StringComparison.Ordinal) == -1 && str.IndexOf("=", StringComparison.Ordinal) == -1)
                {
                    return new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
                    {
                        ["filename"] = str
                    };
                }
                var all = from kvp in str.Split(';')
                    let kv = kvp.Split('=')
                    select new { key = kv.First(), value = kv.Last() };
                return all.ToDictionary(a => a.key, a => a.value, StringComparer.CurrentCultureIgnoreCase);
            }

            public static string Create(IDictionary<string, string> connStrInfo)
            {
                return string.Join(";", connStrInfo.Select(a => string.Join("=", a.Key, a.Value)));
            }
        }

    }
}