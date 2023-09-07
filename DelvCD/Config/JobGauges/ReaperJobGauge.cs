using Dalamud.Game.ClientState.JobGauge;
using Dalamud.Game.ClientState.JobGauge.Types;
using DelvCD.Helpers;
using DelvCD.Helpers.DataSources;
using System.Collections.Generic;
using DalamudJobGauges = Dalamud.Game.ClientState.JobGauge.JobGauges;

namespace DelvCD.Config.JobGauges
{
    public class ReaperJobGauge : JobGauge
    {
        public ReaperJobGauge(string rawData) : base(rawData)
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

        public override (bool, DataSource) IsTriggered(bool preview)
        {
            CooldownDataSource data = new CooldownDataSource();
            RPRGauge gauge = Singletons.Get<DalamudJobGauges>().Get<RPRGauge>();

            data.Value = 0;
            data.MaxValue = 100;

            bool triggered =
                EvaluateCondition(0, gauge.Soul) &&
                EvaluateCondition(0, gauge.Shroud) &&
                EvaluateCondition(0, gauge.EnshroudedTimeRemaining) &&
                EvaluateCondition(0, gauge.LemureShroud) &&
                EvaluateCondition(0, gauge.VoidShroud);

            return (triggered, data);
        }
    }
}
