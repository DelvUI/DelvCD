namespace DelvCD.Helpers.DataSources.JobDataSources
{
    public class BardDataSource : DataSource
    {
        public new static string GetDisplayName() => "Bard";

        public string Active_Song = string.Empty;
        public string Last_Active_Song = string.Empty;
        public float Song_Timer;
        public int Repertoire_Stacks;
        public int Max_Repertoire_Stacks;
        public int Soul_Voice;

        public override float GetConditionValue(int index) => index switch
        {
            0 => Song_Timer,
            1 => Repertoire_Stacks,
            2 => Max_Repertoire_Stacks,
            3 => Soul_Voice,
            _ => 0
        };

        public override float GetProgressValue(int index) => index switch
        {
            0 => Song_Timer,
            1 => Repertoire_Stacks,
            2 => Soul_Voice,
            _ => 0
        };

        public override float GetMaxValue(int index) => index switch
        {
            0 => 45,
            1 => Max_Repertoire_Stacks,
            2 => 100,
            _ => 0
        };

        public BardDataSource()
        {
            _conditionFieldNames = new() {
                nameof(Song_Timer),
                nameof(Repertoire_Stacks),
                nameof(Max_Repertoire_Stacks),
                nameof(Soul_Voice)
            };

            _progressFieldNames = new() {
                nameof(Song_Timer),
                nameof(Repertoire_Stacks),
                nameof(Soul_Voice)
            };
        }
    }
}
