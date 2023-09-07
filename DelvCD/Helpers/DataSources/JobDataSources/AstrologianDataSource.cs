using Lumina.Excel.GeneratedSheets;

namespace DelvCD.Helpers.DataSources.JobDataSources
{
    public class AstrologianDataSource : DataSource
    {
        public new static string DisplayName => "Astrologian";

        public string Card = string.Empty;
        public string Crown_Card = string.Empty;
        public bool Astrosign_Solar;
        public bool Astrosign_Lunar;
        public bool Astrosign_Celestial;

        public override float ProgressValue
        {
            get => 0; 
            set { }
        }

        public override float ProgressMaxValue => 1;

        public override float GetConditionValue(int index) => 0;
    }
}
