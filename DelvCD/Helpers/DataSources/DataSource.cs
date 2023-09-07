using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace DelvCD.Helpers.DataSources
{
    public abstract class DataSource
    {
        public uint Id;
        public uint Icon;

        public static string GetFriendlyName() { return ""; }
        public abstract float ProgressValue { get; set; }
        public abstract float ProgressMaxValue { get; set; }


        protected List<FieldInfo> _conditionFields = new();
        protected string[] _conditionFieldNames = new string[] { };

        protected void SetConditionFields(params string[] fieldNames)
        {
            foreach (string name in fieldNames)
            {
                FieldInfo? field = GetType().GetField(name);
                if (field != null)
                {
                    _conditionFields.Add(field);
                }
            }

            _conditionFieldNames = _conditionFields.Select(x => x.Name).ToArray();
        }

        public string[] GetConditionList()
        {
            return _conditionFieldNames;
        }

        public float GetConditionValue(int index)
        {
            if (index >= _conditionFields.Count) { return 0; }

            object? value = _conditionFields[index].GetValue(this);
            return Convert.ToSingle(value);
        }

        public string GetFormattedString(string format, string numberFormat, int rounding)
        {
            return TextTagFormatter.TextTagRegex.Replace(
                format,
                new TextTagFormatter(this, numberFormat, rounding, GetType()).Evaluate);
        }
    }
}
