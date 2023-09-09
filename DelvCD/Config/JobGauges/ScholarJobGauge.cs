using Dalamud.Game.ClientState.JobGauge.Types;
using DelvCD.Helpers;
using DelvCD.Helpers.DataSources;
using DelvCD.Helpers.DataSources.JobDataSources;
using System.Collections.Generic;
using DalamudJobGauges = Dalamud.Game.ClientState.JobGauge.JobGauges;

namespace DelvCD.Config.JobGauges
{
    public class ScholarJobGauge : JobGauge
    {
        public ScholarJobGauge(string rawData) : base(rawData)
        {
        }

        public override Job Job => Job.SCH;
        private ScholarDataSource _dataSource = new();
        public override DataSource DataSource => _dataSource;

        protected override void InitializeConditions()
        {
            _names = new List<string>() {
                "Aetherflow Stacks",
                "Faerie",
                "Seraph Timer"
            };

            _types = new List<TriggerConditionType>() {
                TriggerConditionType.Numeric,
                TriggerConditionType.Numeric,
                TriggerConditionType.Numeric
            };
        }

        public override bool IsTriggered(bool preview)
        {
            SCHGauge gauge = Singletons.Get<DalamudJobGauges>().Get<SCHGauge>();

            _dataSource.Aetherflow_Stacks = gauge.Aetherflow;
            _dataSource.Fairie = gauge.FairyGauge;
            _dataSource.Seraph_Timer = gauge.SeraphTimer / 1000f;

            if (preview) { return true; }

            return
                EvaluateCondition(0, _dataSource.Aetherflow_Stacks) &&
                EvaluateCondition(1, _dataSource.Fairie) &&
                EvaluateCondition(2, _dataSource.Seraph_Timer);
        }
    }
}
