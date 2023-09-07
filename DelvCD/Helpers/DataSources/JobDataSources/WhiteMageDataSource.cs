namespace DelvCD.Helpers.DataSources.JobDataSources
{
    public class WhiteMageDataSource : DataSource
    {
        public new static string DisplayName => "White Mage";

        public float Lily_Timer;
        public int Lily_Stacks;
        public int Blood_Lily_Stacks;

        public override float ProgressValue
        {
            get => Lily_Timer;
            set => Lily_Timer = value;
        }

        public override float ProgressMaxValue => 20;

        public override float GetConditionValue(int index) => index switch
        {
            0 => Lily_Timer,
            1 => Lily_Stacks,
            2 => Blood_Lily_Stacks,
            _ => 0
        };

        public WhiteMageDataSource()
        {
            _conditionFieldNames = new() {
                nameof(Lily_Timer),
                nameof(Lily_Stacks),
                nameof(Blood_Lily_Stacks)
            };
        }
    }
}
