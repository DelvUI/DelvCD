using FFXIVClientStructs.FFXIV.Client.Game.UI;
using Lumina.Excel.GeneratedSheets;

namespace DelvCD.Helpers.DataSources.JobDataSources
{
    public class AstrologianDataSource : DataSource
    {
        public new static string GetDisplayName() => "Astrologian";

        public string Card = string.Empty;
        public string Crown_Card = string.Empty;
        public bool Astrosign_Solar;
        public bool Astrosign_Lunar;
        public bool Astrosign_Celestial;

        public override float GetConditionValue(int index) => 0;

        public override float GetProgressValue(int index) => 0;
        public override float GetMaxValue(int index) => 0;
    }
}
