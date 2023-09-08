namespace DelvCD.Helpers.DataSources.JobDataSources
{
    public class ScholarDataSource : DataSource
    {
        public new static string GetDisplayName() => "Scholar";

        public int Aetherflow_Stacks;      
        public int Fairie;
        public float Seraph_Timer;

        public override float GetConditionValue(int index) => index switch
        {
            0 => Aetherflow_Stacks,
            1 => Fairie,
            2 => Seraph_Timer,
            _ => 0
        };

        public override float GetProgressValue(int index) => GetConditionValue(index);

        public override float GetMaxValue(int index) => index switch
        {
            0 => 3,
            1 => 100,
            2 => 22,
            _ => 0
        };

        public ScholarDataSource()
        {
            _conditionFieldNames = new() {
                nameof(Aetherflow_Stacks),
                nameof(Fairie),
                nameof(Seraph_Timer)
            };

            _progressFieldNames = _conditionFieldNames;
        }
    }
}
