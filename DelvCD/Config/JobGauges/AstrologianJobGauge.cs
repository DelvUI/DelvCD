using Dalamud.Game.ClientState.JobGauge.Enums;
using Dalamud.Game.ClientState.JobGauge.Types;
using Dalamud.Plugin.Services;
using DelvCD.Helpers;
using DelvCD.Helpers.DataSources;
using DelvCD.Helpers.DataSources.JobDataSources;
using System.Collections.Generic;

namespace DelvCD.Config.JobGauges
{
    public class AstrologianJobGauge : JobGauge
    {
        public AstrologianJobGauge(string rawData) : base(rawData)
        {
        }

        public override Job Job => Job.AST;
        private AstrologianDataSource _dataSource = new();
        public override DataSource DataSource => _dataSource;

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

        public override bool IsTriggered(bool preview)
        {
            ASTGauge gauge = Singletons.Get<IJobGauges>().Get<ASTGauge>();

            _dataSource.Card = _comboOptions[0][_values[0]];
            _dataSource.Crown_Card = _comboOptions[1][_values[1]];
            _dataSource.Astrosign_Solar = gauge.ContainsSeal(SealType.SUN);
            _dataSource.Astrosign_Lunar = gauge.ContainsSeal(SealType.MOON);
            _dataSource.Astrosign_Celestial = gauge.ContainsSeal(SealType.CELESTIAL);

            if (preview) { return true; }

            return
                EvaluateCondition(0, (int)gauge.DrawnCard) &&
                EvaluateCrownCardCondition(gauge) &&
                EvaluateAstrosignsCondition(gauge);
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
