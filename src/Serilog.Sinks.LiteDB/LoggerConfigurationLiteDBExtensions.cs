// Copyright 2014-2016 Serilog Contributors
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using System;
using Serilog.Configuration;
using Serilog.Events;
using Serilog.Formatting;
using Serilog.Sinks.LiteDB;

namespace Serilog
{
    /// <summary>
    /// Adds the WriteTo.LiteDB() extension method to <see cref="LoggerConfiguration"/>.
    /// </summary>
    public static class LoggerConfigurationLiteDBExtensions
    {
        /// <summary>
        /// Adds a sink that writes log events as documents to a LiteDb database.
        /// </summary>
        /// <param name="loggerConfiguration">The logger configuration.</param>
        /// <param name="databaseUrl">The URL of a created LiteDB collection that log events will be written to.</param>
        /// <param name="logCollectionName">Name of the collection. Default is "log".</param>
        /// <param name="rollingFilePeriod">Which period must be used to create a new file. 'Never' value applies default behavior (never create a new file)</param>
        /// <param name="restrictedToMinimumLevel">The minimum log event level required in order to write an event to the sink.</param>
        /// <param name="batchPostingLimit">The batch posting limit.</param>
        /// <param name="period">The period.</param>
        /// <param name="formatter">The formatter.</param>
        /// <returns>
        /// Logger configuration, allowing configuration to continue.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">loggerConfiguration
        /// or
        /// databaseUrl</exception>
        /// <exception cref="ArgumentNullException">A required parameter is null.</exception>
        public static LoggerConfiguration LiteDB(
            this LoggerSinkConfiguration loggerConfiguration,
            string databaseUrl,
            string logCollectionName = LiteDBSink.DefaultLogCollectionName,
            RollingPeriod rollingFilePeriod = RollingPeriod.Never,
            LogEventLevel restrictedToMinimumLevel = LevelAlias.Minimum,
            int batchPostingLimit = LiteDBSink.DefaultBatchPostingLimit,
            TimeSpan? period = null,
            ITextFormatter formatter = null)
        {
            if (loggerConfiguration == null) throw new ArgumentNullException(nameof(loggerConfiguration));
            if (string.IsNullOrWhiteSpace(databaseUrl)) throw new ArgumentNullException(nameof(databaseUrl));
            if (string.IsNullOrWhiteSpace(logCollectionName)) throw new ArgumentNullException(nameof(logCollectionName));
            if (formatter == null) formatter = LiteDBSink.DefaultFormatter;

            return loggerConfiguration.Sink(
                    new LiteDBSink(databaseUrl, rollingFilePeriod, batchPostingLimit, period, logCollectionName, formatter),
                    restrictedToMinimumLevel);
        }
    }
}
