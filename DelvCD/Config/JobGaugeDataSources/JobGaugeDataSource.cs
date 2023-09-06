using DelvCD.Helpers;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;

namespace DelvCD.Config.JobGaugeDataSources
{
    public abstract class JobGaugeDataSource
    {
        private string _rawData = string.Empty;
        public string RawData
        {
            get { return _rawData; }
            set
            {
                _rawData = value;
                DeserializeData();
            }
        }

        public int ConditionCount => _names.Count;
        public abstract Job Job { get; }

        protected List<int> _enabled;
        protected List<int> _values;
        protected List<int> _operators;

        protected List<string> _names = new List<string>();
        protected List<TriggerConditionType> _types = new List<TriggerConditionType>();
        protected Dictionary<int, string[]> _comboOptions = new Dictionary<int, string[]>();

        public JobGaugeDataSource(string rawData)
        {
            InitializeConditions();

            _enabled = new List<int>(ConditionCount);
            _values = new List<int>(ConditionCount);
            _operators = new List<int>(ConditionCount);

            for (int i = 0; i < ConditionCount; i++)
            {
                _enabled.Add(0);
                _values.Add(0);
                _operators.Add(0);
            }

            RawData = rawData;
        }

        protected abstract void InitializeConditions();

        public abstract bool IsTriggered(bool preview, DataSource data);

        protected bool EvaluateCondition(int index, int value)
        {
            if (index < 0 || index > ConditionCount) { return true; }

            bool active = true;
            if (ConditionEnabledforIndex(index))
            {
                if (_types[index] == TriggerConditionType.Numeric)
                {
                    active = Utils.GetResult(value, (TriggerDataOp)_operators[index], _values[index]);
                }
                else
                {
                    active = value == _values[index];
                }
            }

            return active;
        }

        protected bool EvaluateCondition(int index, bool value)
        {
            return EvaluateCondition(index, value ? 1 : 0);
        }

        private void DeserializeData()
        {
            string[] rawArrays = RawData.Split("|");

            // enabled
            string[] enabled = rawArrays[0].Split(",");
            for (int i = 0; i < enabled.Length; i++)
            {
                if (i >= _enabled.Count) { break; }

                try
                {
                    _enabled[i] = int.Parse(enabled[i], CultureInfo.InvariantCulture);
                }
                catch { }
            }

            // values
            if (rawArrays.Length == 1) { return; }

            string[] values = rawArrays[1].Split(",");
            for (int i = 0; i < values.Length; i++)
            {
                if (i >= _values.Count) { break; }

                try
                {
                    _values[i] = int.Parse(values[i], CultureInfo.InvariantCulture);
                }
                catch { }
            }

            // operators
            if (rawArrays.Length == 2) { return; }

            string[] operators = rawArrays[2].Split(",");
            for (int i = 0; i < operators.Length; i++)
            {
                if (i >= _operators.Count) { break; }

                try
                {
                    _operators[i] = int.Parse(operators[i], CultureInfo.InvariantCulture);
                }
                catch { }
            }
        }

        protected void SerializeData()
        {
            if (ConditionCount <= 0)
            {
                RawData = "";
                return;
            }

            string enabledString = string.Join(",", _enabled.ToArray());
            string valuesString = string.Join(",", _values.ToArray());
            string operatorsString = string.Join(",", _operators.ToArray());

            _rawData = enabledString + "|" + valuesString + "|" + operatorsString;
        }

        public bool ConditionEnabledforIndex(int index)
        {
            if (index >= ConditionCount)
            {
                return false;
            }

            return _enabled[index] != 0;
        }

        public void SetEnabledForIndex(int index, bool enabled)
        {
            if (index >= ConditionCount)
            {
                return;
            }

            _enabled[index] = enabled ? 1 : 0;
            SerializeData();
        }

        public string ConditionNameForIndex(int index)
        {
            if (index >= ConditionCount) {
                return "";
            }

            return _names[index];
        }

        public TriggerConditionType ConditionTypeForIndex(int index)
        {
            if (index >= ConditionCount)
            {
                return TriggerConditionType.Numeric;
            }

            return _types[index];
        }

        public string[] ComboOptionsForIndex(int index)
        {
            if (_comboOptions.TryGetValue(index, out string[]? options) && options != null)
            {
                return options;
            }

            return new string[] { };
        }

        public int ValueForIndex(int index)
        {
            if (index >= ConditionCount)
            {
                return 0;
            }

            return _values[index];
        }

        public void SetValueForIndex(int index, int value)
        {
            if (index >= ConditionCount)
            {
                return;
            }

            _values[index] = value;
            SerializeData();
        }

        public TriggerDataOp OperatorForIndex(int index)
        {
            if (index >= ConditionCount)
            {
                return TriggerDataOp.Equals;
            }

            return (TriggerDataOp)_operators[index];
        }

        public void SetOperatorForIndex(int index, TriggerDataOp op)
        {
            if (index >= ConditionCount)
            {
                return;
            }

            _operators[index] = (int)op;
            SerializeData();
        }

        public static JobGaugeDataSource? CreateDataSource(Type type, string rawData)
        {
            try
            {
                ConstructorInfo? constructor = type.GetConstructor(new Type[] { typeof(string) });
                if (constructor != null)
                {
                    return (JobGaugeDataSource)constructor.Invoke(new object[] { rawData });
                }
            }
            catch { }

            return null;
        }
    }
}
