using Dalamud.Game.ClientState.JobGauge;
using Dalamud.Game.ClientState.JobGauge.Types;
using DelvCD.Helpers;
using System.Collections.Generic;

namespace DelvCD.Config.JobGaugeDataSources
{
    public class WhiteMageJobGaugeDataSource : JobGaugeDataSource
    {
        public WhiteMageJobGaugeDataSource(string rawData) : base(rawData)
        {
        }

        public override Job Job => Job.WHM;

        protected override void InitializeConditions()
        {
            _names = new List<string>() {
                "Lily Timer (milliseconds)",
                "Lily Stacks",
                "Blood Lily Stacks"
            };

            _types = new List<TriggerConditionType>() {
                TriggerConditionType.Numeric,
                TriggerConditionType.Numeric,
                TriggerConditionType.Numeric
            };
        }

        public override bool IsTriggered(bool preview, DataSource data)
        {
            WHMGauge gauge = Singletons.Get<JobGauges>().Get<WHMGauge>();

            data.Value = 0;
            data.MaxValue = 100;

            return
                EvaluateCondition(0, gauge.LilyTimer) &&
                EvaluateCondition(1, gauge.Lily) &&
                EvaluateCondition(2, gauge.BloodLily);
        }
    }
}
