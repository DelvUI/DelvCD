using Dalamud.Game.ClientState.JobGauge;
using Dalamud.Game.ClientState.JobGauge.Enums;
using Dalamud.Game.ClientState.JobGauge.Types;
using DelvCD.Helpers;
using System.Collections.Generic;

namespace DelvCD.Config.JobGaugeDataSources
{
    public class WarriorJobGaugeDataSource : JobGaugeDataSource
    {
        public WarriorJobGaugeDataSource(string rawData) : base(rawData)
        {
        }

        public override Job Job => Job.WAR;

        protected override void InitializeConditions()
        {
            _names = new List<string>() { "Wrath" };
            _types = new List<TriggerConditionType>() { TriggerConditionType.Numeric };
        }

        public override bool IsTriggered(bool preview, DataSource data)
        {
            WARGauge gauge = Singletons.Get<JobGauges>().Get<WARGauge>();

            data.Value = 0;
            data.MaxValue = 100;

            return EvaluateCondition(0, gauge.BeastGauge);
        }
    }
}
