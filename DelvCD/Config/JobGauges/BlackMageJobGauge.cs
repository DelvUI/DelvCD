using Dalamud.Game.ClientState.JobGauge;
using Dalamud.Game.ClientState.JobGauge.Types;
using DelvCD.Helpers;
using DelvCD.Helpers.DataSources;
using System.Collections.Generic;
using DalamudJobGauges = Dalamud.Game.ClientState.JobGauge.JobGauges;

namespace DelvCD.Config.JobGauges
{
    public class BlackMageJobGauge : JobGauge
    {
        public BlackMageJobGauge(string rawData) : base(rawData)
        {
        }

        public override Job Job => Job.BLM;

        protected override void InitializeConditions()
        {
            _names = new List<string>() {
                "Enochian",
                "Enochian Timer (milliseconds)",
                "Polyglot Stacks",
                "Element",
                "Element Timer (milliseconds)",
                "Umbral Ice Stacks",
                "Astral Fire Stacks",
                "Umbral Hearts",
                "Paradox"
            };

            _types = new List<TriggerConditionType>() {
                TriggerConditionType.Boolean,
                TriggerConditionType.Numeric,
                TriggerConditionType.Numeric,
                TriggerConditionType.Combo,
                TriggerConditionType.Numeric,
                TriggerConditionType.Numeric,
                TriggerConditionType.Numeric,
                TriggerConditionType.Numeric,
                TriggerConditionType.Boolean
            };

            _comboOptions = new Dictionary<int, string[]>()
            {
                [3] = new string[] { "None", "Umbral Ice", "Astral Fire" },
            };
        }

        public override (bool, DataSource) IsTriggered(bool preview)
        {
            CooldownDataSource data = new CooldownDataSource();
            BLMGauge gauge = Singletons.Get<DalamudJobGauges>().Get<BLMGauge>();

            data.Value = 0;
            data.MaxValue = 100;

            bool triggered =
                EvaluateCondition(0, gauge.IsEnochianActive) &&
                EvaluateCondition(1, gauge.EnochianTimer) &&
                EvaluateCondition(2, gauge.PolyglotStacks) &&
                EvaluateElementCondition(gauge) &&
                EvaluateCondition(4, gauge.ElementTimeRemaining) &&
                EvaluateCondition(5, gauge.UmbralIceStacks) &&
                EvaluateCondition(6, gauge.AstralFireStacks) &&
                EvaluateCondition(7, gauge.UmbralHearts) &&
                EvaluateCondition(8, gauge.IsParadoxActive);

            return (triggered, data);
        }

        private bool EvaluateElementCondition(BLMGauge gauge)
        {
            if (!ConditionEnabledforIndex(3)) { return true; }

            return _values[3] switch
            {
                1 => gauge.InUmbralIce,
                2 => gauge.InAstralFire,
                _ => !gauge.InUmbralIce && !gauge.InAstralFire,
            };
        }
    }
}
