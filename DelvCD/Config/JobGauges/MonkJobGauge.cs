using Dalamud.Game.ClientState.JobGauge.Enums;
using Dalamud.Game.ClientState.JobGauge.Types;
using Dalamud.Plugin.Services;
using DelvCD.Helpers;
using DelvCD.Helpers.DataSources;
using DelvCD.Helpers.DataSources.JobDataSources;
using System.Collections.Generic;

namespace DelvCD.Config.JobGauges
{
    public class MonkJobGauge : JobGauge
    {
        public MonkJobGauge(string rawData) : base(rawData)
        {
        }

        public override Job Job => Job.MNK;
        private MonkDataSource _dataSource = new();
        public override DataSource DataSource => _dataSource;

        protected override void InitializeConditions()
        {
            _names = new List<string>() {
                "Chakra Stacks",
                "Master's Gauge Chakra #1",
                "Master's Gauge Chakra #2",
                "Master's Gauge Chakra #3",
                "Solar Nadi",
                "Lunar Nadi"
            };

            _types = new List<TriggerConditionType>() {
                TriggerConditionType.Numeric,
                TriggerConditionType.Combo,
                TriggerConditionType.Combo,
                TriggerConditionType.Combo,
                TriggerConditionType.Boolean,
                TriggerConditionType.Boolean
            };

            string[] chakras = new string[] { "None", "Coeurl", "Raptor", "Opo-opo" };
            _comboOptions = new Dictionary<int, string[]>()
            {
                [1] = chakras,
                [2] = chakras,
                [3] = chakras,
            };
        }

        public override bool IsTriggered(bool preview)
        {
            MNKGauge gauge = Singletons.Get<IJobGauges>().Get<MNKGauge>();

            _dataSource.Chakra_Stacks = gauge.Chakra;
            _dataSource.Masters_Gauge_Chakra_1 = _comboOptions[1][_values[1]];
            _dataSource.Masters_Gauge_Chakra_2 = _comboOptions[2][_values[2]];
            _dataSource.Masters_Gauge_Chakra_3 = _comboOptions[3][_values[3]];
            _dataSource.Solar_Nadi = (gauge.Nadi & Nadi.SOLAR) != 0;
            _dataSource.Lunar_Nadi = (gauge.Nadi & Nadi.LUNAR) != 0;

            if (preview) { return true; }

            return
                EvaluateCondition(0, _dataSource.Chakra_Stacks) &&
                EvaluateMastersGaugeChakraCondition(gauge, 0) &&
                EvaluateMastersGaugeChakraCondition(gauge, 1) &&
                EvaluateMastersGaugeChakraCondition(gauge, 2) &&
                EvaluateCondition(4, _dataSource.Solar_Nadi) &&
                EvaluateCondition(5, _dataSource.Lunar_Nadi);
        }

        private bool EvaluateMastersGaugeChakraCondition(MNKGauge gauge, int chakra)
        {
            if (!ConditionEnabledforIndex(1 + chakra)) { return true; }

            return (int)gauge.BeastChakra[chakra] == _values[1 + chakra];
        }
    }
}
