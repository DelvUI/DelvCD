using Dalamud.Game.ClientState.JobGauge;
using Dalamud.Game.ClientState.JobGauge.Types;
using DelvCD.Helpers;
using System.Collections.Generic;

namespace DelvCD.Config.JobGaugeDataSources
{
    public class GunbreakerJobGaugeDataSource : JobGaugeDataSource
    {
        public GunbreakerJobGaugeDataSource(string rawData) : base(rawData)
        {
        }

        public override Job Job => Job.GNB;

        protected override void InitializeConditions()
        {
            _names = new List<string>() { "Cartridges" };
            _types = new List<TriggerConditionType>() { TriggerConditionType.Numeric };
        }

        public override bool IsTriggered(bool preview, DataSource data)
        {
            GNBGauge gauge = Singletons.Get<JobGauges>().Get<GNBGauge>();

            data.Value = 0;
            data.MaxValue = 100;

            return EvaluateCondition(0, gauge.Ammo);
        }
    }
}

