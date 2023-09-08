using DelvCD.Helpers;
using DelvCD.Helpers.DataSources;
using Newtonsoft.Json;
using System;
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
        public abstract bool IsTriggered(bool preview);
        public abstract void DrawTriggerOptions(Vector2 size, float padX, float padY);

        [JsonIgnore] public abstract DataSource DataSource { get; }
        [JsonIgnore] public Action? OnDataSourceChange { get; set; }
    }
}