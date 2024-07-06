namespace DelvCD.Helpers.DataSources.JobDataSources
{
    public class ViperDataSource : DataSource
    {
        public new static string GetDisplayName() => "Viper";

        public int SerpentOffering;
        public int AnguineTribute;
        public int DreadComboId;
        public int RattlingCoilStacks;

        public override float GetConditionValue(int index) => index switch
        {
            0 => SerpentOffering,
            1 => AnguineTribute,
            2 => RattlingCoilStacks,
            3 => DreadComboId,
            _ => 0
        };

        public override float GetProgressValue(int index) => GetConditionValue(index);

        public override float GetMaxValue(int index) => index switch
        {
            0 => 100,
            1 => 5,
            2 => 3,
            3 => 0, // Not valid for progress
            _ => 0
        };

        public ViperDataSource()
        {
            _conditionFieldNames = new() {
                nameof(SerpentOffering),
                nameof(AnguineTribute),
                nameof(RattlingCoilStacks),
                nameof(DreadComboId),
            };

            _progressFieldNames = _conditionFieldNames;
        }
    }
}
