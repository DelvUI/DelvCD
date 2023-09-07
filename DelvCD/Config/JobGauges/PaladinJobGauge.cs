using Dalamud.Game.ClientState.JobGauge;
using Dalamud.Game.ClientState.JobGauge.Types;
using DelvCD.Helpers;
using DelvCD.Helpers.DataSources;
using System.Collections.Generic;
using DalamudJobGauges = Dalamud.Game.ClientState.JobGauge.JobGauges;

namespace DelvCD.Config.JobGauges
{
    public class PaladinJobGauge : JobGauge
    {
        public PaladinJobGauge(string rawData) : base(rawData)
        {
        }

        public override Job Job => Job.PLD;

        protected override void InitializeConditions()
        {
            _names = new List<string>() { "Oath Gauge" };
            _types = new List<TriggerConditionType>() { TriggerConditionType.Numeric };
        }

        public override (bool, DataSource) IsTriggered(bool preview)
        {
            CooldownDataSource data = new CooldownDataSource();
            PLDGauge gauge = Singletons.Get<DalamudJobGauges>().Get<PLDGauge>();

            data.Value = gauge.OathGauge;
            data.MaxValue = 100;

            bool triggered = EvaluateCondition(0, gauge.OathGauge);

            return (triggered, data);
        }
    }
}
