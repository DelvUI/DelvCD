namespace DelvCD.Helpers.DataSources.JobDataSources
{
    public class RedMageDataSource : DataSource
    {
        public new static string GetDisplayName() => "Red Mage";

        public int White_Mana;
        public int Black_Mana;
        public int Mana_Stacks;

        public override float GetConditionValue(int index) => index switch
        {
            0 => White_Mana,
            1 => Black_Mana,
            2 => Mana_Stacks,
            _ => 0
        };

        public override float GetProgressValue(int index) => GetConditionValue(index);

        public override float GetMaxValue(int index) => index switch
        {
            0 => 100,
            1 => 100,
            2 => 3,
            _ => 0
        };

        public RedMageDataSource()
        {
            _conditionFieldNames = new() {
                nameof(White_Mana),
                nameof(Black_Mana),
                nameof(Mana_Stacks)
            };

            _progressFieldNames = _conditionFieldNames;
        }
    }
}
