using Dalamud.Game.ClientState.JobGauge.Types;
using Dalamud.Plugin.Services;
using DelvCD.Helpers;
using DelvCD.Helpers.DataSources;
using DelvCD.Helpers.DataSources.JobDataSources;
using System.Collections.Generic;

namespace DelvCD.Config.JobGauges
{
    public class SageJobGauge : JobGauge
    {
        public SageJobGauge(string rawData) : base(rawData)
        {
        }

        public override Job Job => Job.SGE;
        private SageDataSource _dataSource = new();
        public override DataSource DataSource => _dataSource;

        protected override void InitializeConditions()
        {
            _names = new List<string>() {
                "Eukrasia",
                "Addersgall Timer",
                "Addersgall Stacks",
                "Addersting Stacks"
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
            SGEGauge gauge = Singletons.Get<IJobGauges>().Get<SGEGauge>();

            _dataSource.Eukrasia = gauge.Eukrasia;
            _dataSource.Addersgall_Timer = gauge.AddersgallTimer / 1000f;
            _dataSource.Addersgall_Stacks = gauge.Addersgall;
            _dataSource.Addersting_Stacks = gauge.Addersting;

            if (preview) { return true; }

            return
                EvaluateCondition(0, _dataSource.Eukrasia) &&
                EvaluateCondition(1, _dataSource.Addersgall_Timer) &&
                EvaluateCondition(2, _dataSource.Addersgall_Stacks) &&
                EvaluateCondition(3, _dataSource.Addersting_Stacks);
        }
    }
}
