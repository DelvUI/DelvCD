using Dalamud.Game.ClientState.JobGauge.Types;
using Dalamud.Plugin.Services;
using DelvCD.Helpers;
using DelvCD.Helpers.DataSources;
using DelvCD.Helpers.DataSources.JobDataSources;
using System.Collections.Generic;

namespace DelvCD.Config.JobGauges
{
    public class ReaperJobGauge : JobGauge
    {
        public ReaperJobGauge(string rawData) : base(rawData)
        {
        }

        public override Job Job => Job.RPR;
        private ReaperDataSource _dataSource = new();
        public override DataSource DataSource => _dataSource;

        protected override void InitializeConditions()
        {
            _names = new List<string>() {
                "Soul",
                "Shroud",
                "Enshroud Timer",
                "Lemure Shroud Stacks",
                "Void Shroud Stacks"
            };

            _types = new List<TriggerConditionType>() {
                TriggerConditionType.Numeric,
                TriggerConditionType.Numeric,
                TriggerConditionType.Numeric,
                TriggerConditionType.Numeric,
                TriggerConditionType.Numeric
            };
        }

        public override bool IsTriggered(bool preview)
        {
            RPRGauge gauge = Singletons.Get<IJobGauges>().Get<RPRGauge>();

            _dataSource.Soul = gauge.Soul;
            _dataSource.Shroud = gauge.Shroud;
            _dataSource.Enshroud_Timer = gauge.EnshroudedTimeRemaining / 1000f;
            _dataSource.Lemure_Shroud_Stacks = gauge.LemureShroud;
            _dataSource.Void_Shroud_Stacks = gauge.VoidShroud;

            if (preview) { return true; }

            return
                EvaluateCondition(0, _dataSource.Soul) &&
                EvaluateCondition(1, _dataSource.Shroud) &&
                EvaluateCondition(2, _dataSource.Enshroud_Timer) &&
                EvaluateCondition(3, _dataSource.Lemure_Shroud_Stacks) &&
                EvaluateCondition(4, _dataSource.Void_Shroud_Stacks);
        }
    }
}
