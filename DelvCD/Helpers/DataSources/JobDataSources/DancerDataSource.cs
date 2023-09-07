namespace DelvCD.Helpers.DataSources.JobDataSources
{
    public class DancerDataSource : DataSource
    {
        public new static string DisplayName => "Dancer";

        public int Feather_Stacks;
        public int Esprit;
        public bool Dancing;

        public override float ProgressValue
        {
            get => Esprit;
            set => Esprit = (int)value;
        }

        public override float ProgressMaxValue => 100;

        public override float GetConditionValue(int index) => index switch
        {
            0 => Feather_Stacks,
            1 => Esprit,
            _ => 0
        };

        public DancerDataSource()
        {
            _conditionFieldNames = new() {
                nameof(Feather_Stacks),
                nameof(Esprit)
            };
        }
    }
}
