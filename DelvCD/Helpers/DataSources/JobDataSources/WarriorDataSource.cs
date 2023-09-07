namespace DelvCD.Helpers.DataSources.JobDataSources
{
    public class WarriorDataSource : DataSource
    {
        public new static string GetDisplayName() => "Warrior";

        public int Wrath;

        public override float ProgressValue
        {
            get => Wrath;
            set => Wrath = (int)value;
        }

        public override float ProgressMaxValue => 100;

        public override float GetConditionValue(int index) => index switch
        {
            0 => Wrath,
            _ => 0
        };

        public WarriorDataSource()
        {
            _conditionFieldNames = new() {
                nameof(Wrath)
            };
        }
    }
}
