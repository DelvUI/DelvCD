namespace DelvCD.Helpers.DataSources.JobDataSources
{
    public class SageDataSource : DataSource
    {
        public new static string GetDisplayName() => "Sage";

        public bool Eukrasia;
        public float Addersgall_Timer;
        public int Addersgall_Stacks;
        public int Addersting_Stacks;

        public override float ProgressValue
        {
            get => Addersgall_Timer;
            set => Addersgall_Timer = value;
        }

        public override float ProgressMaxValue => 20;

        public override float GetConditionValue(int index) => index switch
        {
            0 => Addersgall_Timer,
            1 => Addersgall_Stacks,
            2 => Addersting_Stacks,
            _ => 0
        };

        public SageDataSource()
        {
            _conditionFieldNames = new() {
                nameof(Addersgall_Timer),
                nameof(Addersgall_Stacks),
                nameof(Addersting_Stacks)
            };
        }
    }
}
