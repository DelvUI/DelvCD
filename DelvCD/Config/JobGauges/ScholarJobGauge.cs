using Dalamud.Game.ClientState.JobGauge;
using Dalamud.Game.ClientState.JobGauge.Types;
using DelvCD.Helpers;
using DelvCD.Helpers.DataSources;
using System.Collections.Generic;
using DalamudJobGauges = Dalamud.Game.ClientState.JobGauge.JobGauges;

namespace DelvCD.Config.JobGauges
{
    public class ScholarJobGauge : JobGauge
    {
        public ScholarJobGauge(string rawData) : base(rawData)
        {
        }

        public override Job Job => Job.SCH;

        protected override void InitializeConditions()
        {
            _names = new List<string>() {
                "Aetherflow Stacks",
                "Faerie",
                "Seraph Timer (milliseconds)"
            };

            _types = new List<TriggerConditionType>() {
                TriggerConditionType.Numeric,
                TriggerConditionType.Numeric,
                TriggerConditionType.Numeric
            };
        }

        public override (bool, DataSource) IsTriggered(bool preview)
        {
            CooldownDataSource data = new CooldownDataSource();
            SCHGauge gauge = Singletons.Get<DalamudJobGauges>().Get<SCHGauge>();

            data.Value = 0;
            data.MaxValue = 100;

            bool triggered =
                EvaluateCondition(0, gauge.Aetherflow) &&
                EvaluateCondition(1, gauge.FairyGauge) &&
                EvaluateCondition(2, gauge.SeraphTimer);

            return (triggered, data);
        }
    }
}
