using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Runtime.InteropServices;
using Dalamud.Game.ClientState.JobGauge.Types;
using Dalamud.Game.ClientState.Objects.SubKinds;
using Dalamud.Plugin.Services;
using DelvCD.Helpers;
using DelvCD.Helpers.DataSources;
using DelvCD.Helpers.DataSources.JobDataSources;
using FFXIVClientStructs.FFXIV.Client.Game.Gauge;

namespace DelvCD.Config.JobGauges
{
    public class BlackMageJobGauge : JobGauge
    {
        public BlackMageJobGauge(string rawData) : base(rawData)
        {
        }

        public override Job Job => Job.BLM;
        private BlackMageDataSource _dataSource = new();
        public override DataSource DataSource => _dataSource;

        protected override void InitializeConditions()
        {
            _names = new List<string>() {
                "Enochian",
                "Enochian Timer",
                "Polyglot Stacks",
                "Element",
                "Element Timer",
                "Umbral Ice Stacks",
                "Astral Fire Stacks",
                "Umbral Hearts",
                "Paradox",
                "Astral Soul Stacks"
            };

            _types = new List<TriggerConditionType>() {
                TriggerConditionType.Boolean,
                TriggerConditionType.Numeric,
                TriggerConditionType.Numeric,
                TriggerConditionType.Combo,
                TriggerConditionType.Numeric,
                TriggerConditionType.Numeric,
                TriggerConditionType.Numeric,
                TriggerConditionType.Numeric,
                TriggerConditionType.Boolean,
                TriggerConditionType.Numeric
            };

            _comboOptions = new Dictionary<int, string[]>()
            {
                [3] = new string[] { "None", "Umbral Ice", "Astral Fire" },
            };
        }

        public override unsafe bool IsTriggered(bool preview)
        {
            BLMGauge _gauge = Singletons.Get<IJobGauges>().Get<BLMGauge>();
            BlackMageGaugeTmp* gauge = (BlackMageGaugeTmp*)_gauge.Address;

            _dataSource.Enochian = gauge->EnochianActive;
            _dataSource.Enochian_Timer = gauge->EnochianTimer / 1000f;
            _dataSource.Polyglot_Stacks = gauge->PolyglotStacks;
            _dataSource.Element = _comboOptions[3][_values[3]];
            _dataSource.Element_Timer = 0; // TODO: Figure out how to remove this without breaking everything...
            _dataSource.Umbral_Ice_Stacks = gauge->UmbralStacks;
            _dataSource.Astral_Fire_Stacks = gauge->AstralStacks;
            _dataSource.Umbral_Hearts = gauge->UmbralHearts;
            _dataSource.Paradox = gauge->ParadoxActive;
            _dataSource.Astral_Soul_Stacks = gauge->AstralSoulStacks;

            IPlayerCharacter? player = Singletons.Get<IClientState>().LocalPlayer;
            _dataSource.Max_Polyglot_Stacks = player == null ? 2 : (player.Level < 80 ? 1 : ((player.Level < 98 ? 2 : 3)));

            if (preview) { return true; }

            return
                EvaluateCondition(0, _dataSource.Enochian) &&
                EvaluateCondition(1, _dataSource.Enochian_Timer) &&
                EvaluateCondition(2, _dataSource.Polyglot_Stacks) &&
                EvaluateElementCondition(gauge) &&
                EvaluateCondition(4, _dataSource.Element_Timer) &&
                EvaluateCondition(5, _dataSource.Umbral_Ice_Stacks) &&
                EvaluateCondition(6, _dataSource.Astral_Fire_Stacks) &&
                EvaluateCondition(7, _dataSource.Umbral_Hearts) &&
                EvaluateCondition(8, _dataSource.Paradox) &&
                EvaluateCondition(9, _dataSource.Astral_Soul_Stacks);
        }

        private unsafe bool EvaluateElementCondition(BlackMageGaugeTmp* gauge)
        {
            if (!ConditionEnabledforIndex(3)) { return true; }

            bool inUmbralIce = gauge->ElementStance < 0;
            bool inAstralFire = gauge->ElementStance > 0;

            return _values[3] switch
            {
                1 => inUmbralIce,
                2 => inAstralFire,
                _ => !inUmbralIce && !inAstralFire,
            };
        }
    }
}


[StructLayout(LayoutKind.Explicit, Size = 0x30)]
public struct BlackMageGaugeTmp
{
    [FieldOffset(0x08)] public short EnochianTimer;
    [FieldOffset(0x0A)] public sbyte ElementStance;
    [FieldOffset(0x0B)] public byte UmbralHearts;
    [FieldOffset(0x0C)] public byte PolyglotStacks;
    [FieldOffset(0x0D)] public EnochianFlags EnochianFlags;

    public int UmbralStacks => ElementStance >= 0 ? 0 : ElementStance * -1;
    public int AstralStacks => ElementStance <= 0 ? 0 : ElementStance;
    public bool EnochianActive => EnochianFlags.HasFlag(EnochianFlags.Enochian);
    public bool ParadoxActive => EnochianFlags.HasFlag(EnochianFlags.Paradox);
    public int AstralSoulStacks => ((int)EnochianFlags >> 2) & 7;
}
