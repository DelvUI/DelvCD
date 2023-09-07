using Dalamud.Game.ClientState.JobGauge;
using Dalamud.Game.ClientState.JobGauge.Types;
using DelvCD.Helpers;
using DelvCD.Helpers.DataSources;
using DelvCD.Helpers.DataSources.JobDataSources;
using System.Collections.Generic;
using DalamudJobGauges = Dalamud.Game.ClientState.JobGauge.JobGauges;

namespace DelvCD.Config.JobGauges
{
    public class DragoonJobGauge : JobGauge
    {
        public DragoonJobGauge(string rawData) : base(rawData)
        {
        }

        public override Job Job => Job.DRG;
        private DragoonDataSource _dataSource = new();
        public override DataSource DataSource => _dataSource;

        protected override void InitializeConditions()
        {
            _names = new List<string>() {
                "Life of the Dragon",
                "Life of the Dragon Timer (milliseconds)",
                "First Brood's Gaze Stacks",
                "Firstminds' Focus Stacks"
            };

            _types = new List<TriggerConditionType>() {
                TriggerConditionType.Boolean,
                TriggerConditionType.Numeric,
                TriggerConditionType.Numeric,
                TriggerConditionType.Numeric
            };
        }

        public override bool IsTriggered(bool preview)
        {
            DRGGauge gauge = Singletons.Get<DalamudJobGauges>().Get<DRGGauge>();

            _dataSource.Life_Of_The_Dragon = gauge.IsLOTDActive;
            _dataSource.Life_Of_The_Dragon_Timer = gauge.LOTDTimer / 1000f;
            _dataSource.First_Broods_Gaze_Stacks = gauge.EyeCount;
            _dataSource.Firstminds_Focus_Stacks = gauge.FirstmindsFocusCount;

            if (preview) { return true; }

            return
                EvaluateCondition(0, _dataSource.Life_Of_The_Dragon) &&
                EvaluateCondition(1, _dataSource.Life_Of_The_Dragon_Timer) &&
                EvaluateCondition(2, _dataSource.First_Broods_Gaze_Stacks) &&
                EvaluateCondition(3, _dataSource.Firstminds_Focus_Stacks);
        }
    }
}
