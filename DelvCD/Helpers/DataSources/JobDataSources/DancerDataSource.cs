namespace DelvCD.Helpers.DataSources.JobDataSources
{
    public class DancerDataSource : DataSource
    {
        public new static string GetDisplayName() => "Dancer";

        public int Feather_Stacks;
        public int Esprit;
        public bool Dancing;
        public int Completed_Steps;

        public override float GetConditionValue(int index) => index switch
        {
            0 => Feather_Stacks,
            1 => Esprit,
            2 => Completed_Steps,
            _ => 0
        };

        public override float GetProgressValue(int index) => GetConditionValue(index);

        public override float GetMaxValue(int index) => index switch
        {
            0 => 4,
            1 => 100,
            2 => 4,
            _ => 0
        };

        public DancerDataSource()
        {
            _conditionFieldNames = new() {
                nameof(Feather_Stacks),
                nameof(Esprit),
                nameof(Completed_Steps)
            };

            _progressFieldNames = _conditionFieldNames;
        }
    }
}
