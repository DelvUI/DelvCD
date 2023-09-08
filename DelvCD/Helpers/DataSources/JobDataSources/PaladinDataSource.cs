namespace DelvCD.Helpers.DataSources.JobDataSources
{
    public class PaladinDataSource : DataSource
    {
        public new static string GetDisplayName() => "Paladin";

        public int Oath;

        public override float GetConditionValue(int index) => index switch
        {
            0 => Oath,
            _ => 0
        };

        public override float GetProgressValue(int index) => GetConditionValue(index);
        
        public override float GetMaxValue(int index) => index switch
        {
            0 => 100,
            _ => 0
        };

        public PaladinDataSource()
        {
            _conditionFieldNames = new() {
                nameof(Oath)
            };

            _progressFieldNames = _conditionFieldNames;
        }
    }
}
