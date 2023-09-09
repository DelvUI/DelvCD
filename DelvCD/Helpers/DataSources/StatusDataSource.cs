namespace DelvCD.Helpers.DataSources
{
    public class StatusDataSource : DataSource
    {
        public new static string GetDisplayName() => "Status";

        public float Status_Timer;
        public float Max_Status_Timer;
        public int Status_Stacks;
        public int Max_Status_Stacks;

        public override float GetConditionValue(int index) => index switch
        {
            0 => Status_Timer,
            1 => Status_Stacks,
            2 => Max_Status_Stacks,
            _ => 0
        };

        public override float GetProgressValue(int index) => index switch
        {
            0 => Status_Timer,
            1 => Status_Stacks,
            _ => 0
        };

        public override float GetMaxValue(int index) => index switch
        {
            0 => Max_Status_Timer,
            1 => Max_Status_Stacks,
            _ => 0
        };

        public StatusDataSource()
        {
            _conditionFieldNames = new() {
                nameof(Status_Timer),
                nameof(Status_Stacks),
                nameof(Max_Status_Stacks)
            };

            _progressFieldNames = new()
            {
                nameof(Status_Timer),
                nameof(Status_Stacks)
            };
        }
    }
}
