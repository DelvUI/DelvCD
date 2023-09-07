using Dalamud.Game.ClientState.JobGauge.Types;
using DelvCD.Helpers;
using DelvCD.Helpers.DataSources;
using System.Collections.Generic;
using DalamudJobGauges = Dalamud.Game.ClientState.JobGauge.JobGauges;

namespace DelvCD.Config.JobGauges
{
    public class WhiteMageJobGauge : JobGauge
    {
        public WhiteMageJobGauge(string rawData) : base(rawData)
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

        public override (bool, DataSource) IsTriggered(bool preview)
        {
            CooldownDataSource data = new CooldownDataSource();
            WHMGauge gauge = Singletons.Get<DalamudJobGauges>().Get<WHMGauge>();

            data.Value = 0;
            data.MaxValue = 100;

            bool triggered =
                EvaluateCondition(0, gauge.LilyTimer) &&
                EvaluateCondition(1, gauge.Lily) &&
                EvaluateCondition(2, gauge.BloodLily);

            return (triggered, data);
        }
    }
}
