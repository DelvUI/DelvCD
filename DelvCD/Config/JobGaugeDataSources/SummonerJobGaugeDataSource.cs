using Dalamud.Game.ClientState.JobGauge;
using Dalamud.Game.ClientState.JobGauge.Types;
using DelvCD.Helpers;
using System.Collections.Generic;

namespace DelvCD.Config.JobGaugeDataSources
{
    public class SummonerJobGaugeDataSource : JobGaugeDataSource
    {
        public SummonerJobGaugeDataSource(string rawData) : base(rawData)
        {
        }

        public override Job Job => Job.SMN;

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
                "Summon Timer (milliseconds)",
                "Ifrit Ready",
                "Titan Ready",
                "Garuda Ready",
                "Active Attunement",
                "Attunement Timer (milliseconds)",
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
                [7] = new string[] { "None", "Ifrit", "Titan", "Phoenix" }
            };
        }

        public override bool IsTriggered(bool preview, DataSource data)
        {
            SMNGauge gauge = Singletons.Get<JobGauges>().Get<SMNGauge>();

            data.Value = 0;
            data.MaxValue = 100;

            return
                EvaluateCondition(0, gauge.AetherflowStacks) &&
                EvaluateCondition(1, NextSummon(gauge)) &&
                EvaluateCondition(2, ActiveSummon(gauge)) &&
                EvaluateCondition(3, gauge.SummonTimerRemaining) &&
                EvaluateCondition(4, gauge.IsIfritReady) &&
                EvaluateCondition(5, gauge.IsTitanReady) &&
                EvaluateCondition(6, gauge.IsPhoenixReady) &&
                EvaluateCondition(7, ActiveAttunement(gauge)) &&
                EvaluateCondition(8, gauge.AttunmentTimerRemaining) &&
                EvaluateCondition(9, gauge.Attunement);
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
    }
}
