using Dalamud.Game.ClientState.JobGauge.Types;
using Dalamud.Plugin.Services;
using DelvCD.Helpers;
using DelvCD.Helpers.DataSources;
using DelvCD.Helpers.DataSources.JobDataSources;
using System.Collections.Generic;

namespace DelvCD.Config.JobGauges
{
    public class MachinistJobGauge : JobGauge
    {
        public MachinistJobGauge(string rawData) : base(rawData)
        {
        }

        public override Job Job => Job.MCH;
        private MachinistDataSource _dataSource = new();
        public override DataSource DataSource => _dataSource;

        protected override void InitializeConditions()
        {
            _names = new List<string>() {
                "Heat",
                "Overheat",
                "Overheat Timer",
                "Battery",
                "Summon",
                "Summon Timer"
            };

            _types = new List<TriggerConditionType>() {
                TriggerConditionType.Numeric,
                TriggerConditionType.Boolean,
                TriggerConditionType.Numeric,
                TriggerConditionType.Numeric,
                TriggerConditionType.Boolean,
                TriggerConditionType.Numeric
            };
        }

        public override bool IsTriggered(bool preview)
        {
            MCHGauge gauge = Singletons.Get<IJobGauges>().Get<MCHGauge>();

            _dataSource.Heat = gauge.Heat;
            _dataSource.Overheat = gauge.IsOverheated;
            _dataSource.Overheat_Timer = gauge.OverheatTimeRemaining / 1000f;
            _dataSource.Battery = gauge.Battery;
            _dataSource.Summon = gauge.IsRobotActive;
            _dataSource.Summon_Timer = gauge.SummonTimeRemaining / 1000f;

            if (preview) { return true; }

            return
                EvaluateCondition(0, _dataSource.Heat) &&
                EvaluateCondition(1, _dataSource.Overheat) &&
                EvaluateCondition(2, _dataSource.Overheat_Timer) &&
                EvaluateCondition(3, _dataSource.Battery) &&
                EvaluateCondition(4, _dataSource.Summon) &&
                EvaluateCondition(5, _dataSource.Summon_Timer);
        }
    }
}
