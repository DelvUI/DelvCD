using Dalamud.Game.ClientState.JobGauge.Types;
using DelvCD.Helpers;
using DelvCD.Helpers.DataSources;
using DelvCD.Helpers.DataSources.JobDataSources;
using System.Collections.Generic;
using DalamudJobGauges = Dalamud.Game.ClientState.JobGauge.JobGauges;

namespace DelvCD.Config.JobGauges
{
    public class NinjaJobGauge : JobGauge
    {
        public NinjaJobGauge(string rawData) : base(rawData)
        {
        }

        public override Job Job => Job.NIN;
        private NinjaDataSource _dataSource = new();
        public override DataSource DataSource => _dataSource;

        protected override void InitializeConditions()
        {
            _names = new List<string>() {
                "Huton Timer (milliseconds)",
                "Ninki"
            };

            _types = new List<TriggerConditionType>() {
                TriggerConditionType.Numeric,
                TriggerConditionType.Numeric
            };
        }

        public override bool IsTriggered(bool preview)
        {
            NINGauge gauge = Singletons.Get<DalamudJobGauges>().Get<NINGauge>();

            _dataSource.Huton_Timer = gauge.HutonTimer / 1000f;
            _dataSource.Ninki = gauge.Ninki;

            if (preview) { return true; }

            return
                EvaluateCondition(0, _dataSource.Huton_Timer) &&
                EvaluateCondition(1, _dataSource.Ninki);
        }
    }
}
