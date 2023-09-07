using Dalamud.Game.ClientState;
using Dalamud.Game.ClientState.JobGauge;
using Dalamud.Game.ClientState.JobGauge.Types;
using DelvCD.Helpers;
using DelvCD.Helpers.DataSources;
using System.Collections.Generic;
using DalamudJobGauges = Dalamud.Game.ClientState.JobGauge.JobGauges;

namespace DelvCD.Config.JobGauges
{
    internal enum DNCStep : uint
    {
        None = 15998,
        Emboite = 15999,
        Entrechat = 16000,
        Jete = 16001,
        Pirouette = 16002
    }

    public class DancerJobGauge : JobGauge
    {
        public DancerJobGauge(string rawData) : base(rawData)
        {
        }

        public override Job Job => Job.DNC;

        protected override void InitializeConditions()
        {
            _names = new List<string>() {
                "Fourfold Feathers Stacks",
                "Espirit",
                "Dancing",
                "Next Step",
                "Step #1",
                "Step #2",
                "Step #3",
                "Step #4"
            };

            _types = new List<TriggerConditionType>() {
                TriggerConditionType.Numeric,
                TriggerConditionType.Numeric,
                TriggerConditionType.Boolean,
                TriggerConditionType.Combo,
                TriggerConditionType.Combo,
                TriggerConditionType.Combo,
                TriggerConditionType.Combo,
                TriggerConditionType.Combo
            };

            string[] steps = new string[] { "None", "Emboite", "Entrechat", "Jete", "Pirouette" };
            _comboOptions = new Dictionary<int, string[]>()
            {
                [3] = steps,
                [4] = steps,
                [5] = steps,
                [6] = steps,
                [7] = steps,
            };
        }

        public override (bool, DataSource) IsTriggered(bool preview)
        {
            CooldownDataSource data = new CooldownDataSource();
            DNCGauge gauge = Singletons.Get<DalamudJobGauges>().Get<DNCGauge>();

            data.Value = 0;
            data.MaxValue = 100;

            bool triggered =
                EvaluateCondition(0, gauge.Feathers) &&
                EvaluateCondition(1, gauge.Esprit) &&
                EvaluateCondition(2, gauge.IsDancing) &&
                EvaluateCondition(3, (int)gauge.NextStep - (int)DNCStep.None) &&
                EvaluateStepCondition(gauge, 0) &&
                EvaluateStepCondition(gauge, 1) &&
                EvaluateStepCondition(gauge, 2) &&
                EvaluateStepCondition(gauge, 3);

            return (triggered, data);
        }

        private bool EvaluateStepCondition(DNCGauge gauge, int step)
        {
            if (!ConditionEnabledforIndex(4 + step)) { return true; }

            int value = (int)gauge.Steps[step] - (int)DNCStep.None;
            return (value == _values[4 + step]);
        }
    }
}
