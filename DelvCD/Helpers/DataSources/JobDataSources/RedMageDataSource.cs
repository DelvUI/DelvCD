namespace DelvCD.Helpers.DataSources.JobDataSources
{
    public class RedMageDataSource : DataSource
    {
        public new static string GetDisplayName() => "Red Mage";

        public int White_Mana;
        public int Black_Mana;
        public int Mana_Stacks;

        public override float ProgressValue
        {
            get => Mana_Stacks;
            set => Mana_Stacks = (int)value;
        }

        public override float ProgressMaxValue => 3;

        public override float GetConditionValue(int index) => index switch
        {
            0 => White_Mana,
            1 => Black_Mana,
            2 => Mana_Stacks,
            _ => 0
        };

        public RedMageDataSource()
        {
            _conditionFieldNames = new() {
                nameof(White_Mana),
                nameof(Black_Mana),
                nameof(Mana_Stacks)
            };
        }
    }
}
