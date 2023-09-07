using Dalamud.Game.ClientState.JobGauge;
using Dalamud.Game.ClientState.JobGauge.Types;
using DelvCD.Helpers;
using DelvCD.Helpers.DataSources;
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

        protected override void InitializeConditions()
        {
            _names = new List<string>() { "Cartridges" };
            _types = new List<TriggerConditionType>() { TriggerConditionType.Numeric };
        }

        public override (bool, DataSource) IsTriggered(bool preview)
        {
            CooldownDataSource data = new CooldownDataSource();
            GNBGauge gauge = Singletons.Get<DalamudJobGauges>().Get<GNBGauge>();

            data.Value = 0;
            data.MaxValue = 100;

            bool triggered = EvaluateCondition(0, gauge.Ammo);

            return (triggered, data);
        }
    }
}

