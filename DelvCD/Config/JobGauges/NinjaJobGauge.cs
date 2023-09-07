using Dalamud.Game.ClientState.JobGauge;
using Dalamud.Game.ClientState.JobGauge.Types;
using DelvCD.Helpers;
using DelvCD.Helpers.DataSources;
using System.Collections.Generic;
using DalamudJobGauges = Dalamud.Game.ClientState.JobGauge.JobGauges;

namespace DelvCD.Config.JobGauges
{
    public class NinjaJobGauge : JobGauge
    {
        public NinjaJobGauge(string rawData) : base(rawData)
        {
        }

        public override Job Job => Job.NIN;

        protected override void InitializeConditions()
        {
            _names = new List<string>() {
                "Huton Timer (milliseconds)",
                "Ninki"
            };

            _types = new List<TriggerConditionType>() {
                TriggerConditionType.Numeric,
                TriggerConditionType.Numeric
            };
        }

        public override (bool, DataSource) IsTriggered(bool preview)
        {
            CooldownDataSource data = new CooldownDataSource();
            NINGauge gauge = Singletons.Get<DalamudJobGauges>().Get<NINGauge>();

            data.Value = 0;
            data.MaxValue = 100;

            bool triggered =
                EvaluateCondition(0, gauge.HutonTimer) &&
                EvaluateCondition(1, gauge.Ninki);

            return (triggered, data);
        }
    }
}
