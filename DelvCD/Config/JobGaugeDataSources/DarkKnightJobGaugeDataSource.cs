using Dalamud.Game.ClientState.JobGauge;
using Dalamud.Game.ClientState.JobGauge.Types;
using DelvCD.Helpers;
using System.Collections.Generic;

namespace DelvCD.Config.JobGaugeDataSources
{
    public class DarkKnightJobGaugeDataSource : JobGaugeDataSource
    {
        public DarkKnightJobGaugeDataSource(string rawData) : base(rawData)
        {
        }

        public override Job Job => Job.DRK;

        protected override void InitializeConditions()
        {
            _names = new List<string>() {
                "Blood",
                "Darkside Timer (milliseconds)",
                "Shadow Timer (milliseconds)",
                "Dark Arts"
            };

            _types = new List<TriggerConditionType>() {
                TriggerConditionType.Numeric,
                TriggerConditionType.Numeric,
                TriggerConditionType.Numeric,
                TriggerConditionType.Boolean
            };
        }

        public override bool IsTriggered(bool preview, DataSource data)
        {
            DRKGauge gauge = Singletons.Get<JobGauges>().Get<DRKGauge>();

            data.Value = 0;
            data.MaxValue = 100;

            return
                EvaluateCondition(0, gauge.Blood) &&
                EvaluateCondition(1, gauge.DarksideTimeRemaining) &&
                EvaluateCondition(2, gauge.ShadowTimeRemaining) &&
                EvaluateCondition(3, gauge.HasDarkArts);
        }
    }
}
