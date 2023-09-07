using Dalamud.Game.ClientState.JobGauge.Enums;
using Dalamud.Game.ClientState.JobGauge.Types;
using DelvCD.Helpers;
using DelvCD.Helpers.DataSources;
using DelvCD.Helpers.DataSources.JobDataSources;
using System.Collections.Generic;
using DalamudJobGauges = Dalamud.Game.ClientState.JobGauge.JobGauges;

namespace DelvCD.Config.JobGauges
{
    public class BardJobGauge : JobGauge
    {
        public BardJobGauge(string rawData) : base(rawData)
        {
        }

        public override Job Job => Job.BRD;
        private BardDataSource _dataSource = new();
        public override DataSource DataSource => _dataSource;

        protected override void InitializeConditions()
        {
            _names = new List<string>() {
                "Active Song",
                "Last Active Song",
                "Song Time",
                "Repertoire Stacks",
                "Soul Voice",
                "Coda"
            };

            _types = new List<TriggerConditionType>() {
                TriggerConditionType.Combo,
                TriggerConditionType.Combo,
                TriggerConditionType.Numeric,
                TriggerConditionType.Numeric,
                TriggerConditionType.Numeric,
                TriggerConditionType.Combo
            };

            string[] songs = new string[] { "None", "Mage's Ballad", "Army's Paeon", "The Wanderer's Minute" };
            _comboOptions = new Dictionary<int, string[]>()
            {
                [0] = songs,
                [1] = songs,
                [5] = songs
            };
        }

        public override bool IsTriggered(bool preview)
        {
            BRDGauge gauge = Singletons.Get<DalamudJobGauges>().Get<BRDGauge>();

            _dataSource.Active_Song = _comboOptions[0][_values[0]];
            _dataSource.Last_Active_Song = _comboOptions[0][_values[0]];
            _dataSource.Song_Timer = gauge.SongTimer / 1000f;
            _dataSource.Repertoire_Stacks = gauge.Repertoire;
            _dataSource.Max_Repertoire_Stacks = MaxRepertoireStacks(gauge.Song);
            _dataSource.Soul_Voice = gauge.SoulVoice;

            if (preview) { return true; }

            return
                EvaluateCondition(0, (int)gauge.Song) &&
                EvaluateCondition(1, (int)gauge.LastSong) &&
                EvaluateCondition(2, _dataSource.Song_Timer) &&
                EvaluateCondition(3, _dataSource.Repertoire_Stacks) &&
                EvaluateCondition(4, _dataSource.Soul_Voice) &&
                EvaluateCodaCondition(gauge);
        }

        private bool EvaluateCodaCondition(BRDGauge gauge)
        {
            if (!ConditionEnabledforIndex(5)) { return true; }

            int value = _values[5];

            // none?
            if (value == 0)
            {
                return gauge.Coda[0] == Song.NONE && gauge.Coda[1] == Song.NONE && gauge.Coda[2] == Song.NONE;
            }
            else if (value < 4)
            {
                return gauge.Coda[value - 1] != Song.NONE;
            }

            return true;
        }

        private int MaxRepertoireStacks(Song song) => song switch
        {
            Song.ARMY => 4,
            Song.WANDERER => 3,
            _ => 0
        };
    }
}
