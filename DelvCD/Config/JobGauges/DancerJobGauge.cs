using Dalamud.Game.ClientState.JobGauge.Types;
using DelvCD.Helpers;
using DelvCD.Helpers.DataSources;
using DelvCD.Helpers.DataSources.JobDataSources;
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
        private DancerDataSource _dataSource = new();
        public override DataSource DataSource => _dataSource;

        protected override void InitializeConditions()
        {
            _names = new List<string>() {
                "Fourfold Feathers Stacks",
                "Esprit",
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

        public override bool IsTriggered(bool preview)
        {
            DNCGauge gauge = Singletons.Get<DalamudJobGauges>().Get<DNCGauge>();

            _dataSource.Feather_Stacks = gauge.Feathers;
            _dataSource.Esprit = gauge.Esprit;
            _dataSource.Dancing = gauge.IsDancing;

            if (preview) { return true; }

            return
                EvaluateCondition(0, _dataSource.Feather_Stacks) &&
                EvaluateCondition(1, _dataSource.Esprit) &&
                EvaluateCondition(2, _dataSource.Dancing) &&
                EvaluateCondition(3, (int)gauge.NextStep - (int)DNCStep.None) &&
                EvaluateStepCondition(gauge, 0) &&
                EvaluateStepCondition(gauge, 1) &&
                EvaluateStepCondition(gauge, 2) &&
                EvaluateStepCondition(gauge, 3);
        }

        private bool EvaluateStepCondition(DNCGauge gauge, int step)
        {
            if (!ConditionEnabledforIndex(4 + step)) { return true; }

            int value = (int)gauge.Steps[step] - (int)DNCStep.None;
            return (value == _values[4 + step]);
        }
    }
}
