namespace DelvCD.Helpers.DataSources.JobDataSources
{
    public class WarriorDataSource : DataSource
    {
        public new static string GetDisplayName() => "Warrior";

        public int Wrath;

        public override float GetConditionValue(int index) => index switch
        {
            0 => Wrath,
            _ => 0
        };

        public override float GetProgressValue(int index) => GetConditionValue(index);

        public override float GetMaxValue(int index) => index switch
        {
            0 => 100,
            _ => 0
        };

        public WarriorDataSource()
        {
            _conditionFieldNames = new() {
                nameof(Wrath)
            };

            _progressFieldNames = _conditionFieldNames;
        }
    }
}
