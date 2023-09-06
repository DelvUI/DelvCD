using Dalamud.Game.ClientState.JobGauge;
using Dalamud.Game.ClientState.JobGauge.Types;
using DelvCD.Helpers;
using System.Collections.Generic;

namespace DelvCD.Config.JobGaugeDataSources
{
    public class PaladinJobGaugeDataSource : JobGaugeDataSource
    {
        public PaladinJobGaugeDataSource(string rawData) : base(rawData)
        {
        }

        public override Job Job => Job.PLD;

        protected override void InitializeConditions()
        {
            _names = new List<string>() { "Oath Gauge" };
            _types = new List<TriggerConditionType>() { TriggerConditionType.Numeric };
        }

        public override bool IsTriggered(bool preview, DataSource data)
        {
            PLDGauge gauge = Singletons.Get<JobGauges>().Get<PLDGauge>();

            data.Value = gauge.OathGauge;
            data.MaxValue = 100;

            return EvaluateCondition(0, gauge.OathGauge);
        }
    }
}
