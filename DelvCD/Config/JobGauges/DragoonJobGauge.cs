using Dalamud.Game.ClientState.JobGauge;
using Dalamud.Game.ClientState.JobGauge.Types;
using DelvCD.Helpers;
using DelvCD.Helpers.DataSources;
using System.Collections.Generic;
using DalamudJobGauges = Dalamud.Game.ClientState.JobGauge.JobGauges;

namespace DelvCD.Config.JobGauges
{
    public class DragoonJobGauge : JobGauge
    {
        public DragoonJobGauge(string rawData) : base(rawData)
        {
        }

        public override Job Job => Job.DRG;

        protected override void InitializeConditions()
        {
            _names = new List<string>() {
                "Life of the Dragon",
                "Life of the Dragon Timer (milliseconds)",
                "First Brood's Gaze Stacks",
                "Firstminds' Focus Stacks"
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
            DRGGauge gauge = Singletons.Get<DalamudJobGauges>().Get<DRGGauge>();

            data.Value = 0;
            data.MaxValue = 100;

            bool triggered =
                EvaluateCondition(0, gauge.IsLOTDActive) &&
                EvaluateCondition(1, gauge.LOTDTimer) &&
                EvaluateCondition(2, gauge.EyeCount) &&
                EvaluateCondition(3, gauge.FirstmindsFocusCount);

            return (triggered, data);
        }
    }
}
