using Dalamud.Game.ClientState.JobGauge.Types;
using DelvCD.Helpers;
using DelvCD.Helpers.DataSources;
using DelvCD.Helpers.DataSources.JobDataSources;
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
        private WarriorDataSource _dataSource = new();
        public override DataSource DataSource => _dataSource;

        protected override void InitializeConditions()
        {
            _names = new List<string>() { "Wrath" };
            _types = new List<TriggerConditionType>() { TriggerConditionType.Numeric };
        }

        public override bool IsTriggered(bool preview)
        {
            WARGauge gauge = Singletons.Get<DalamudJobGauges>().Get<WARGauge>();

            _dataSource.Wrath = gauge.BeastGauge;

            if (preview) { return true; }

            return EvaluateCondition(0, _dataSource.Wrath);
        }
    }
}
