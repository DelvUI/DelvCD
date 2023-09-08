namespace DelvCD.Helpers.DataSources
{
    public enum CooldownDataSourceType
    {
        Action = 0,
        Status = 1,
        Item = 2
    }

    public class CooldownDataSource : DataSource
    {
        public new static string GetDisplayName() => "Cooldown and Status";

        public float Value;
        public float MaxValue;
        public int Stacks;
        public int MaxStacks;

        public CooldownDataSourceType Type;

        public override float GetConditionValue(int index) => index switch
        {
            0 => Value,
            1 => Stacks,
            2 => MaxStacks,
            _ => 0
        };

        public override float GetProgressValue(int index) => index switch
        {
            0 => Value,
            1 => Stacks,
            _ => 0
        };

        public override float GetMaxValue(int index) => index switch
        {
            0 => MaxValue,
            1 => MaxStacks,
            _ => 0
        };

        public CooldownDataSource()
        {
            _conditionFieldNames = new() {
                nameof(Value),
                nameof(Stacks),
                nameof(MaxStacks)
            };

            _progressFieldNames = new()
            {
                nameof(Value),
                nameof(Stacks)
            };
        }
    }
}
