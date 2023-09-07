namespace DelvCD.Helpers.DataSources.JobDataSources
{
    public class GunbreakerDataSource : DataSource
    {
        public new static string DisplayName => "Gunbreaker";

        public int Cartridges;

        public override float ProgressValue
        {
            get => Cartridges;
            set => Cartridges = (int)value;
        }

        public override float ProgressMaxValue => 3;

        public override float GetConditionValue(int index) => index switch
        {
            0 => Cartridges,
            _ => 0
        };

        public GunbreakerDataSource()
        {
            _conditionFieldNames = new() {
                nameof(Cartridges)
            };
        }
    }
}
