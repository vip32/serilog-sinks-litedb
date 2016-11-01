using LiteDB;
using Serilog.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Serilog.Sinks.MongoDB.Sinks.LiteDB
{
    /// <summary>
    /// Provides bson mapping services
    /// </summary>
    public static class DocumentMapper
    {
        private static readonly BsonMapper _mapper;

        static DocumentMapper()
        {
            _mapper = new BsonMapper();
            _mapper.Entity<LogEvent>()
                .Ignore(x => x.Exception);
        }

        /// <summary>
        /// Maps the specified source.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="formatProvider">The format provider.</param>
        /// <returns></returns>
        public static BsonDocument MapLogEvent(LogEvent source, IFormatProvider formatProvider)
        {
            var target = _mapper.ToDocument(source);
            target.Add("_id", ObjectId.NewObjectId());
            target.Add("Timestamp", source.Timestamp.ToUniversalTime().DateTime);
            //target.Add("Ticks", source.Timestamp.ToUniversalTime().Ticks);
            target.Add("Message", new BsonValue(source.RenderMessage(formatProvider)));
            if (source.Exception != null)
            {
                target.Add("Exception", MapException(source.Exception));
            }
            return target;
        }

        /// <summary>
        /// Maps the specified source.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <returns></returns>
        public static BsonDocument MapException(Exception source)
        {
            var target = new BsonDocument();
            target.Add("_type", new BsonValue(source.GetType().AssemblyQualifiedName));
            target.Add("Message", new BsonValue(source.Message));
            if (source.HResult > 0) target.Add("HResult", new BsonValue(source.HResult));
            if (!string.IsNullOrEmpty(source.HelpLink)) target.Add("HelpLink", new BsonValue(source.HelpLink));
            if (!string.IsNullOrEmpty(source.Source)) target.Add("Source", new BsonValue(source.Source));
            if (!string.IsNullOrEmpty(source.StackTrace)) target.Add("StackTrace", new BsonValue(source.StackTrace));
            if (source.InnerException != null)
            {
                target.Add("InnerException", MapException(source.InnerException));
            }
            return target;
        }

        //public static BsonDocument Convert(LogEvent logEvent)
        //{
        //    var doc = new BsonDocument();
        //    doc["_id"] = ObjectId.NewObjectId();
        //    doc["Timestamp"] = logEvent.Timestamp.ToUniversalTime().DateTime;
        //    doc["Level"] = logEvent.Level.ToString();
        //    doc["Template"] = logEvent.MessageTemplate.Text;
        //    doc["Message"] = logEvent.RenderMessage(_formatProvider);
        //    if (logEvent.Exception != null)
        //    {
        //        doc.Set($"Exception.Message", new BsonValue(logEvent.Exception.Message));
        //        doc.Set($"Exception.StackTrace", new BsonValue(logEvent.Exception.StackTrace));
        //        doc.Set($"Exception.Source", new BsonValue(logEvent.Exception.Source));
        //    }
        //    if (logEvent.Properties != null)
        //    {
        //        foreach (var property in logEvent.Properties)
        //        {
        //            var scalar = property.Value as ScalarValue;
        //            if (scalar != null)
        //            {
        //                doc.Set($"Properties.{property.Key}", new BsonValue(scalar.Value));
        //            }

        //            // TODO
        //            //var seq = property.Value as SequenceValue;
        //            //if (seq != null)
        //            //{
        //            //    doc.Set($"Properties.{property.Key}", new BsonValue(seq));
        //            //}

        //            //var str = property.Value as StructureValue;
        //            //if (str != null)
        //            //{
        //            //    doc.Set($"Properties.{property.Key}", new BsonValue(str));
        //            //}

        //            //var div = property.Value as DictionaryValue;
        //            //if (div != null)
        //            //{
        //            //    doc.Set($"Properties.{property.Key}", new BsonValue(property.Value.ToString()));
        //            //}
        //        }
        //    }

        //    return doc;
        //}
    }
}
