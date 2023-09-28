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
                "Mana Stacks"
            };

            _types = new List<TriggerConditionType>() {
                TriggerConditionType.Numeric,
                TriggerConditionType.Numeric,
                TriggerConditionType.Numeric
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
                EvaluateCondition(2, _dataSource.Mana_Stacks);
        }
    }
}
