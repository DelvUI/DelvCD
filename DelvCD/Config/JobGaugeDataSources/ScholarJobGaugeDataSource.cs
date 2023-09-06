using Dalamud.Game.ClientState.JobGauge;
using Dalamud.Game.ClientState.JobGauge.Types;
using DelvCD.Helpers;
using System.Collections.Generic;

namespace DelvCD.Config.JobGaugeDataSources
{
    public class ScholarJobGaugeDataSource : JobGaugeDataSource
    {
        public ScholarJobGaugeDataSource(string rawData) : base(rawData)
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

        public override bool IsTriggered(bool preview, DataSource data)
        {
            SCHGauge gauge = Singletons.Get<JobGauges>().Get<SCHGauge>();

            data.Value = 0;
            data.MaxValue = 100;

            return
                EvaluateCondition(0, gauge.Aetherflow) &&
                EvaluateCondition(1, gauge.FairyGauge) &&
                EvaluateCondition(2, gauge.SeraphTimer);
        }
    }
}
