using Dalamud.Game.ClientState.JobGauge;
using Dalamud.Game.ClientState.JobGauge.Types;
using DelvCD.Helpers;
using System.Collections.Generic;

namespace DelvCD.Config.JobGaugeDataSources
{
    public class NinjaJobGaugeDataSource : JobGaugeDataSource
    {
        public NinjaJobGaugeDataSource(string rawData) : base(rawData)
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

        public override bool IsTriggered(bool preview, DataSource data)
        {
            NINGauge gauge = Singletons.Get<JobGauges>().Get<NINGauge>();

            data.Value = 0;
            data.MaxValue = 100;

            return
                EvaluateCondition(0, gauge.HutonTimer) &&
                EvaluateCondition(1, gauge.Ninki);
        }
    }
}
