namespace DelvCD.Helpers.DataSources.JobDataSources
{
    public class DarkKnightDataSource : DataSource
    {
        public new static string GetDisplayName() => "Dark Knight";

        public int Blood;
        public float Darkside_Timer;
        public float Shadow_Timer;
        public bool Dark_Arts;

        public override float ProgressValue
        {
            get => Blood;
            set => Blood = (int)value;
        }

        public override float ProgressMaxValue => 100;

        public override float GetConditionValue(int index) => index switch
        {
            0 => Blood,
            1 => Darkside_Timer,
            2 => Shadow_Timer,
            _ => 0
        };

        public DarkKnightDataSource()
        {
            _conditionFieldNames = new() {
                nameof(Blood),
                nameof(Darkside_Timer),
                nameof(Shadow_Timer)
            };
        }
    }
}
