using FFXIVClientStructs.FFXIV.Client.Game.UI;
using Lumina.Excel.Sheets;

namespace DelvCD.Helpers.DataSources.JobDataSources
{
    public class AstrologianDataSource : DataSource
    {
        public new static string GetDisplayName() => "Astrologian";

        public string Card1 = string.Empty;
        public string Card2 = string.Empty;
        public string Card3 = string.Empty;
        public string Crown_Card = string.Empty;

        public override float GetConditionValue(int index) => 0;

        public override float GetProgressValue(int index) => 0;
        public override float GetMaxValue(int index) => 0;
    }
}
