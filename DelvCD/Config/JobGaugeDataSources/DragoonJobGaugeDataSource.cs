using Dalamud.Game.ClientState.JobGauge;
using Dalamud.Game.ClientState.JobGauge.Types;
using DelvCD.Helpers;
using System.Collections.Generic;

namespace DelvCD.Config.JobGaugeDataSources
{
    public class DragoonJobGaugeDataSource : JobGaugeDataSource
    {
        public DragoonJobGaugeDataSource(string rawData) : base(rawData)
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

        public override bool IsTriggered(bool preview, DataSource data)
        {
            DRGGauge gauge = Singletons.Get<JobGauges>().Get<DRGGauge>();

            data.Value = 0;
            data.MaxValue = 100;

            return
                EvaluateCondition(0, gauge.IsLOTDActive) &&
                EvaluateCondition(1, gauge.LOTDTimer) &&
                EvaluateCondition(2, gauge.EyeCount) &&
                EvaluateCondition(3, gauge.FirstmindsFocusCount);
        }
    }
}
