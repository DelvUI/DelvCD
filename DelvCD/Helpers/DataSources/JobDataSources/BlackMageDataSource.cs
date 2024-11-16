using Lumina.Excel.Sheets;

namespace DelvCD.Helpers.DataSources.JobDataSources
{
    public class BlackMageDataSource : DataSource
    {
        public new static string GetDisplayName() => "Black Mage";

        public bool Enochian;
        public float Enochian_Timer;
        public int Polyglot_Stacks;
        public int Max_Polyglot_Stacks;
        public string Element = string.Empty;
        public float Element_Timer;
        public int Umbral_Ice_Stacks;
        public int Astral_Fire_Stacks;
        public int Umbral_Hearts;
        public bool Paradox;
        public int Astral_Soul_Stacks;

        public override float GetConditionValue(int index) => index switch
        {
            0 => Enochian_Timer,
            1 => Polyglot_Stacks,
            2 => Max_Polyglot_Stacks,
            3 => Element_Timer,
            4 => Umbral_Ice_Stacks,
            5 => Astral_Fire_Stacks,
            6 => Umbral_Hearts,
            7 => Astral_Soul_Stacks,
            _ => 0
        };

        public override float GetProgressValue(int index) => index switch
        {
            0 => Enochian_Timer,
            1 => Polyglot_Stacks,
            2 => Element_Timer,
            3 => Umbral_Ice_Stacks,
            4 => Astral_Fire_Stacks,
            5 => Umbral_Hearts,
            6 => Astral_Soul_Stacks,
            _ => 0
        };

        public override float GetMaxValue(int index) => index switch
        {
            0 => 30,
            1 => Max_Polyglot_Stacks,
            2 => 15,
            3 => 3,
            4 => 3,
            5 => 3,
            6 => 6,
            _ => 0
        };

        public BlackMageDataSource()
        {
            _conditionFieldNames = new() {
                nameof(Enochian_Timer),
                nameof(Polyglot_Stacks),
                nameof(Max_Polyglot_Stacks),
                nameof(Element_Timer),
                nameof(Umbral_Ice_Stacks),
                nameof(Astral_Fire_Stacks),
                nameof(Umbral_Hearts),
                nameof(Astral_Soul_Stacks)
            };

            _progressFieldNames = new() {
                nameof(Enochian_Timer),
                nameof(Polyglot_Stacks),
                nameof(Element_Timer),
                nameof(Umbral_Ice_Stacks),
                nameof(Astral_Fire_Stacks),
                nameof(Umbral_Hearts),
                nameof(Astral_Fire_Stacks)
            };
        }
    }
}
