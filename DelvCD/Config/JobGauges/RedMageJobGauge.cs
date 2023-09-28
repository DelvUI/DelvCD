using Dalamud.Game.ClientState.JobGauge.Types;
using Dalamud.Plugin.Services;
using DelvCD.Helpers;
using DelvCD.Helpers.DataSources;
using DelvCD.Helpers.DataSources.JobDataSources;
using System.Collections.Generic;

namespace DelvCD.Config.JobGauges
{
    public class RedMageJobGauge : JobGauge
    {
        public RedMageJobGauge(string rawData) : base(rawData)
        {
        }

        public override Job Job => Job.RDM;
        private RedMageDataSource _dataSource = new();
        public override DataSource DataSource => _dataSource;

        protected override void InitializeConditions()
        {
            _names = new List<string>() {
                "White Mana",
                "Black Mana",
                "Mana Stacks",
                "Mana Comparison"
            };

            _types = new List<TriggerConditionType>() {
                TriggerConditionType.Numeric,
                TriggerConditionType.Numeric,
                TriggerConditionType.Numeric,
                TriggerConditionType.Combo,
            };

            _comboOptions = new Dictionary<int, string[]>()
            {
                [3] = new string[] { "Equal", "White Mana > Black Mana", "Black Mana > White Mana" }
            };
        }

        public override bool IsTriggered(bool preview)
        {
            RDMGauge gauge = Singletons.Get<IJobGauges>().Get<RDMGauge>();

            _dataSource.White_Mana = gauge.WhiteMana;
            _dataSource.Black_Mana = gauge.BlackMana;
            _dataSource.Mana_Stacks = gauge.ManaStacks;

            if (preview) { return true; }

            return
                EvaluateCondition(0, _dataSource.White_Mana) &&
                EvaluateCondition(1, _dataSource.Black_Mana) &&
                EvaluateCondition(2, _dataSource.Mana_Stacks) &&
                EvaluateCondition(3, ManaComparisonValue(_dataSource.White_Mana, _dataSource.Black_Mana));
        }

        private int ManaComparisonValue(int whiteMana, int blackMana)
        {
            if (whiteMana == blackMana) { return 0; }
            if (whiteMana >  blackMana) { return 1; }
            return 2;
        }
    }
}
