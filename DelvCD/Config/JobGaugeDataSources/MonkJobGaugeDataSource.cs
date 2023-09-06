using Dalamud.Game.ClientState.JobGauge;
using Dalamud.Game.ClientState.JobGauge.Enums;
using Dalamud.Game.ClientState.JobGauge.Types;
using DelvCD.Helpers;
using System.Collections.Generic;

namespace DelvCD.Config.JobGaugeDataSources
{
    public class MonkJobGaugeDataSource : JobGaugeDataSource
    {
        public MonkJobGaugeDataSource(string rawData) : base(rawData)
        {
        }

        public override Job Job => Job.MNK;

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

        public override bool IsTriggered(bool preview, DataSource data)
        {
            MNKGauge gauge = Singletons.Get<JobGauges>().Get<MNKGauge>();

            data.Value = 0;
            data.MaxValue = 100;

            return
                EvaluateCondition(0, gauge.Chakra) &&
                EvaluateMastersGaugeChakraCondition(gauge, 0) &&
                EvaluateMastersGaugeChakraCondition(gauge, 1) &&
                EvaluateMastersGaugeChakraCondition(gauge, 2) &&
                EvaluateCondition(4, (gauge.Nadi & Nadi.SOLAR) != 0) &&
                EvaluateCondition(5, (gauge.Nadi & Nadi.LUNAR) != 0);
        }

        private bool EvaluateMastersGaugeChakraCondition(MNKGauge gauge, int chakra)
        {
            if (!ConditionEnabledforIndex(1 + chakra)) { return true; }

            return (int)gauge.BeastChakra[chakra] == _values[1 + chakra];
        }
    }
}
