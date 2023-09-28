using Dalamud.Game.ClientState.JobGauge.Types;
using Dalamud.Plugin.Services;
using DelvCD.Helpers;
using DelvCD.Helpers.DataSources;
using DelvCD.Helpers.DataSources.JobDataSources;
using System.Collections.Generic;

namespace DelvCD.Config.JobGauges
{
    public class DarkKnightJobGauge : JobGauge
    {
        public DarkKnightJobGauge(string rawData) : base(rawData)
        {
        }

        public override Job Job => Job.DRK;
        private DarkKnightDataSource _dataSource = new();
        public override DataSource DataSource => _dataSource;

        protected override void InitializeConditions()
        {
            _names = new List<string>() {
                "Blood",
                "Darkside Timer",
                "Shadow Timer",
                "Dark Arts"
            };

            _types = new List<TriggerConditionType>() {
                TriggerConditionType.Numeric,
                TriggerConditionType.Numeric,
                TriggerConditionType.Numeric,
                TriggerConditionType.Boolean
            };
        }

        public override bool IsTriggered(bool preview)
        {
            DRKGauge gauge = Singletons.Get<IJobGauges>().Get<DRKGauge>();

            _dataSource.Blood = gauge.Blood;
            _dataSource.Darkside_Timer = gauge.DarksideTimeRemaining / 1000f;
            _dataSource.Shadow_Timer = gauge.ShadowTimeRemaining / 1000f;
            _dataSource.Dark_Arts = gauge.HasDarkArts;

            if (preview) { return true; }

            return
                EvaluateCondition(0, _dataSource.Blood) &&
                EvaluateCondition(1, _dataSource.Darkside_Timer) &&
                EvaluateCondition(2, _dataSource.Shadow_Timer) &&
                EvaluateCondition(3, _dataSource.Dark_Arts);
        }
    }
}
