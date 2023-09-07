using Dalamud.Game.ClientState.JobGauge;
using Dalamud.Game.ClientState.JobGauge.Enums;
using Dalamud.Game.ClientState.JobGauge.Types;
using DelvCD.Helpers;
using DelvCD.Helpers.DataSources;
using System.Collections.Generic;
using DalamudJobGauges = Dalamud.Game.ClientState.JobGauge.JobGauges;

namespace DelvCD.Config.JobGauges
{
    public class MonkJobGauge : JobGauge
    {
        public MonkJobGauge(string rawData) : base(rawData)
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

        public override (bool, DataSource) IsTriggered(bool preview)
        {
            CooldownDataSource data = new CooldownDataSource();
            MNKGauge gauge = Singletons.Get<DalamudJobGauges>().Get<MNKGauge>();

            data.Value = 0;
            data.MaxValue = 100;

            bool triggered =
                EvaluateCondition(0, gauge.Chakra) &&
                EvaluateMastersGaugeChakraCondition(gauge, 0) &&
                EvaluateMastersGaugeChakraCondition(gauge, 1) &&
                EvaluateMastersGaugeChakraCondition(gauge, 2) &&
                EvaluateCondition(4, (gauge.Nadi & Nadi.SOLAR) != 0) &&
                EvaluateCondition(5, (gauge.Nadi & Nadi.LUNAR) != 0);

            return (triggered, data);
        }

        private bool EvaluateMastersGaugeChakraCondition(MNKGauge gauge, int chakra)
        {
            if (!ConditionEnabledforIndex(1 + chakra)) { return true; }

            return (int)gauge.BeastChakra[chakra] == _values[1 + chakra];
        }
    }
}
