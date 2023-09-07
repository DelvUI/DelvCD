namespace DelvCD.Helpers.DataSources.JobDataSources
{
    public class NinjaDataSource : DataSource
    {
        public new static string GetDisplayName() => "Ninja";

        public float Huton_Timer;
        public int Ninki;

        public override float ProgressValue
        {
            get => Huton_Timer;
            set => Huton_Timer = value;
        }

        public override float ProgressMaxValue => 60;

        public override float GetConditionValue(int index) => index switch
        {
            0 => Huton_Timer,
            1 => Ninki,
            _ => 0
        };

        public NinjaDataSource()
        {
            _conditionFieldNames = new() {
                nameof(Huton_Timer),
                nameof(Ninki)
            };
        }
    }
}
