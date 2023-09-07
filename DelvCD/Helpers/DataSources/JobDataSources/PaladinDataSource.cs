namespace DelvCD.Helpers.DataSources.JobDataSources
{
    public class PaladinDataSource : DataSource
    {
        public new static string DisplayName => "Paladin";

        public int Oath;

        public override float ProgressValue
        {
            get => Oath;
            set => Oath = (int)value;
        }

        public override float ProgressMaxValue => 100;

        public override float GetConditionValue(int index) => index switch
        {
            0 => Oath,
            _ => 0
        };

        public PaladinDataSource()
        {
            _conditionFieldNames = new() {
                nameof(Oath)
            };
        }
    }
}
