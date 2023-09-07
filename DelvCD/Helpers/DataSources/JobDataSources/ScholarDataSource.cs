namespace DelvCD.Helpers.DataSources.JobDataSources
{
    public class ScholarDataSource : DataSource
    {
        public new static string GetDisplayName() => "Scholar";

        public int Aetherflow_Stacks;      
        public int Fairie;
        public float Seraph_Timer;

        public override float ProgressValue
        {
            get => Seraph_Timer;
            set => Seraph_Timer = value;
        }

        public override float ProgressMaxValue => 22;

        public override float GetConditionValue(int index) => index switch
        {
            0 => Aetherflow_Stacks,
            1 => Fairie,
            2 => Seraph_Timer,
            _ => 0
        };

        public ScholarDataSource()
        {
            _conditionFieldNames = new() {
                nameof(Aetherflow_Stacks),
                nameof(Fairie),
                nameof(Seraph_Timer)
            };
        }
    }
}
