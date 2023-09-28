using Dalamud.Game.ClientState.JobGauge.Types;
using Dalamud.Plugin.Services;
using DelvCD.Helpers;
using DelvCD.Helpers.DataSources;
using DelvCD.Helpers.DataSources.JobDataSources;
using System.Collections.Generic;

namespace DelvCD.Config.JobGauges
{
    public class SummonerJobGauge : JobGauge
    {
        public SummonerJobGauge(string rawData) : base(rawData)
        {
        }

        public override Job Job => Job.SMN;
        private SummonerDataSource _dataSource = new();
        public override DataSource DataSource => _dataSource;

        private const int NONE_INDEX = 0;
        private const int BAHAMUT_INDEX = 1;
        private const int PHOENIX_INDEX = 2;

        private const int IFRIT_INDEX = 1;
        private const int TITAN_INDEX = 2;
        private const int GARUDA_INDEX = 3;

        protected override void InitializeConditions()
        {
            _names = new List<string>() {
                "Aetherflow Stacks",
                "Next Summon",
                "Active Summon",
                "Summon Timer",
                "Ifrit Ready",
                "Titan Ready",
                "Garuda Ready",
                "Active Attunement",
                "Attunement Timer",
                "Attunement Stacks"
            };

            _types = new List<TriggerConditionType>() {
                TriggerConditionType.Numeric,
                TriggerConditionType.Combo,
                TriggerConditionType.Combo,
                TriggerConditionType.Numeric,
                TriggerConditionType.Boolean,
                TriggerConditionType.Boolean,
                TriggerConditionType.Boolean,
                TriggerConditionType.Combo,
                TriggerConditionType.Numeric,
                TriggerConditionType.Numeric
            };

            string[] summons = new string[] { "None", "Bahamut", "Phoenix" };
            _comboOptions = new Dictionary<int, string[]>()
            {
                [1] = summons,
                [2] = summons,
                [7] = new string[] { "None", "Ifrit", "Titan", "Garuda" }
            };
        }

        public override bool IsTriggered(bool preview)
        {
            SMNGauge gauge = Singletons.Get<IJobGauges>().Get<SMNGauge>();

            _dataSource.Aetherflow_Stacks = gauge.AetherflowStacks;

            int nextSummon = NextSummon(gauge);
            _dataSource.Next_Summon = _comboOptions[1][nextSummon];

            int activeSummon = ActiveSummon(gauge);
            _dataSource.Active_Summon = _comboOptions[2][activeSummon];

            _dataSource.Summon_Timer = gauge.SummonTimerRemaining / 1000f;

            int attunement = ActiveAttunement(gauge);
            _dataSource.Active_Attunement = _comboOptions[7][attunement];

            _dataSource.Attunement_Timer = gauge.AttunmentTimerRemaining / 1000f;
            _dataSource.Attunement_Stacks = gauge.Attunement;
            _dataSource.Max_Attunement_Stacks = MaxAttunement(attunement);

            if (preview) { return true; }

            return
                EvaluateCondition(0, _dataSource.Aetherflow_Stacks) &&
                EvaluateCondition(1, nextSummon) &&
                EvaluateCondition(2, activeSummon) &&
                EvaluateCondition(3, _dataSource.Summon_Timer) &&
                EvaluateCondition(4, gauge.IsIfritReady) &&
                EvaluateCondition(5, gauge.IsTitanReady) &&
                EvaluateCondition(6, gauge.IsGarudaReady) &&
                EvaluateCondition(7, attunement) &&
                EvaluateCondition(8, _dataSource.Attunement_Timer) &&
                EvaluateCondition(9, _dataSource.Attunement_Stacks);
        }

        private int NextSummon(SMNGauge gauge)
        {
            return gauge.IsPhoenixReady ? PHOENIX_INDEX : BAHAMUT_INDEX;
        }

        private int ActiveSummon(SMNGauge gauge)
        {
            if (gauge.SummonTimerRemaining <= 0) { return NONE_INDEX; }

            return NextSummon(gauge);
        }

        private int ActiveAttunement(SMNGauge gauge)
        {
            if (gauge.IsIfritAttuned)
            {
                return IFRIT_INDEX;
            }
            else if (gauge.IsTitanAttuned)
            {
                return TITAN_INDEX;
            }
            else if (gauge.IsGarudaAttuned)
            {
                return GARUDA_INDEX;
            }

            return NONE_INDEX;
        }

        private int MaxAttunement(int attunement) => attunement switch
        {
            IFRIT_INDEX => 2,
            TITAN_INDEX => 4,
            GARUDA_INDEX => 4,
            _ => 0
        };
    }
}
