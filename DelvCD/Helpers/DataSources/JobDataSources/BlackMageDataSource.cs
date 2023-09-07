using Lumina.Excel.GeneratedSheets;

namespace DelvCD.Helpers.DataSources.JobDataSources
{
    public class BlackMageDataSource : DataSource
    {
        public new static string DisplayName => "Black Mage";

        public bool Enochian;
        public float Enochian_Timer;
        public int Polyglot_Stacks;
        public string Element = string.Empty;
        public float Element_Timer;
        public int Umbral_Ice_Stacks;
        public int Astral_Fire_Stacks;
        public int Umbral_Hearts;
        public bool Paradox;

        public override float ProgressValue
        {
            get => Enochian_Timer;
            set => Enochian_Timer = (int)value;
        }

        public override float ProgressMaxValue => 30;

        public override float GetConditionValue(int index) => index switch
        {
            0 => Enochian_Timer,
            1 => Polyglot_Stacks,
            2 => Element_Timer,
            3 => Umbral_Ice_Stacks,
            4 => Astral_Fire_Stacks,
            5 => Umbral_Hearts,
            _ => 0
        };


        public BlackMageDataSource()
        {
            _conditionFieldNames = new() {
                nameof(Enochian_Timer),
                nameof(Polyglot_Stacks),
                nameof(Element_Timer),
                nameof(Umbral_Ice_Stacks),
                nameof(Astral_Fire_Stacks),
                nameof(Umbral_Hearts),
            };
        }
    }
}
