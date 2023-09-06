using Dalamud.Game.ClientState.JobGauge;
using Dalamud.Game.ClientState.JobGauge.Types;
using DelvCD.Helpers;
using System.Collections.Generic;

namespace DelvCD.Config.JobGaugeDataSources
{
    public class SageJobGaugeDataSource : JobGaugeDataSource
    {
        public SageJobGaugeDataSource(string rawData) : base(rawData)
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

        public override bool IsTriggered(bool preview, DataSource data)
        {
            SGEGauge gauge = Singletons.Get<JobGauges>().Get<SGEGauge>();

            data.Value = 0;
            data.MaxValue = 100;

            return
                EvaluateCondition(0, gauge.Eukrasia) &&
                EvaluateCondition(1, gauge.AddersgallTimer) &&
                EvaluateCondition(2, gauge.Addersgall) &&
                EvaluateCondition(3, gauge.Addersting);
        }
    }
}
