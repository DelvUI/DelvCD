using Dalamud.Game.ClientState.JobGauge.Enums;
using Dalamud.Game.ClientState.JobGauge.Types;
using Dalamud.Plugin.Services;
using DelvCD.Helpers;
using DelvCD.Helpers.DataSources;
using DelvCD.Helpers.DataSources.JobDataSources;
using FFXIVClientStructs.FFXIV.Client.Game;
using System.Collections.Generic;

namespace DelvCD.Config.JobGauges
{
    public class AstrologianJobGauge : JobGauge
    {
        public AstrologianJobGauge(string rawData) : base(rawData)
        {
        }

        public override Job Job => Job.AST;
        private AstrologianDataSource _dataSource = new();
        public override DataSource DataSource => _dataSource;

        protected override void InitializeConditions()
        {
            _names = new List<string>() {
                "Card1",
                "Card2",
                "Card3",
                "Crown Card"
            };

            _types = new List<TriggerConditionType>() {
                TriggerConditionType.Combo,
                TriggerConditionType.Combo,
                TriggerConditionType.Combo,
                TriggerConditionType.Combo
            };

            _comboOptions = new Dictionary<int, string[]>()
            {
                [0] = new string[] { "None", "Balance", "Spear" },
                [1] = new string[] { "None", "Arrow", "Bole" },
                [2] = new string[] { "None", "Spire", "Ewer" },
                [3] = new string[] { "None", "Lord of Crowns", "Lady of Crowns" }
            };
        }

        public override bool IsTriggered(bool preview)
        {
            ASTGauge gauge = Singletons.Get<IJobGauges>().Get<ASTGauge>();

            int card1 = GetCard1Index();
            int card2 = GetCard2Index();
            int card3 = GetCard3Index();
            int minorArcana = GetCrownCardIndex();

            _dataSource.Card1 = _comboOptions[0][card1];
            _dataSource.Card2 = _comboOptions[1][card2];
            _dataSource.Card3 = _comboOptions[2][card3];
            _dataSource.Crown_Card = _comboOptions[3][minorArcana];

            if (preview) { return true; }

            return
                EvaluateCondition(0, card1) &&
                EvaluateCondition(1, card2) &&
                EvaluateCondition(2, card3) &&
                EvaluateCondition(3, minorArcana);
        }

        private unsafe int GetCard1Index()
        {
            uint play1 = ActionManager.Instance()->GetAdjustedActionId(37019);
            bool theBalance = play1 == 37023;
            bool theSpear = play1 == 37026;

            if (theBalance) { return 1; }
            if (theSpear) { return 2; }

            return 0;
        }

        private unsafe int GetCard2Index()
        {
            uint play2 = ActionManager.Instance()->GetAdjustedActionId(37020);
            bool theArrow = play2 == 37024;
            bool theBole = play2 == 37027;

            if (theArrow) { return 1; }
            if (theBole) { return 2; }

            return 0;
        }

        private unsafe int GetCard3Index()
        {
            uint play3 = ActionManager.Instance()->GetAdjustedActionId(37021);
            bool theSpire = play3 == 37025;
            bool theEwer = play3 == 37028;

            if (theSpire) { return 1; }
            if (theEwer) { return 2; }

            return 0;
        }

        private unsafe int GetCrownCardIndex()
        {
            uint minorArcana = ActionManager.Instance()->GetAdjustedActionId(37022);
            bool lord = minorArcana == 7444;
            bool lady = minorArcana == 7445;

            if (lord) { return 1; }
            if (lady) { return 2; }

            return 0;
        }
    }
}
