using Dalamud.Game.ClientState.JobGauge.Types;
using DelvCD.Helpers;
using DelvCD.Helpers.DataSources;
using DelvCD.Helpers.DataSources.JobDataSources;
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
        private BlackMageDataSource _dataSource = new();
        public override DataSource DataSource => _dataSource;

        protected override void InitializeConditions()
        {
            _names = new List<string>() {
                "Enochian",
                "Enochian Timer",
                "Polyglot Stacks",
                "Element",
                "Element Timer",
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

        public override bool IsTriggered(bool preview)
        {
            BLMGauge gauge = Singletons.Get<DalamudJobGauges>().Get<BLMGauge>();

            _dataSource.Enochian = gauge.IsEnochianActive;
            _dataSource.Enochian_Timer = gauge.EnochianTimer / 1000f;
            _dataSource.Polyglot_Stacks = gauge.PolyglotStacks;
            _dataSource.Element = _comboOptions[3][_values[3]];
            _dataSource.Element_Timer = gauge.ElementTimeRemaining / 1000f;
            _dataSource.Umbral_Ice_Stacks = gauge.UmbralIceStacks;
            _dataSource.Astral_Fire_Stacks = gauge.AstralFireStacks;
            _dataSource.Umbral_Hearts = gauge.UmbralHearts;
            _dataSource.Paradox = gauge.IsParadoxActive;

            if (preview) { return true; }

            return
                EvaluateCondition(0, _dataSource.Enochian) &&
                EvaluateCondition(1, _dataSource.Enochian_Timer) &&
                EvaluateCondition(2, _dataSource.Polyglot_Stacks) &&
                EvaluateElementCondition(gauge) &&
                EvaluateCondition(4, _dataSource.Element_Timer) &&
                EvaluateCondition(5, _dataSource.Umbral_Ice_Stacks) &&
                EvaluateCondition(6, _dataSource.Astral_Fire_Stacks) &&
                EvaluateCondition(7, _dataSource.Umbral_Hearts) &&
                EvaluateCondition(8, _dataSource.Paradox);
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
