using Dalamud.Game.ClientState.JobGauge;
using Dalamud.Game.ClientState.JobGauge.Types;
using DelvCD.Helpers;
using System.Collections.Generic;

namespace DelvCD.Config.JobGaugeDataSources
{
    public class BlackMageJobGaugeDataSource : JobGaugeDataSource
    {
        public BlackMageJobGaugeDataSource(string rawData) : base(rawData)
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

        public override bool IsTriggered(bool preview, DataSource data)
        {
            BLMGauge gauge = Singletons.Get<JobGauges>().Get<BLMGauge>();

            data.Value = 0;
            data.MaxValue = 100;

            return
                EvaluateCondition(0, gauge.IsEnochianActive) &&
                EvaluateCondition(1, gauge.EnochianTimer) &&
                EvaluateCondition(2, gauge.PolyglotStacks) &&
                EvaluateElementCondition(gauge) &&
                EvaluateCondition(4, gauge.ElementTimeRemaining) &&
                EvaluateCondition(5, gauge.UmbralIceStacks) &&
                EvaluateCondition(6, gauge.AstralFireStacks) &&
                EvaluateCondition(7, gauge.UmbralHearts) &&
                EvaluateCondition(8, gauge.IsParadoxActive);
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
