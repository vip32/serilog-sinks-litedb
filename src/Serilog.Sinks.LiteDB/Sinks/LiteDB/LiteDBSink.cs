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
using Serilog.Events;
using Serilog.Core;
using LiteDB;

namespace Serilog.Sinks.LiteDB
{
    /// <summary>
    /// Writes log events as documents to a LiteDB database.
    /// </summary>
    public class LiteDBSink : ILogEventSink
    {
        private readonly string _connectionString;
        private readonly IFormatProvider _formatProvider;
        private readonly string _collectionName;

        /// <summary>
        /// Construct a sink posting to the specified database.
        /// </summary>
        /// <param name="connectionString">The URL of a LiteDB database, or connection string name containing the URL.</param>
        /// <param name="formatProvider">Supplies culture-specific formatting information, or null.</param>
        /// <param name="collectionName">Name of the LiteDb collection to use for the log. Default is "log".</param>
        /// <param name="collectionCreationOptions">Collection Creation Options for the log collection creation.</param>
        public LiteDBSink(
            string connectionString,
            IFormatProvider formatProvider = null,
            string collectionName = DefaultCollectionName)
        {
            _connectionString = connectionString;
            _formatProvider = formatProvider;
            _collectionName = collectionName;
        }

        /// <summary>
        /// A reasonable default for the number of events posted in
        /// each batch.
        /// </summary>
        public const int DefaultBatchPostingLimit = 50;

        /// <summary>
        /// A reasonable default time to wait between checking for event batches.
        /// </summary>
        public static readonly TimeSpan DefaultPeriod = TimeSpan.FromSeconds(2);

        /// <summary>
        /// The default name for the log collection.
        /// </summary>
        public const string DefaultCollectionName = "log";

        /// <summary>
        /// Emit the provided log event to the sink.
        /// </summary>
        /// <param name="logEvent">The log event to write.</param>
        public void Emit(LogEvent logEvent)
        {
            using (var db = new LiteDatabase(_connectionString))
            {
                db.GetCollection(_collectionName)
                    .Insert(Convert(logEvent));
            }
        }

        private  BsonDocument Convert(LogEvent logEvent)
        {
            var doc = new BsonDocument();
            doc["_id"] = ObjectId.NewObjectId();
            doc["Timestamp"] = logEvent.Timestamp.ToUniversalTime().DateTime;
            //doc["Timestamp"] = logEvent.Timestamp.UtcDateTime;
            doc["Level"] = logEvent.Level.ToString();
            doc["Template"] = logEvent.MessageTemplate.Text;
            doc["Message"] = logEvent.RenderMessage(_formatProvider);
            if (logEvent.Exception != null)
            {
                // TODO
                doc.Set($"Exception.Message", new BsonValue(logEvent.Exception.Message));
                doc.Set($"Exception.StackTrace", new BsonValue(logEvent.Exception.StackTrace));
                doc.Set($"Exception.Source", new BsonValue(logEvent.Exception.Source));
            }
            if (logEvent.Properties != null)
            {
                foreach (var property in logEvent.Properties)
                {
                    var scalar = property.Value as ScalarValue;
                    if (scalar != null)
                    {
                        doc.Set($"Properties.{property.Key}", new BsonValue(scalar.Value));
                    }
                    //var seq = property.Value as SequenceValue;
                    //if (seq != null)
                    //{
                    //    doc.Set($"Properties.{property.Key}", new BsonValue(seq));
                    //}

                    //var str = property.Value as StructureValue;
                    //if (str != null)
                    //{
                    //    doc.Set($"Properties.{property.Key}", new BsonValue(str));
                    //}

                    //var div = property.Value as DictionaryValue;
                    //if (div != null)
                    //{
                    //    doc.Set($"Properties.{property.Key}", new BsonValue(property.Value.ToString()));
                    //}
                }
            }

            return doc;
        }
    }
}