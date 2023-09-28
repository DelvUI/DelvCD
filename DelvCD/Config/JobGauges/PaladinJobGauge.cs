using Dalamud.Game.ClientState.JobGauge.Types;
using Dalamud.Plugin.Services;
using DelvCD.Helpers;
using DelvCD.Helpers.DataSources;
using DelvCD.Helpers.DataSources.JobDataSources;
using System.Collections.Generic;

namespace DelvCD.Config.JobGauges
{
    public class PaladinJobGauge : JobGauge
    {
        public PaladinJobGauge(string rawData) : base(rawData)
        {
        }

        public override Job Job => Job.PLD;
        private PaladinDataSource _dataSource = new();
        public override DataSource DataSource => _dataSource;

        protected override void InitializeConditions()
        {
            _names = new List<string>() { "Oath" };
            _types = new List<TriggerConditionType>() { TriggerConditionType.Numeric };
        }

        public override bool IsTriggered(bool preview)
        {
            PLDGauge gauge = Singletons.Get<IJobGauges>().Get<PLDGauge>();

            _dataSource.Oath = gauge.OathGauge;

            if (preview) { return true; }

            return EvaluateCondition(0, _dataSource.Oath);
        }
    }
}
