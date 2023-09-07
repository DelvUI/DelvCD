namespace DelvCD.Helpers.DataSources
{
    public class CooldownDataSource : DataSource
    {
        public float Value;
        public float MaxValue;
        public int Stacks;
        public int MaxStacks;

        public new static string GetFriendlyName() { return "Cooldowns and Status Triggers"; }

        public override float ProgressValue
        {
            get => Value;
            set => Value = value;
        }

        public override float ProgressMaxValue
        {
            get => MaxValue;
            set => MaxValue = value;
        }
        public CooldownDataSource()
        {
            SetConditionFields(
                nameof(Value),
                nameof(Stacks),
                nameof(MaxStacks)
            );
        }
    }
}
