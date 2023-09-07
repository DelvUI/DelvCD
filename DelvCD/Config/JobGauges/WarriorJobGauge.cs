using Dalamud.Game.ClientState.JobGauge;
using Dalamud.Game.ClientState.JobGauge.Enums;
using Dalamud.Game.ClientState.JobGauge.Types;
using DelvCD.Helpers;
using DelvCD.Helpers.DataSources;
using System.Collections.Generic;
using DalamudJobGauges = Dalamud.Game.ClientState.JobGauge.JobGauges;

namespace DelvCD.Config.JobGauges
{
    public class WarriorJobGauge : JobGauge
    {
        public WarriorJobGauge(string rawData) : base(rawData)
        {
        }

        public override Job Job => Job.WAR;

        protected override void InitializeConditions()
        {
            _names = new List<string>() { "Wrath" };
            _types = new List<TriggerConditionType>() { TriggerConditionType.Numeric };
        }

        public override (bool, DataSource) IsTriggered(bool preview)
        {
            CooldownDataSource data = new CooldownDataSource();
            WARGauge gauge = Singletons.Get<DalamudJobGauges>().Get<WARGauge>();

            data.Value = 0;
            data.MaxValue = 100;

            bool triggered = EvaluateCondition(0, gauge.BeastGauge);

            return (triggered, data);
        }
    }
}
