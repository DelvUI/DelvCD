using Dalamud.Game.ClientState.JobGauge;
using Dalamud.Game.ClientState.JobGauge.Types;
using DelvCD.Helpers;
using DelvCD.Helpers.DataSources;
using DelvCD.Helpers.DataSources.JobDataSources;
using System.Collections.Generic;
using DalamudJobGauges = Dalamud.Game.ClientState.JobGauge.JobGauges;

namespace DelvCD.Config.JobGauges
{
    public class SamuraiJobGauge : JobGauge
    {
        public SamuraiJobGauge(string rawData) : base(rawData)
        {
        }

        public override Job Job => Job.SAM;
        private SamuraiDataSource _dataSource = new();
        public override DataSource DataSource => _dataSource;

        protected override void InitializeConditions()
        {
            _names = new List<string>() {
                "Setsu",
                "Getsu",
                "Ka",
                "Kenki",
                "Meditation Stacks"
            };

            _types = new List<TriggerConditionType>() {
                TriggerConditionType.Boolean,
                TriggerConditionType.Boolean,
                TriggerConditionType.Boolean,
                TriggerConditionType.Numeric,
                TriggerConditionType.Numeric
            };
        }

        public override bool IsTriggered(bool preview)
        {
            SAMGauge gauge = Singletons.Get<DalamudJobGauges>().Get<SAMGauge>();

            _dataSource.Setsu = gauge.HasSetsu;
            _dataSource.Getsu = gauge.HasGetsu;
            _dataSource.Ka = gauge.HasKa;
            _dataSource.Kenki = gauge.Kenki;
            _dataSource.Meditation_Stacks = gauge.MeditationStacks;

            if (preview) { return true; }

            return
                EvaluateCondition(0, _dataSource.Setsu) &&
                EvaluateCondition(1, _dataSource.Getsu) &&
                EvaluateCondition(2, _dataSource.Ka) &&
                EvaluateCondition(3, _dataSource.Kenki) &&
                EvaluateCondition(4, _dataSource.Meditation_Stacks);
        }
    }
}
