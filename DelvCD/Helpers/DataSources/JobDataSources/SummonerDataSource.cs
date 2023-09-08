namespace DelvCD.Helpers.DataSources.JobDataSources
{
    public class SummonerDataSource : DataSource
    {
        public new static string GetDisplayName() => "Summoner";

        public int Aetherflow_Stacks;
        public string Next_Summon = string.Empty;
        public string Active_Summon = string.Empty;
        public float Summon_Timer;
        public string Active_Attunement = string.Empty;
        public float Attunement_Timer;
        public int Attunement_Stacks;
        public int Max_Attunement_Stacks;

        public override float GetConditionValue(int index) => index switch
        {
            0 => Aetherflow_Stacks,
            1 => Summon_Timer,
            2 => Attunement_Timer,
            3 => Attunement_Stacks,
            4 => Max_Attunement_Stacks,
            _ => 0
        };

        public override float GetProgressValue(int index) => index switch
        {
            0 => Aetherflow_Stacks,
            1 => Summon_Timer,
            2 => Attunement_Timer,
            3 => Attunement_Stacks,
            _ => 0
        };

        public override float GetMaxValue(int index) => index switch
        {
            0 => 2,
            1 => 15,
            2 => 30,
            3 => Max_Attunement_Stacks,
            _ => 0
        };

        public SummonerDataSource()
        {
            _conditionFieldNames = new() {
                nameof(Aetherflow_Stacks),
                nameof(Summon_Timer),
                nameof(Attunement_Timer),
                nameof(Attunement_Stacks),
                nameof(Max_Attunement_Stacks)
            };

            _progressFieldNames = new() {
                nameof(Aetherflow_Stacks),
                nameof(Summon_Timer),
                nameof(Attunement_Timer),
                nameof(Attunement_Stacks)
            };
        }
    }
}
