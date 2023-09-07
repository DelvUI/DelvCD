namespace DelvCD.Helpers.DataSources.JobDataSources
{
    public class SamuraiDataSource : DataSource
    {
        public new static string DisplayName => "Samurai";

        public bool Setsu;
        public bool Getsu;
        public bool Ka;
        public int Kenki;
        public int Meditation_Stacks;

        public override float ProgressValue
        {
            get => Kenki;
            set => Kenki = (int)value;
        }

        public override float ProgressMaxValue => 100;

        public override float GetConditionValue(int index) => index switch
        {
            0 => Kenki,
            1 => Meditation_Stacks,
            _ => 0
        };

        public SamuraiDataSource()
        {
            _conditionFieldNames = new() {
                nameof(Kenki),
                nameof(Meditation_Stacks)
            };
        }
    }
}
