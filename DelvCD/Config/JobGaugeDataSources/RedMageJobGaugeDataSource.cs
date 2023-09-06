using Dalamud.Game.ClientState.JobGauge;
using Dalamud.Game.ClientState.JobGauge.Types;
using DelvCD.Helpers;
using System.Collections.Generic;

namespace DelvCD.Config.JobGaugeDataSources
{
    public class RedMageJobGaugeDataSource : JobGaugeDataSource
    {
        public RedMageJobGaugeDataSource(string rawData) : base(rawData)
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

        public override bool IsTriggered(bool preview, DataSource data)
        {
            RDMGauge gauge = Singletons.Get<JobGauges>().Get<RDMGauge>();

            data.Value = 0;
            data.MaxValue = 100;

            return
                EvaluateCondition(0, gauge.WhiteMana) &&
                EvaluateCondition(1, gauge.BlackMana) &&
                EvaluateCondition(2, gauge.ManaStacks);
        }
    }
}
