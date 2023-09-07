using Dalamud.Game.ClientState.JobGauge;
using Dalamud.Game.ClientState.JobGauge.Types;
using DelvCD.Helpers;
using DelvCD.Helpers.DataSources;
using System.Collections.Generic;
using DalamudJobGauges = Dalamud.Game.ClientState.JobGauge.JobGauges;

namespace DelvCD.Config.JobGauges
{
    public class MachinistJobGauge : JobGauge
    {
        public MachinistJobGauge(string rawData) : base(rawData)
        {
        }

        public override Job Job => Job.MCH;

        protected override void InitializeConditions()
        {
            _names = new List<string>() {
                "Heat",
                "Overheat",
                "Overheat Timer (milliseconds)",
                "Battery",
                "Summon",
                "Summon Timer (milliseconds)"
            };

            _types = new List<TriggerConditionType>() {
                TriggerConditionType.Numeric,
                TriggerConditionType.Boolean,
                TriggerConditionType.Numeric,
                TriggerConditionType.Numeric,
                TriggerConditionType.Boolean,
                TriggerConditionType.Numeric
            };
        }

        public override (bool, DataSource) IsTriggered(bool preview)
        {
            CooldownDataSource data = new CooldownDataSource();
            MCHGauge gauge = Singletons.Get<DalamudJobGauges>().Get<MCHGauge>();

            data.Value = 0;
            data.MaxValue = 100;

            bool triggered =
                EvaluateCondition(0, gauge.Heat) &&
                EvaluateCondition(1, gauge.IsOverheated) &&
                EvaluateCondition(2, gauge.OverheatTimeRemaining) &&
                EvaluateCondition(3, gauge.Battery) &&
                EvaluateCondition(4, gauge.IsRobotActive) &&
                EvaluateCondition(5, gauge.SummonTimeRemaining);

            return (triggered, data);
        }
    }
}
