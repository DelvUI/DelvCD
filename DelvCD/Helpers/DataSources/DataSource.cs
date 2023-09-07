using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace DelvCD.Helpers.DataSources
{
    public abstract class DataSource
    {
        public static string GetDisplayName() => "";

        public uint Id;
        public uint Icon;

        public abstract float ProgressValue { get; set; }
        public abstract float ProgressMaxValue { get; }

        protected List<string> _conditionFieldNames = new();
        public List<string> ConditionFieldNames => _conditionFieldNames;

        public abstract float GetConditionValue(int index);

        public string GetFormattedString(string format, string numberFormat, int rounding)
        {
            return TextTagFormatter.TextTagRegex.Replace(
                format,
                new TextTagFormatter(this, numberFormat, rounding, GetType()).Evaluate);
        }
    }
}
