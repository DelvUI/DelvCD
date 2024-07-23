namespace DelvCD.Helpers.DataSources
{
    public class ItemCooldownDataSource : DataSource
    {
        public new static string GetDisplayName() => "Item Cooldown";

        public float Item_Cooldown_Timer;
        public float Max_Item_Cooldown_Timer;
        public int Item_Cooldown_Stacks;
        public int Max_Item_Cooldown_Stacks;
        public string? Item_Keybind = string.Empty;
        public string? Item_Keybind_Formatted = string.Empty;

        public override float GetConditionValue(int index) => index switch
        {
            0 => Item_Cooldown_Timer,
            1 => Item_Cooldown_Stacks,
            2 => Max_Item_Cooldown_Stacks,
            _ => 0
        };

        public override float GetProgressValue(int index) => index switch
        {
            0 => Item_Cooldown_Timer,
            1 => Item_Cooldown_Stacks,
            _ => 0
        };

        public override float GetMaxValue(int index) => index switch
        {
            0 => Max_Item_Cooldown_Timer,
            1 => Max_Item_Cooldown_Stacks,
            _ => 0
        };

        public ItemCooldownDataSource()
        {
            _conditionFieldNames = new() {
                nameof(Item_Cooldown_Timer),
                nameof(Item_Cooldown_Stacks),
                nameof(Max_Item_Cooldown_Stacks)
            };

            _progressFieldNames = new()
            {
                nameof(Item_Cooldown_Timer),
                nameof(Item_Cooldown_Stacks)
            };
        }
    }
}
