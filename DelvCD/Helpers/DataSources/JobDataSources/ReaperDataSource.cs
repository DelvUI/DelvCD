namespace DelvCD.Helpers.DataSources.JobDataSources
{
    public class ReaperDataSource : DataSource
    {
        public new static string DisplayName => "Reaper";

        public int Soul;
        public int Shroud;
        public float Enshroud_Timer;
        public int Lemure_Shroud_Stacks;
        public int Void_Shroud_Stacks;

        public override float ProgressValue
        {
            get => Enshroud_Timer;
            set => Enshroud_Timer = value;
        }

        public override float ProgressMaxValue => 30;

        public override float GetConditionValue(int index) => index switch
        {
            0 => Soul,
            1 => Shroud,
            2 => Enshroud_Timer,
            3 => Lemure_Shroud_Stacks,
            4 => Void_Shroud_Stacks,
            _ => 0
        };

        public ReaperDataSource()
        {
            _conditionFieldNames = new() {
                nameof(Soul),
                nameof(Shroud),
                nameof(Enshroud_Timer),
                nameof(Lemure_Shroud_Stacks),
                nameof(Void_Shroud_Stacks)
            };
        }
    }
}
