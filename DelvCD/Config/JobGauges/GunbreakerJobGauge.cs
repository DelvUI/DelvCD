using Dalamud.Game.ClientState.JobGauge.Types;
using DelvCD.Helpers;
using DelvCD.Helpers.DataSources;
using DelvCD.Helpers.DataSources.JobDataSources;
using System.Collections.Generic;
using DalamudJobGauges = Dalamud.Game.ClientState.JobGauge.JobGauges;

namespace DelvCD.Config.JobGauges
{
    public class GunbreakerJobGauge : JobGauge
    {
        public GunbreakerJobGauge(string rawData) : base(rawData)
        {
        }

        public override Job Job => Job.GNB;
        private GunbreakerDataSource _dataSource = new();
        public override DataSource DataSource => _dataSource;

        protected override void InitializeConditions()
        {
            _names = new List<string>() { "Cartridges" };
            _types = new List<TriggerConditionType>() { TriggerConditionType.Numeric };
        }

        public override bool IsTriggered(bool preview)
        {
            GNBGauge gauge = Singletons.Get<DalamudJobGauges>().Get<GNBGauge>();

            _dataSource.Cartridges = gauge.Ammo;

            if (preview) { return true; }

            return EvaluateCondition(0, _dataSource.Cartridges);
        }
    }
}

