namespace DelvCD.Helpers.DataSources.JobDataSources
{
    public class MachinistDataSource : DataSource
    {
        public new static string GetDisplayName() => "Machinist";

        public int Heat;
        public bool Overheat;
        public float Overheat_Timer;
        public int Battery;
        public bool Summon;
        public float Summon_Timer;

        public override float GetConditionValue(int index) => index switch
        {
            0 => Heat,
            1 => Overheat_Timer,
            2 => Battery,
            3 => Summon_Timer,
            _ => 0
        };

        public override float GetProgressValue(int index) => GetConditionValue(index);

        public override float GetMaxValue(int index) => index switch
        {
            0 => 100,
            1 => 10,
            2 => 100,
            3 => 12,
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

            _progressFieldNames = _conditionFieldNames;
        }
    }
}
