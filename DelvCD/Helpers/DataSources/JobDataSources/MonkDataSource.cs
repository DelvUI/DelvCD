namespace DelvCD.Helpers.DataSources.JobDataSources
{
    public class MonkDataSource : DataSource
    {
        public new static string GetDisplayName() => "Monk";

        public int Chakra_Stacks;
        public string Masters_Gauge_Chakra_1 = string.Empty;
        public string Masters_Gauge_Chakra_2 = string.Empty;
        public string Masters_Gauge_Chakra_3 = string.Empty;
        public bool Solar_Nadi;
        public bool Lunar_Nadi;
        public int OpoOpoFury;
        public int RaptorFury;
        public int CoeurlFury;

        public override float GetConditionValue(int index) => index switch
        {
            0 => Chakra_Stacks,
            _ => 0
        };

        public override float GetProgressValue(int index) => GetConditionValue(index);

        public override float GetMaxValue(int index) => index switch
        {
            0 => 5, // Chakra
            6 => 1, // Opo Fury
            7 => 1, // Raptor Fury
            8 => 2, // Coeurl Fury
            _ => 0
        };

        public MonkDataSource()
        {
            _conditionFieldNames = new() {
                nameof(Chakra_Stacks),
                nameof(Masters_Gauge_Chakra_1),
                nameof(Masters_Gauge_Chakra_2),
                nameof(Masters_Gauge_Chakra_3),
                nameof(Solar_Nadi),
                nameof(Lunar_Nadi),
                nameof(OpoOpoFury),
                nameof(RaptorFury),
                nameof(CoeurlFury),
            };

            _progressFieldNames = _conditionFieldNames;
        }
    }
}
