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
using Serilog.Formatting.Compact;
using Serilog.Formatting.Json;
using System.IO;
using Serilog.Formatting;

namespace Serilog.Sinks.MongoDB.Sinks.LiteDB
{
    /// <summary>
    /// Writes log events as documents to a LiteDB database.
    /// </summary>
    public class LiteDBSink : ILogEventSink
    {
        private readonly string _connectionString;
        private readonly string _collectionName;
        private readonly ITextFormatter _formatter;
        private readonly bool _includeMessageTemplate;

        /// <summary>
        /// Construct a sink posting to the specified database.
        /// </summary>
        /// <param name="connectionString">The URL of a LiteDB database, or connection string name containing the URL.</param>
        /// <param name="collectionName">Name of the LiteDb collection to use for the log. Default is "log".</param>
        /// <param name="includeMessageTemplate">if set to <c>true</c> [include message template].</param>
        /// <param name="formatter">The formatter. Default is <see cref="RenderedCompactJsonFormatter" /> used</param>
        public LiteDBSink(
            string connectionString,
            string collectionName = DefaultCollectionName,
            bool includeMessageTemplate = false,
            ITextFormatter formatter = null)
        {
            _connectionString = connectionString;
            _collectionName = collectionName;
            _includeMessageTemplate = includeMessageTemplate;
            _formatter = formatter;
        }

        /// <summary>
        /// The default name for the log collection.
        /// </summary>
        public const string DefaultCollectionName = "log";

        /// <summary>
        /// The default formatter
        /// </summary>
        public static ITextFormatter DefaultFormatter = new RenderedCompactJsonFormatter();

        /// <summary>
        /// Emit the provided log event to the sink.
        /// </summary>
        /// <param name="logEvent">The log event to write.</param>
        public void Emit(LogEvent logEvent)
        {
            using (var db = new LiteDatabase(_connectionString))
            {
                var sw = new StringWriter();
                _formatter.Format(logEvent, sw);
                var doc = JsonSerializer.Deserialize(
                        new StringReader(sw.ToString().Replace("@", "_"))).AsDocument;
                if(_includeMessageTemplate)
                    doc.Set("_mt", new BsonValue(logEvent.MessageTemplate.Text));
                db.GetCollection(_collectionName).Insert(doc);
            }
        }
    }
}