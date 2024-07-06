using Dalamud.Game.ClientState.JobGauge.Enums;
using Dalamud.Game.ClientState.JobGauge.Types;
using Dalamud.Plugin.Services;
using DelvCD.Helpers;
using DelvCD.Helpers.DataSources;
using DelvCD.Helpers.DataSources.JobDataSources;
using System.Collections.Generic;

namespace DelvCD.Config.JobGauges
{
    internal class PictomancerJobGauge : JobGauge
    {
        public PictomancerJobGauge(string rawData) : base(rawData)
        {
        }

        public override Job Job => Job.PCT;
        private PictomancerDataSource _dataSource = new();
        public override DataSource DataSource => _dataSource;

        protected override void InitializeConditions()
        {
            _names = new List<string>() {
                "Pallete",
                "Paint",
                "Creature Motif Drawn",
                "Weapon Motif Drawn",
                "Landscape Motif Drawn",
                "Creature Motif",
                "Creature Canvas",
                "Creature Portrait"
            };

            _types = new List<TriggerConditionType>() {
                TriggerConditionType.Numeric,
                TriggerConditionType.Numeric,
                TriggerConditionType.Boolean,
                TriggerConditionType.Boolean,
                TriggerConditionType.Boolean,
                TriggerConditionType.Combo,
                TriggerConditionType.Combo,
                TriggerConditionType.Combo
            };

            _comboOptions = new Dictionary<int, string[]>()
            {
                [5] = new string[] { "None", "Pom", "Wings", "Claw", "Fangs" },
                [6] = new string[] { "None", "Pom", "Wings", "Claw" },
                [7] = new string[] { "None", "Moogle", "Madeen" }
            };
        }

        public override bool IsTriggered(bool preview)
        {
            PCTGauge gauge = Singletons.Get<IJobGauges>().Get<PCTGauge>();

            _dataSource.Pallete = gauge.PalleteGauge;
            _dataSource.Paint = gauge.Paint;
            _dataSource.Creature_Motif_Drawn = gauge.CreatureMotifDrawn;
            _dataSource.Weapon_Motif_Drawn = gauge.WeaponMotifDrawn;
            _dataSource.Landscape_Motif_Drawn = gauge.LandscapeMotifDrawn;
            _dataSource.Creature_Motif = _comboOptions[5][_values[5]];
            _dataSource.Creature_Canvas = _comboOptions[6][_values[6]];
            _dataSource.Creature_Portrait = _comboOptions[7][_values[7]];

            if (preview) { return true; }

            return
                EvaluateCondition(0, _dataSource.Pallete) &&
                EvaluateCondition(1, _dataSource.Paint) &&
                EvaluateCondition(2, _dataSource.Creature_Motif_Drawn) &&
                EvaluateCondition(3, _dataSource.Weapon_Motif_Drawn) &&
                EvaluateCondition(4, _dataSource.Landscape_Motif_Drawn) &&
                EvaluateCreatureMotifCondition(gauge) &&
                EvaluateCreatureCanvasCondition(gauge) &&
                EvaluateCreaturePortraitCondition(gauge);
        }

        private bool EvaluateCreatureMotifCondition(PCTGauge gauge)
        {
            if (!ConditionEnabledforIndex(5)) { return true; }

            return _values[5] switch
            {
                1 => gauge.CanvasFlags.HasFlag(CanvasFlags.Pom),
                2 => gauge.CanvasFlags.HasFlag(CanvasFlags.Wing),
                3 => gauge.CanvasFlags.HasFlag(CanvasFlags.Claw),
                4 => gauge.CanvasFlags.HasFlag(CanvasFlags.Maw),
                _ => !gauge.CanvasFlags.HasFlag(CanvasFlags.Pom) &&
                     !gauge.CanvasFlags.HasFlag(CanvasFlags.Wing) &&
                     !gauge.CanvasFlags.HasFlag(CanvasFlags.Claw) &&
                     !gauge.CanvasFlags.HasFlag(CanvasFlags.Maw)
            };
        }

        private bool EvaluateCreatureCanvasCondition(PCTGauge gauge)
        {
            if (!ConditionEnabledforIndex(6)) { return true; }

            return _values[6] switch
            {
                1 => gauge.CreatureFlags.HasFlag(CreatureFlags.Pom),
                2 => gauge.CreatureFlags.HasFlag(CreatureFlags.Wings),
                3 => gauge.CreatureFlags.HasFlag(CreatureFlags.Claw),
                _ => !gauge.CreatureFlags.HasFlag(CreatureFlags.Pom) &&
                     !gauge.CreatureFlags.HasFlag(CreatureFlags.Wings) &&
                     !gauge.CreatureFlags.HasFlag(CreatureFlags.Claw)
            };
        }

        private bool EvaluateCreaturePortraitCondition(PCTGauge gauge)
        {
            if (!ConditionEnabledforIndex(7)) { return true; }

            return _values[7] switch
            {
                1 => gauge.CreatureFlags.HasFlag(CreatureFlags.MooglePortait),
                2 => gauge.CreatureFlags.HasFlag(CreatureFlags.MadeenPortrait),
                _ => !gauge.CreatureFlags.HasFlag(CreatureFlags.MooglePortait) &&
                     !gauge.CreatureFlags.HasFlag(CreatureFlags.MadeenPortrait)
            };
        }
    }
}
