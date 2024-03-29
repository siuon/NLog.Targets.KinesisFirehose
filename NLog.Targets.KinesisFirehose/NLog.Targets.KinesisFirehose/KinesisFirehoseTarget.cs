using Amazon.KinesisFirehose;
using Amazon.KinesisFirehose.Model;
using Newtonsoft.Json;
using NLog.Common;
using NLog.Config;
using NLog.Targets.KinesisFirehose.Extensions;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace NLog.Targets.KinesisFirehose
{
    [Target("KinesisFirehose")]
    public class KinesisFirehose : TargetWithLayout
    {
        [RequiredParameter]
        public string EndPointName { get; set; }

        [RequiredParameter]
        public string StreamName { get; set; }

        public string AwsAccessKeyId { get; set; }

        public string AwsSecretAccessKey { get; set; }
        [ArrayParameter(typeof(Field), "field")]
        public IList<Field> Fields { get; set; } = new List<Field>();

        private AmazonKinesisFirehoseClient _client;

        protected override void InitializeTarget()
        {
            var regionEndpoint = Amazon.RegionEndpoint.GetBySystemName(EndPointName) ?? throw new InvalidArgumentException(nameof(EndPointName));

            if (!string.IsNullOrEmpty(AwsAccessKeyId)
             && !string.IsNullOrEmpty(AwsSecretAccessKey))
            {
                _client = new AmazonKinesisFirehoseClient(AwsAccessKeyId, AwsSecretAccessKey, regionEndpoint);
            }
            else
            {
                _client = new AmazonKinesisFirehoseClient(regionEndpoint);
            }

            base.InitializeTarget();
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (disposing)
            {
                _client.Dispose();
            }
        }

        protected override void Write(LogEventInfo logEvent)
        {
            PutRecord(logEvent);
        }

        protected override void Write(AsyncLogEventInfo logEvent)
        {
            PutRecordAsync(logEvent.LogEvent);
        }

        protected override void Write(IList<AsyncLogEventInfo> logEvents)
        {
            foreach (var logEvent in logEvents)
            {
                PutRecordAsync(logEvent.LogEvent);
            }
        }

        private void PutRecord(LogEventInfo logEvent)
        {
            var request = GetPutRecordRequest(logEvent);

            _client.PutRecordAsync(request);
        }

        private void PutRecordAsync(LogEventInfo logEvent)
        {
            var request = GetPutRecordRequest(logEvent);

            Task.Run(() => _client.PutRecordAsync(request));
        }

        private PutRecordRequest GetPutRecordRequest(LogEventInfo logEvent)
        {
            var fieldValues = GetFieldValues(logEvent);
            var jsonData = JsonConvert.SerializeObject(fieldValues);
            var bytes = Encoding.UTF8.GetBytes(jsonData);

            return new PutRecordRequest
            {
                DeliveryStreamName = StreamName,
                Record = new Record
                {
                    Data = new MemoryStream(bytes)
                }
            };
        }

        private Dictionary<string, object> GetFieldValues(LogEventInfo logEvent)
        {
            var documents = new Dictionary<string, object>();

            foreach (var field in Fields)
            {
                var renderedField = RenderLogEvent(field.Layout, logEvent);

                if (string.IsNullOrWhiteSpace(renderedField))
                { continue; }

                documents.Add(field.Name, renderedField.ToSystemType(field.LayoutType, logEvent.FormatProvider));
            }

            return documents;
        }
    }
}
