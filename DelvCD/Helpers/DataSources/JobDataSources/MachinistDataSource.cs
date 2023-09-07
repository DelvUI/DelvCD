namespace DelvCD.Helpers.DataSources.JobDataSources
{
    public class MachinistDataSource : DataSource
    {
        public new static string DisplayName => "Machinist";

        public int Heat;
        public bool Overheat;
        public float Overheat_Timer;
        public int Battery;
        public bool Summon;
        public float Summon_Timer;

        public override float ProgressValue
        {
            get => Heat;
            set => Heat = (int)value;
        }

        public override float ProgressMaxValue => 100;

        public override float GetConditionValue(int index) => index switch
        {
            0 => Heat,
            1 => Overheat_Timer,
            2 => Battery,
            3 => Summon_Timer,
            _ => 0
        };

        public MachinistDataSource()
        {
            _conditionFieldNames = new() {
                nameof(Heat),
                nameof(Overheat_Timer),
                nameof(Battery),
                nameof(Summon_Timer)
            };
        }
    }
}
