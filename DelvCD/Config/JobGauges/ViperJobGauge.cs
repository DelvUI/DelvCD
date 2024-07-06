using System.Collections.Generic;
using Dalamud.Game.ClientState.JobGauge.Enums;
using Dalamud.Game.ClientState.JobGauge.Types;
using Dalamud.Plugin.Services;
using DelvCD.Helpers;
using DelvCD.Helpers.DataSources;
using DelvCD.Helpers.DataSources.JobDataSources;

namespace DelvCD.Config.JobGauges
{
    public class ViperJobGauge : JobGauge
    {
        public ViperJobGauge(string rawData) : base(rawData)
        {
        }

        public override Job Job => Job.VPR;
        private ViperDataSource _dataSource = new();
        public override DataSource DataSource => _dataSource;

        protected override void InitializeConditions()
        {
            _names = new List<string>() {
                "Serpent Offering",
                "Anguine Tribute",
                "Rattling Coil Stacks",
                "Dread Combo"
            };

            _types = new List<TriggerConditionType>() {
                TriggerConditionType.Numeric,
                TriggerConditionType.Numeric,
                TriggerConditionType.Numeric,
                TriggerConditionType.Numeric,
            };
        }

        public override bool IsTriggered(bool preview)
        {
            VPRGauge gauge = Singletons.Get<IJobGauges>().Get<VPRGauge>();
            _dataSource.SerpentOffering = gauge.SerpentOffering;
            _dataSource.AnguineTribute = gauge.AnguineTribute;
            _dataSource.RattlingCoilStacks = gauge.RattlingCoilStacks;

            // Converting DreadCombo enum values to their action ids which should be more useful
            _dataSource.DreadComboId = gauge.DreadCombo switch
            {
                DreadCombo.Dreadwinder => 34620,
                DreadCombo.HuntersCoil => 34621,
                DreadCombo.SwiftskinsCoil => 34622,
                DreadCombo.PitOfDread => 34623,
                DreadCombo.HuntersDen => 34624,
                DreadCombo.SwiftskinsDen => 34625,
                _ => 0
            };

            if (preview) { return true; }

            return
                EvaluateCondition(0, _dataSource.SerpentOffering) &&
                EvaluateCondition(1, _dataSource.AnguineTribute) &&
                EvaluateCondition(2, _dataSource.RattlingCoilStacks) &&
                EvaluateCondition(3, _dataSource.DreadComboId);
        }
    }
}
