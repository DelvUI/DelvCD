namespace DelvCD.Helpers.DataSources.JobDataSources
{
    public class WhiteMageDataSource : DataSource
    {
        public new static string GetDisplayName() => "White Mage";

        public float Lily_Timer;
        public int Lily_Stacks;
        public int Blood_Lily_Stacks;

        public override float GetConditionValue(int index) => index switch
        {
            0 => Lily_Timer,
            1 => Lily_Stacks,
            2 => Blood_Lily_Stacks,
            _ => 0
        };

        public override float GetProgressValue(int index) => GetConditionValue(index);

        public override float GetMaxValue(int index) => index switch
        {
            0 => 20,
            1 => 3,
            2 => 3,
            _ => 0
        };

        public WhiteMageDataSource()
        {
            _conditionFieldNames = new() {
                nameof(Lily_Timer),
                nameof(Lily_Stacks),
                nameof(Blood_Lily_Stacks)
            };

            _progressFieldNames = _conditionFieldNames;
        }
    }
}
