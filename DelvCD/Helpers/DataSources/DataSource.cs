using System.Collections.Generic;

namespace DelvCD.Helpers.DataSources
{
    public abstract class DataSource
    {
        public static string GetDisplayName() => "";

        public uint Id;
        public uint Icon;

        public float PreviewValue;
        public float PreviewMaxValue => 100;
        public float Value;

        // condition fields
        protected List<string> _conditionFieldNames = new();
        public List<string> ConditionFieldNames => _conditionFieldNames;

        public abstract float GetConditionValue(int index);

        // progress fields
        protected List<string> _progressFieldNames = new();
        public List<string> ProgressFieldNames => _progressFieldNames;

        public abstract float GetProgressValue(int index);
        public abstract float GetMaxValue(int index);
    }
}
