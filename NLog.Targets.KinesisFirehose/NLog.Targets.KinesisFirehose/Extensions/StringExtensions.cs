using System;

namespace NLog.Targets.KinesisFirehose.Extensions
{
    internal static class StringExtensions
    {
        public static object ToSystemType(this string value, Type type, IFormatProvider formatProvider)
        {
            switch (type.FullName)
            {
                case "System.Boolean":
                    return Convert.ToBoolean(value, formatProvider);
                case "System.DateTime":
                    return Convert.ToDateTime(value, formatProvider);
                case "System.Double":
                    return Convert.ToDouble(value, formatProvider);
                case "System.Int16":
                    return Convert.ToInt16(value, formatProvider);
                case "System.Int32":
                    return Convert.ToInt32(value, formatProvider);
                case "System.Int64":
                    return Convert.ToInt64(value, formatProvider);
                default:
                    return value;
            }
        }
    }
}
