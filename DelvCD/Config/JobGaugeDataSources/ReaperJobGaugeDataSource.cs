using Dalamud.Game.ClientState.JobGauge;
using Dalamud.Game.ClientState.JobGauge.Types;
using DelvCD.Helpers;
using System.Collections.Generic;

namespace DelvCD.Config.JobGaugeDataSources
{
    public class ReaperJobGaugeDataSource : JobGaugeDataSource
    {
        public ReaperJobGaugeDataSource(string rawData) : base(rawData)
        {
        }

        public override Job Job => Job.RPR;

        protected override void InitializeConditions()
        {
            _names = new List<string>() {
                "Soul",
                "Shroud",
                "Enshroud Timer (milliseconds)",
                "Lemure Shroud Stacks",
                "Void Shroud Stacks"
            };

            _types = new List<TriggerConditionType>() {
                TriggerConditionType.Numeric,
                TriggerConditionType.Numeric,
                TriggerConditionType.Numeric,
                TriggerConditionType.Numeric,
                TriggerConditionType.Numeric
            };
        }

        public override bool IsTriggered(bool preview, DataSource data)
        {
            RPRGauge gauge = Singletons.Get<JobGauges>().Get<RPRGauge>();

            data.Value = 0;
            data.MaxValue = 100;

            return
                EvaluateCondition(0, gauge.Soul) &&
                EvaluateCondition(0, gauge.Shroud) &&
                EvaluateCondition(0, gauge.EnshroudedTimeRemaining) &&
                EvaluateCondition(0, gauge.LemureShroud) &&
                EvaluateCondition(0, gauge.VoidShroud);
        }
    }
}
