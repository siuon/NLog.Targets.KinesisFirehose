using NLog.Config;
using NLog.Layouts;
using System;

namespace NLog.Targets.KinesisFirehose
{
    [NLogConfigurationItem]
    public class Field
    {
        [RequiredParameter]
        public string Name { get; set; }

        [RequiredParameter]
        public Layout Layout { get; set; }

        public Type LayoutType { get; set; } = typeof(String);
    }
}
