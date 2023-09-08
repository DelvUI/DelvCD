using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;

namespace DelvCD.Helpers
{
    [StructLayout(LayoutKind.Explicit, Size = 0x8)]
    public struct Combo
    {
        [FieldOffset(0x00)] public float Timer;
        [FieldOffset(0x04)] public uint Action;
    }

    public class TriggerData
    {
        public string Name;
        public uint Id;
        public uint Icon;
        public byte MaxStacks;
        public uint[] ComboId;
        public CombatType CombatType;

        public TriggerData(string name, uint id, uint icon, byte maxStacks = 0, uint[]? comboId = null, CombatType combatType = CombatType.PvE)
        {
            Name = name;
            Id = id;
            Icon = icon;
            MaxStacks = maxStacks;
            ComboId = comboId ?? new uint[0];
            CombatType = combatType;
        }
    }

    public struct RecastInfo
    {
        public float RecastTime;
        public float RecastTimeElapsed;
        public ushort MaxCharges;

        public RecastInfo(float recastTime, float recastTimeElapsed, ushort maxCharges)
        {
            RecastTime = recastTime;
            RecastTimeElapsed = recastTimeElapsed;
            MaxCharges = maxCharges;
        }
    }
}
