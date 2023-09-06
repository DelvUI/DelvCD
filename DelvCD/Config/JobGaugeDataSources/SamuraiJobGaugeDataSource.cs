using Dalamud.Game.ClientState.JobGauge;
using Dalamud.Game.ClientState.JobGauge.Types;
using DelvCD.Helpers;
using System.Collections.Generic;

namespace DelvCD.Config.JobGaugeDataSources
{
    public class SamuraiJobGaugeDataSource : JobGaugeDataSource
    {
        public SamuraiJobGaugeDataSource(string rawData) : base(rawData)
        {
        }

        public override Job Job => Job.SAM;

        protected override void InitializeConditions()
        {
            _names = new List<string>() {
                "Setsu",
                "Getsu",
                "Ka",
                "Kenki",
                "Meditation Stacks"
            };

            _types = new List<TriggerConditionType>() {
                TriggerConditionType.Boolean,
                TriggerConditionType.Boolean,
                TriggerConditionType.Boolean,
                TriggerConditionType.Numeric,
                TriggerConditionType.Numeric
            };
        }

        public override bool IsTriggered(bool preview, DataSource data)
        {
            SAMGauge gauge = Singletons.Get<JobGauges>().Get<SAMGauge>();

            data.Value = 0;
            data.MaxValue = 100;

            return
                EvaluateCondition(0, gauge.HasSetsu) &&
                EvaluateCondition(1, gauge.HasGetsu) &&
                EvaluateCondition(2, gauge.HasKa) &&
                EvaluateCondition(3, gauge.Kenki) &&
                EvaluateCondition(4, gauge.MeditationStacks);
        }
    }
}
