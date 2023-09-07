using Dalamud.Game.ClientState.JobGauge;
using Dalamud.Game.ClientState.JobGauge.Types;
using DelvCD.Helpers;
using DelvCD.Helpers.DataSources;
using System.Collections.Generic;
using DalamudJobGauges = Dalamud.Game.ClientState.JobGauge.JobGauges;

namespace DelvCD.Config.JobGauges
{
    public class DarkKnightJobGauge : JobGauge
    {
        public DarkKnightJobGauge(string rawData) : base(rawData)
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

        public override (bool, DataSource) IsTriggered(bool preview)
        {
            CooldownDataSource data = new CooldownDataSource();
            DRKGauge gauge = Singletons.Get<DalamudJobGauges>().Get<DRKGauge>();

            data.Value = 0;
            data.MaxValue = 100;

            bool triggered =
                EvaluateCondition(0, gauge.Blood) &&
                EvaluateCondition(1, gauge.DarksideTimeRemaining) &&
                EvaluateCondition(2, gauge.ShadowTimeRemaining) &&
                EvaluateCondition(3, gauge.HasDarkArts);

            return (triggered, data);
        }
    }
}
