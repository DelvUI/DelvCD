namespace DelvCD.Helpers.DataSources.JobDataSources
{
    public class DragoonDataSource : DataSource
    {
        public new static string GetDisplayName() => "Dragoon";

        public bool Life_Of_The_Dragon;
        public float Life_Of_The_Dragon_Timer;
        public int First_Broods_Gaze_Stacks;
        public int Firstminds_Focus_Stacks;

        public override float GetConditionValue(int index) => index switch
        {
            0 => Life_Of_The_Dragon_Timer,
            1 => First_Broods_Gaze_Stacks,
            2 => Firstminds_Focus_Stacks,
            _ => 0
        };

        public override float GetProgressValue(int index) => GetConditionValue(index);

        public override float GetMaxValue(int index) => index switch
        {
            0 => 30,
            1 => 2,
            2 => 2,
            _ => 0
        };

        public DragoonDataSource()
        {
            _conditionFieldNames = new() {
                nameof(Life_Of_The_Dragon_Timer),
                nameof(First_Broods_Gaze_Stacks),
                nameof(Firstminds_Focus_Stacks)
            };

            _progressFieldNames = _conditionFieldNames;
        }
    }
}
