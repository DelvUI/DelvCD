using Dalamud.Game.ClientState.JobGauge.Enums;
using Dalamud.Game.ClientState.JobGauge.Types;
using Dalamud.Game.ClientState.Objects.SubKinds;
using Dalamud.Plugin.Services;
using DelvCD.Helpers;
using DelvCD.Helpers.DataSources;
using DelvCD.Helpers.DataSources.JobDataSources;
using System.Collections.Generic;
using System.Linq;

namespace DelvCD.Config.JobGauges
{
    public class MonkJobGauge : JobGauge
    {
        public MonkJobGauge(string rawData) : base(rawData)
        {
        }

        public override Job Job => Job.MNK;
        private MonkDataSource _dataSource = new();
        public override DataSource DataSource => _dataSource;

        protected override void InitializeConditions()
        {
            _names = new List<string>() {
                "Chakra Stacks",
                "Master's Gauge Chakra #1",
                "Master's Gauge Chakra #2",
                "Master's Gauge Chakra #3",
                "Solar Nadi",
                "Lunar Nadi",
                "Blitz Timer",
                "Opoopo Stacks",
                "Raptor Stacks",
                "Coeurl Stacks"
            };

            _types = new List<TriggerConditionType>() {
                TriggerConditionType.Numeric,
                TriggerConditionType.Combo,
                TriggerConditionType.Combo,
                TriggerConditionType.Combo,
                TriggerConditionType.Boolean,
                TriggerConditionType.Boolean,
                TriggerConditionType.Numeric,
                TriggerConditionType.Numeric,
                TriggerConditionType.Numeric
            };

            string[] chakras = new string[] { "None", "Coeurl", "Opo-opo", "Raptor" };
            _comboOptions = new Dictionary<int, string[]>()
            {
                [1] = chakras,
                [2] = chakras,
                [3] = chakras,
            };
        }

        public override bool IsTriggered(bool preview)
        {
            MNKGauge gauge = Singletons.Get<IJobGauges>().Get<MNKGauge>();

            _dataSource.Chakra_Stacks = gauge.Chakra;
            _dataSource.Masters_Gauge_Chakra_1 = _comboOptions[1][_values[1]];
            _dataSource.Masters_Gauge_Chakra_2 = _comboOptions[2][_values[2]];
            _dataSource.Masters_Gauge_Chakra_3 = _comboOptions[3][_values[3]];
            _dataSource.Solar_Nadi = (gauge.Nadi & Nadi.SOLAR) != 0;
            _dataSource.Lunar_Nadi = (gauge.Nadi & Nadi.LUNAR) != 0;
            _dataSource.Blitz_Timer = gauge.BlitzTimeRemaining / 1000f;
            _dataSource.Opoopo_Stacks = gauge.OpoOpoFury;
            _dataSource.Raptor_Stacks = gauge.RaptorFury;
            _dataSource.Coeurl_Stacks = gauge.CoeurlFury;

            IPlayerCharacter? player = Singletons.Get<IClientState>().LocalPlayer;
            _dataSource.Max_Chakra_Stacks = player?.StatusList.FirstOrDefault(s => s.StatusId is 1182 or 2174) != null ? 10 : 5;

            if (preview) { return true; }

            return
                EvaluateCondition(0, _dataSource.Chakra_Stacks) &&
                EvaluateMastersGaugeChakraCondition(gauge, 0) &&
                EvaluateMastersGaugeChakraCondition(gauge, 1) &&
                EvaluateMastersGaugeChakraCondition(gauge, 2) &&
                EvaluateCondition(4, _dataSource.Solar_Nadi) &&
                EvaluateCondition(5, _dataSource.Lunar_Nadi) &&
                EvaluateCondition(6, _dataSource.Blitz_Timer) &&
                EvaluateCondition(7, _dataSource.Opoopo_Stacks) &&
                EvaluateCondition(8, _dataSource.Raptor_Stacks) &&
                EvaluateCondition(9, _dataSource.Coeurl_Stacks);
        }

        private bool EvaluateMastersGaugeChakraCondition(MNKGauge gauge, int chakra)
        {
            if (!ConditionEnabledforIndex(1 + chakra)) { return true; }

            return (int)gauge.BeastChakra[chakra] == _values[1 + chakra];
        }
    }
}
