namespace DelvCD.Helpers.DataSources
{
    public class CooldownDataSource : DataSource
    {
        public new static string GetDisplayName() => "Cooldown and Status";

        public float Value;
        public float MaxValue;
        public int Stacks;
        public int MaxStacks;

        public override float ProgressValue
        {
            get => Value;
            set => Value = value;
        }

        public override float ProgressMaxValue => MaxValue;

        public override float GetConditionValue(int index) => index switch
        {
            0 => Value,
            1 => Stacks,
            2 => MaxStacks,
            _ => 0
        };

        public CooldownDataSource()
        {
            _conditionFieldNames = new() {
                nameof(Value),
                nameof(Stacks),
                nameof(MaxStacks)
            };
        }
    }
}
