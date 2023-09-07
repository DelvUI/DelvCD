using Dalamud.Game.ClientState.JobGauge;
using Dalamud.Game.ClientState.JobGauge.Enums;
using Dalamud.Game.ClientState.JobGauge.Types;
using DelvCD.Helpers;
using DelvCD.Helpers.DataSources;
using System.Collections.Generic;
using static System.Runtime.InteropServices.JavaScript.JSType;
using DalamudJobGauges = Dalamud.Game.ClientState.JobGauge.JobGauges;

namespace DelvCD.Config.JobGauges
{
    public class AstrologianJobGauge : JobGauge
    {
        public AstrologianJobGauge(string rawData) : base(rawData)
        {
        }

        public override Job Job => Job.AST;

        protected override void InitializeConditions()
        {
            _names = new List<string>() {
                "Card",
                "Crown Card",
                "Astrosign"
            };

            _types = new List<TriggerConditionType>() {
                TriggerConditionType.Combo,
                TriggerConditionType.Combo,
                TriggerConditionType.Combo
            };

            _comboOptions = new Dictionary<int, string[]>()
            {
                [0] = new string[] { "None", "Balance", "Bole", "Arrow", "Spear", "Ewer", "Spire" },
                [1] = new string[] { "None", "Lord of Crowns", "Lady of Crowns" },
                [2] = new string[] { "None", "Solar", "Lunar", "Celestial" },
            };
        }

        public override (bool, DataSource) IsTriggered(bool preview)
        {
            CooldownDataSource data = new CooldownDataSource();
            ASTGauge gauge = Singletons.Get<DalamudJobGauges>().Get<ASTGauge>();

            data.Value = 0;
            data.MaxValue = 100;

            bool triggered =
                EvaluateCondition(0, (int)gauge.DrawnCard) &&
                EvaluateCrownCardCondition(gauge) &&
                EvaluateAstrosignsCondition(gauge);

            return (triggered, data);
        }

        private bool EvaluateCrownCardCondition(ASTGauge gauge)
        {
            if (!ConditionEnabledforIndex(1)) { return true; }

            return _values[1] switch
            {
                1 => gauge.DrawnCrownCard == CardType.LORD,
                2 => gauge.DrawnCrownCard == CardType.LADY,
                _ => gauge.DrawnCrownCard == CardType.NONE,
            };
        }

        private bool EvaluateAstrosignsCondition(ASTGauge gauge)
        {
            if (!ConditionEnabledforIndex(2)) { return true; }

            return _values[2] switch
            {
                1 => gauge.ContainsSeal(SealType.SUN),
                2 => gauge.ContainsSeal(SealType.MOON),
                3 => gauge.ContainsSeal(SealType.CELESTIAL),
                _ => gauge.Seals[0] == SealType.NONE && gauge.Seals[1] == SealType.NONE && gauge.Seals[2] == SealType.NONE
            };
        }
    }
}
