using DelvCD.Helpers;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Numerics;

namespace DelvCD.Config
{
    public abstract class TriggerOptions
    {
        [JsonIgnore] public static readonly string[] OperatorOptions = new string[] { "==", "!=", "<", ">", "<=", ">=" };

        public TriggerCond Condition = TriggerCond.And;
        public List<TriggerData> TriggerData { get; set; } = new List<TriggerData>();

        public abstract TriggerType Type { get; }
        public abstract TriggerSource Source { get; }
        public abstract bool IsTriggered(bool preview, out DataSource data);
        public abstract void DrawTriggerOptions(Vector2 size, float padX, float padY);
    }
}