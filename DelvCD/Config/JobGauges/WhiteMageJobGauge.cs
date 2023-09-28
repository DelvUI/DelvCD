using Dalamud.Game.ClientState.JobGauge.Types;
using Dalamud.Plugin.Services;
using DelvCD.Helpers;
using DelvCD.Helpers.DataSources;
using DelvCD.Helpers.DataSources.JobDataSources;
using System.Collections.Generic;

namespace DelvCD.Config.JobGauges
{
    public class WhiteMageJobGauge : JobGauge
    {
        public WhiteMageJobGauge(string rawData) : base(rawData)
        {
        }

        public override Job Job => Job.WHM;
        private WhiteMageDataSource _dataSource = new();
        public override DataSource DataSource => _dataSource;

        protected override void InitializeConditions()
        {
            _names = new List<string>() {
                "Lily Timer",
                "Lily Stacks",
                "Blood Lily Stacks"
            };

            _types = new List<TriggerConditionType>() {
                TriggerConditionType.Numeric,
                TriggerConditionType.Numeric,
                TriggerConditionType.Numeric
            };
        }

        public override bool IsTriggered(bool preview)
        {
            WHMGauge gauge = Singletons.Get<IJobGauges>().Get<WHMGauge>();

            _dataSource.Lily_Timer = gauge.LilyTimer / 1000f;
            _dataSource.Lily_Stacks = gauge.Lily;
            _dataSource.Blood_Lily_Stacks = gauge.BloodLily;

            if (preview) { return true; }

            return
                EvaluateCondition(0, _dataSource.Lily_Timer) &&
                EvaluateCondition(1, _dataSource.Lily_Stacks) &&
                EvaluateCondition(2, _dataSource.Blood_Lily_Stacks);
        }
    }
}
