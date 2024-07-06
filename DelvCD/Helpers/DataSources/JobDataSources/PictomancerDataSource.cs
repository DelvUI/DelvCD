using Dalamud.Game.ClientState.JobGauge.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DelvCD.Helpers.DataSources.JobDataSources
{
    internal class PictomancerDataSource : DataSource
    {
        public new static string GetDisplayName() => "Pictomancer";

        public int Pallete;
        public int Paint;
        public bool Creature_Motif_Drawn;
        public bool Weapon_Motif_Drawn;
        public bool Landscape_Motif_Drawn;
        public string Creature_Motif = string.Empty;
        public string Creature_Canvas = string.Empty;
        public string Creature_Portrait = string.Empty;


        public override float GetConditionValue(int index) => index switch
        {
            0 => Pallete,
            1 => Paint,
            _ => 0
        };

        public override float GetProgressValue(int index) => GetConditionValue(index);

        public override float GetMaxValue(int index) => index switch
        {
            0 => 100,
            1 => 5,
            _ => 0
        };

        public PictomancerDataSource()
        {
            _conditionFieldNames = new() {
                nameof(Pallete),
                nameof(Paint)
            };

            _progressFieldNames = _conditionFieldNames;
        }
    }
}
