namespace DelvCD.Helpers.DataSources
{
    public class CooldownDataSource : DataSource
    {
        public new static string GetDisplayName() => "Cooldown";

        public string? Name = string.Empty;
        public float Cooldown_Timer;
        public float Max_Cooldown_Timer;
        public int Cooldown_Stacks;
        public int Max_Cooldown_Stacks;

        public override float GetConditionValue(int index) => index switch
        {
            0 => Cooldown_Timer,
            1 => Cooldown_Stacks,
            2 => Max_Cooldown_Stacks,
            _ => 0
        };

        public override float GetProgressValue(int index) => index switch
        {
            0 => Cooldown_Timer,
            1 => Cooldown_Stacks,
            _ => 0
        };

        public override float GetMaxValue(int index) => index switch
        {
            0 => Max_Cooldown_Timer,
            1 => Max_Cooldown_Stacks,
            _ => 0
        };

        public CooldownDataSource()
        {
            _conditionFieldNames = new() {
                nameof(Cooldown_Timer),
                nameof(Cooldown_Stacks),
                nameof(Max_Cooldown_Stacks)
            };

            _progressFieldNames = new()
            {
                nameof(Cooldown_Timer),
                nameof(Cooldown_Stacks)
            };
        }
    }
}
