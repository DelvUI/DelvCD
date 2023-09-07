using Newtonsoft.Json.Linq;

namespace DelvCD.Helpers.DataSources
{
    public class CharacterStateDataSource : DataSource
    {
        public string Name = string.Empty;
        public LazyString<string?>? Name_First;
        public LazyString<string?>? Name_Last;
        public Job Job;
        public LazyString<Job>? JobName;

        public uint Level;
        public float Hp;
        public float MaxHp;
        public float Mp;
        public float MaxMp;
        public float Gp;
        public float MaxGp;
        public float Cp;
        public float MaxCp;
        public bool HasPet;
        public float Distance;

        public new static string GetFriendlyName() { return "Character State Triggers"; }

        public override float ProgressValue
        {
            get => Hp;
            set => Hp = value;
        }

        public override float ProgressMaxValue
        {
            get => MaxHp;
            set => MaxHp = value;
        }

        public CharacterStateDataSource()
        {
            Name_First = new LazyString<string?>(() => Name, LazyStringConverters.FirstName);
            Name_Last = new LazyString<string?>(() => Name, LazyStringConverters.LastName);
            JobName = new LazyString<Job>(() => Job, LazyStringConverters.JobName);

            SetConditionFields(
                nameof(Hp),
                nameof(Mp),
                nameof(Cp),
                nameof(Gp),
                nameof(Level),
                nameof(Distance)
            );
        }
    }
}
