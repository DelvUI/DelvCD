using Dalamud.Game.ClientState.JobGauge.Types;
using Dalamud.Plugin.Services;

namespace DelvCD.Helpers.DataSources.JobDataSources
{
    internal class ViperDataSource : DataSource
    {
        public new static string GetDisplayName() => "Viper";

        public int Rattling_Coil_Stacks;
        public int Max_Rattling_Coil_Stacks;
        public int Serpent_Offering;
        public int Anguine_Tribute_Stacks;
        public int Max_Anguine_Tribute_Stacks;

        public override float GetConditionValue(int index) => index switch
        {
            0 => Rattling_Coil_Stacks,
            1 => Max_Rattling_Coil_Stacks,
            2 => Serpent_Offering,
            3 => Anguine_Tribute_Stacks,
            4 => Max_Anguine_Tribute_Stacks,
            _ => 0
        };

        public override float GetProgressValue(int index) => index switch
        {
            0 => Rattling_Coil_Stacks,
            1 => Serpent_Offering,
            2 => Anguine_Tribute_Stacks,
            _ => 0
        };

        public override float GetMaxValue(int index) => index switch
        {
            0 => Max_Rattling_Coil_Stacks,
            1 => 100,
            2 => Max_Anguine_Tribute_Stacks,
            _ => 0
        };

        public ViperDataSource()
        {
            _conditionFieldNames = new() {
                nameof(Rattling_Coil_Stacks),
                nameof(Max_Rattling_Coil_Stacks),
                nameof(Serpent_Offering),
                nameof(Anguine_Tribute_Stacks),
                nameof(Max_Anguine_Tribute_Stacks)
            };

            _progressFieldNames = new() {
                nameof(Rattling_Coil_Stacks),
                nameof(Serpent_Offering),
                nameof(Anguine_Tribute_Stacks)
            };
        }
    }
}
