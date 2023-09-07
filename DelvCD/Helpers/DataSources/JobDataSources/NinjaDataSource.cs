namespace DelvCD.Helpers.DataSources.JobDataSources
{
    public class NinjaDataSource : DataSource
    {
        public new static string GetDisplayName() => "Ninja";

        public float Huton_Timer;
        public int Ninki;


        public override float GetConditionValue(int index) => index switch
        {
            0 => Huton_Timer,
            1 => Ninki,
            _ => 0
        };

        public override float GetProgressValue(int index) => GetConditionValue(index);

        public override float GetMaxValue(int index) => index switch
        {
            0 => 60,
            1 => 100,
            _ => 0
        };

        public NinjaDataSource()
        {
            _conditionFieldNames = new() {
                nameof(Huton_Timer),
                nameof(Ninki)
            };

            _progressFieldNames = _conditionFieldNames;
        }
    }
}
