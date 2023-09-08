namespace DelvCD.Helpers.DataSources.JobDataSources
{
    public class GunbreakerDataSource : DataSource
    {
        public new static string GetDisplayName() => "Gunbreaker";

        public int Cartridges;

        public override float GetConditionValue(int index) => index switch
        {
            0 => Cartridges,
            _ => 0
        };

        public override float GetProgressValue(int index) => GetConditionValue(index);

        public override float GetMaxValue(int index) => index switch
        {
            0 => 3,
            _ => 0
        };

        public GunbreakerDataSource()
        {
            _conditionFieldNames = new() {
                nameof(Cartridges)
            };

            _progressFieldNames = _conditionFieldNames;
        }
    }
}
