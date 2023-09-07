using Dalamud.Game.ClientState.JobGauge;
using Dalamud.Game.ClientState.JobGauge.Types;
using DelvCD.Helpers;
using DelvCD.Helpers.DataSources;
using System.Collections.Generic;
using DalamudJobGauges = Dalamud.Game.ClientState.JobGauge.JobGauges;

namespace DelvCD.Config.JobGauges
{
    public class RedMageJobGauge : JobGauge
    {
        public RedMageJobGauge(string rawData) : base(rawData)
        {
        }

        public override Job Job => Job.RDM;

        protected override void InitializeConditions()
        {
            _names = new List<string>() {
                "White Mana",
                "Black Mana",
                "Mana Stacks"
            };

            _types = new List<TriggerConditionType>() {
                TriggerConditionType.Numeric,
                TriggerConditionType.Numeric,
                TriggerConditionType.Numeric
            };
        }

        public override (bool, DataSource) IsTriggered(bool preview)
        {
            CooldownDataSource data = new CooldownDataSource();
            RDMGauge gauge = Singletons.Get<DalamudJobGauges>().Get<RDMGauge>();

            data.Value = 0;
            data.MaxValue = 100;

            bool triggered =
                EvaluateCondition(0, gauge.WhiteMana) &&
                EvaluateCondition(1, gauge.BlackMana) &&
                EvaluateCondition(2, gauge.ManaStacks);

            return (triggered, data);
        }
    }
}
