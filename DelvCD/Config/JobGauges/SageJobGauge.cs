using Dalamud.Game.ClientState.JobGauge;
using Dalamud.Game.ClientState.JobGauge.Types;
using DelvCD.Helpers;
using DelvCD.Helpers.DataSources;
using System.Collections.Generic;
using DalamudJobGauges = Dalamud.Game.ClientState.JobGauge.JobGauges;

namespace DelvCD.Config.JobGauges
{
    public class SageJobGauge : JobGauge
    {
        public SageJobGauge(string rawData) : base(rawData)
        {
        }

        public override Job Job => Job.SGE;

        protected override void InitializeConditions()
        {
            _names = new List<string>() {
                "Eukrasia",
                "Addersgall Timer (milliseconds)",
                "Addersgall Stacks",
                "Addersting Stacks"
            };

            _types = new List<TriggerConditionType>() {
                TriggerConditionType.Boolean,
                TriggerConditionType.Numeric,
                TriggerConditionType.Numeric,
                TriggerConditionType.Numeric
            };
        }

        public override (bool, DataSource) IsTriggered(bool preview)
        {
            CooldownDataSource data = new CooldownDataSource();
            SGEGauge gauge = Singletons.Get<DalamudJobGauges>().Get<SGEGauge>();

            data.Value = 0;
            data.MaxValue = 100;

            bool triggered =
                EvaluateCondition(0, gauge.Eukrasia) &&
                EvaluateCondition(1, gauge.AddersgallTimer) &&
                EvaluateCondition(2, gauge.Addersgall) &&
                EvaluateCondition(3, gauge.Addersting);

            return (triggered, data);
        }
    }
}
