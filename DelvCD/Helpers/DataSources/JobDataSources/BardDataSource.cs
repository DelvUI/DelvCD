namespace DelvCD.Helpers.DataSources.JobDataSources
{
    public class BardDataSource : DataSource
    {
        public new static string GetDisplayName() => "Bard";

        public string Active_Song = string.Empty;
        public string Last_Active_Song = string.Empty;
        public float Song_Timer;
        public float Repertoire_Stacks;
        public int Soul_Voice;

        public override float ProgressValue
        {
            get => Song_Timer;
            set => Song_Timer = value;
        }

        public override float ProgressMaxValue => 45;

        public override float GetConditionValue(int index) => index switch
        {
            0 => Song_Timer,
            1 => Repertoire_Stacks,
            2 => Soul_Voice,
            _ => 0
        };

        public BardDataSource()
        {
            _conditionFieldNames = new() {
                nameof(Song_Timer),
                nameof(Repertoire_Stacks),
                nameof(Soul_Voice)
            };
        }
    }
}
