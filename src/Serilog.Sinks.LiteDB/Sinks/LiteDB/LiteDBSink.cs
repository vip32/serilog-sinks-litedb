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

using Serilog.Events;
using Serilog.Core;
using LiteDB;
using System.IO;
using Serilog.Formatting;
using Serilog.Sinks.PeriodicBatching;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using System;
using Serilog.Sinks.LiteDB;

namespace Serilog.Sinks.LiteDB
{
    /// <summary>
    /// Writes log events as documents to a LiteDB database.
    /// </summary>
    public class LiteDBSink : PeriodicBatchingSink
    {
        private readonly string _connectionString;
        private readonly RollingPeriod _rollingPeriod;
        private readonly string _logCollectionName;
        private readonly ITextFormatter _formatter;

        /// <summary>
        /// Construct a sink posting to the specified database.
        /// </summary>
        /// <param name="connectionString">The URL of a LiteDB database, or connection string name containing the URL.</param>
        /// <param name="rollingPeriod">When to roll a new file</param>
        /// <param name="batchPostingLimit">The batch posting limit.</param>
        /// <param name="period">The period.</param>
        /// <param name="logCollectionName">Name of the LiteDb collection to use for the log. Default is "log".</param>
        /// <param name="formatter">The formatter. Default is <see cref="LiteDbJsonFormatter" /> used</param>
        public LiteDBSink(
            string connectionString,
            RollingPeriod rollingPeriod,
            int batchPostingLimit = DefaultBatchPostingLimit,
            TimeSpan? period = null,
            string logCollectionName = DefaultLogCollectionName,
            ITextFormatter formatter = null)
             : base(batchPostingLimit, period ?? DefaultPeriod)
        {
            _connectionString = connectionString;
            _rollingPeriod = rollingPeriod;
            _logCollectionName = logCollectionName;
            _formatter = formatter;
        }

        /// <summary>
        /// A reasonable default for the number of events posted in
        /// each batch.
        /// </summary>
        public const int DefaultBatchPostingLimit = 50;

        /// <summary>
        /// The default name for the log collection.
        /// </summary>
        public const string DefaultLogCollectionName = "log";

        /// <summary>
        /// A reasonable default time to wait between checking for event batches.
        /// </summary>
        public static readonly TimeSpan DefaultPeriod = TimeSpan.FromSeconds(2);

        /// <summary>
        /// The default formatter
        /// </summary>
        public static ITextFormatter DefaultFormatter = new LiteDbJsonFormatter();

        /// <summary>
        /// Emit a batch of log events, running asynchronously.
        /// </summary>
        /// <param name="events">The events to emit.</param>
        /// <remarks>
        /// Override either <see cref="M:Serilog.Sinks.PeriodicBatching.PeriodicBatchingSink.EmitBatch(System.Collections.Generic.IEnumerable{Serilog.Events.LogEvent})" /> or <see cref="M:Serilog.Sinks.PeriodicBatching.PeriodicBatchingSink.EmitBatchAsync(System.Collections.Generic.IEnumerable{Serilog.Events.LogEvent})" />,
        /// not both. Overriding EmitBatch() is preferred.
        /// </remarks>
        protected override void EmitBatch(IEnumerable<LogEvent> events)
        {
            using (var db = new LiteDatabase(FileRoller.GetFilename(_connectionString, this._rollingPeriod, DateTime.UtcNow)))
            {
                db.GetCollection(_logCollectionName)
                    .Insert(events.Select(e => AsDocument(e, _formatter)));
            }
        }

        private BsonDocument AsDocument(LogEvent @event, ITextFormatter formatter)
        {
            var sw = new StringWriter();
            formatter.Format(@event, sw);
            return JsonSerializer.Deserialize(new StringReader(sw.ToString())).AsDocument;
        }
    }
}