using Dalamud.Game.ClientState.JobGauge.Enums;
using Dalamud.Game.ClientState.JobGauge.Types;
using Dalamud.Game.ClientState.Objects.SubKinds;
using Dalamud.Plugin.Services;
using DelvCD.Helpers;
using DelvCD.Helpers.DataSources;
using DelvCD.Helpers.DataSources.JobDataSources;
using System.Collections.Generic;
using System.Linq;

namespace DelvCD.Config.JobGauges
{
    public class MonkJobGauge : JobGauge
    {
        public MonkJobGauge(string rawData) : base(rawData)
        {
        }

        public override Job Job => Job.MNK;
        private MonkDataSource _dataSource = new();
        public override DataSource DataSource => _dataSource;

        protected override void InitializeConditions()
        {
            _names = new List<string>() {
                "Chakra Stacks",
                "Master's Gauge Opo-opo Count",
                "Master's Gauge Raptor Count",
                "Master's Gauge Coeurl Count",
                "Solar Nadi",
                "Lunar Nadi",
                "Blitz Timer",
                "Opoopo Stacks",
                "Raptor Stacks",
                "Coeurl Stacks"
            };

            _types = new List<TriggerConditionType>() {
                TriggerConditionType.Numeric,
                TriggerConditionType.Numeric,
                TriggerConditionType.Numeric,
                TriggerConditionType.Numeric,
                TriggerConditionType.Boolean,
                TriggerConditionType.Boolean,
                TriggerConditionType.Numeric,
                TriggerConditionType.Numeric,
                TriggerConditionType.Numeric,
                TriggerConditionType.Numeric
            };
        }

        public override bool IsTriggered(bool preview)
        {
            MNKGauge gauge = Singletons.Get<IJobGauges>().Get<MNKGauge>();

            _dataSource.Chakra_Stacks = gauge.Chakra;

            _dataSource.Masters_Gauge_Opo_Count = 0;
            _dataSource.Masters_Gauge_Raptor_Count = 0;
            _dataSource.Masters_Gauge_Coeurl_Count = 0;
            foreach (var chakra in gauge.BeastChakra)
            {
                switch (chakra)
                {
                    case BeastChakra.OPOOPO:
                        _dataSource.Masters_Gauge_Opo_Count++;
                        break;
                    case BeastChakra.RAPTOR:
                        _dataSource.Masters_Gauge_Raptor_Count++;
                        break;
                    case BeastChakra.COEURL:
                        _dataSource.Masters_Gauge_Coeurl_Count++;
                        break;
                }
            }

            _dataSource.Solar_Nadi = (gauge.Nadi & Nadi.SOLAR) != 0;
            _dataSource.Lunar_Nadi = (gauge.Nadi & Nadi.LUNAR) != 0;
            _dataSource.Blitz_Timer = gauge.BlitzTimeRemaining / 1000f;
            _dataSource.Opoopo_Stacks = gauge.OpoOpoFury;
            _dataSource.Raptor_Stacks = gauge.RaptorFury;
            _dataSource.Coeurl_Stacks = gauge.CoeurlFury;

            IPlayerCharacter? player = Singletons.Get<IClientState>().LocalPlayer;
            _dataSource.Max_Chakra_Stacks = player?.StatusList.FirstOrDefault(s => s.StatusId is 1182 or 2174) != null ? 10 : 5;

            if (preview) { return true; }

            return
                EvaluateCondition(0, _dataSource.Chakra_Stacks) &&
                EvaluateCondition(1, _dataSource.Masters_Gauge_Opo_Count) &&
                EvaluateCondition(2, _dataSource.Masters_Gauge_Raptor_Count) &&
                EvaluateCondition(3, _dataSource.Masters_Gauge_Coeurl_Count) &&
                EvaluateCondition(4, _dataSource.Solar_Nadi) &&
                EvaluateCondition(5, _dataSource.Lunar_Nadi) &&
                EvaluateCondition(6, _dataSource.Blitz_Timer) &&
                EvaluateCondition(7, _dataSource.Opoopo_Stacks) &&
                EvaluateCondition(8, _dataSource.Raptor_Stacks) &&
                EvaluateCondition(9, _dataSource.Coeurl_Stacks);
        }
    }
}
