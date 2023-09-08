namespace DelvCD.Helpers.DataSources.JobDataSources
{
    public class SamuraiDataSource : DataSource
    {
        public new static string GetDisplayName() => "Samurai";

        public bool Setsu;
        public bool Getsu;
        public bool Ka;
        public int Kenki;
        public int Meditation_Stacks;


        public override float GetConditionValue(int index) => index switch
        {
            0 => Kenki,
            1 => Meditation_Stacks,
            _ => 0
        };

        public override float GetProgressValue(int index) => GetConditionValue(index);

        public override float GetMaxValue(int index) => index switch
        {
            0 => 100,
            1 => 5,
            _ => 0
        };

        public SamuraiDataSource()
        {
            _conditionFieldNames = new() {
                nameof(Kenki),
                nameof(Meditation_Stacks)
            };

            _progressFieldNames = _conditionFieldNames;
        }
    }
}
