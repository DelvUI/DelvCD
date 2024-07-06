using Dalamud.Game.ClientState.JobGauge.Types;
using Dalamud.Game.ClientState.Objects.SubKinds;
using Dalamud.Plugin.Services;
using DelvCD.Helpers;
using DelvCD.Helpers.DataSources;
using DelvCD.Helpers.DataSources.JobDataSources;
using System.Collections.Generic;

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
                "Rattling Coil Stacks",
                "Serpent Offering",
                "Anguine Tribute Stacks"
            };

            _types = new List<TriggerConditionType>() {
                TriggerConditionType.Numeric,
                TriggerConditionType.Numeric,
                TriggerConditionType.Numeric
            };
        }

        public override unsafe bool IsTriggered(bool preview)
        {
            VPRGauge gauge = Singletons.Get<IJobGauges>().Get<VPRGauge>();
            IPlayerCharacter? player = Singletons.Get<IClientState>().LocalPlayer;

            _dataSource.Rattling_Coil_Stacks = gauge.RattlingCoilStacks;
            _dataSource.Max_Rattling_Coil_Stacks = player == null || player.Level < 88 ? 2 : 3;
            _dataSource.Serpent_Offering = gauge.SerpentOffering;
            _dataSource.Anguine_Tribute_Stacks = gauge.AnguineTribute;
            _dataSource.Max_Anguine_Tribute_Stacks = player == null || player.Level < 96 ? 4 : 5;

            if (preview) { return true; }

            return
                EvaluateCondition(0, _dataSource.Rattling_Coil_Stacks) &&
                EvaluateCondition(1, _dataSource.Serpent_Offering) &&
                EvaluateCondition(2, _dataSource.Anguine_Tribute_Stacks);
        }
    }
}
