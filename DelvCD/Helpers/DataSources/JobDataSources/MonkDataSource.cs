namespace DelvCD.Helpers.DataSources.JobDataSources
{
    public class MonkDataSource : DataSource
    {
        public new static string GetDisplayName() => "Monk";

        public int Chakra_Stacks;
        public int Max_Chakra_Stacks;
        public string Masters_Gauge_Chakra_1 = string.Empty;
        public string Masters_Gauge_Chakra_2 = string.Empty;
        public string Masters_Gauge_Chakra_3 = string.Empty;
        public bool Solar_Nadi;
        public bool Lunar_Nadi;
        public float Blitz_Timer;
        public int Opoopo_Stacks;
        public int Raptor_Stacks;
        public int Coeurl_Stacks;

        public override float GetConditionValue(int index) => index switch
        {
            0 => Chakra_Stacks,
            1 => Max_Chakra_Stacks,
            2 => Blitz_Timer,
            3 => Opoopo_Stacks,
            4 => Raptor_Stacks,
            5 => Coeurl_Stacks,
            _ => 0
        };

        public override float GetProgressValue(int index) => index switch
        {
            0 => Chakra_Stacks,
            1 => Blitz_Timer,
            2 => Opoopo_Stacks,
            3 => Raptor_Stacks,
            4 => Coeurl_Stacks,
            _ => 0
        };

        public override float GetMaxValue(int index) => index switch
        {
            0 => Max_Chakra_Stacks,
            1 => 20,
            2 => 1,
            3 => 1,
            4 => 2,
            _ => 0
        };

        public MonkDataSource()
        {
            _conditionFieldNames = new() {
                nameof(Chakra_Stacks),
                nameof(Max_Chakra_Stacks),
                nameof(Blitz_Timer),
                nameof(Opoopo_Stacks),
                nameof(Raptor_Stacks),
                nameof(Coeurl_Stacks),
            };

            _progressFieldNames = new() {
                nameof(Chakra_Stacks),
                nameof(Blitz_Timer),
                nameof(Opoopo_Stacks),
                nameof(Raptor_Stacks),
                nameof(Coeurl_Stacks),
            };
        }
    }
}
