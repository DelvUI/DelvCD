﻿using Newtonsoft.Json.Linq;
using System.Collections.Generic;

namespace DelvCD.Helpers.DataSources
{
    public class CharacterStateDataSource : DataSource
    {
        public new static string GetDisplayName() => "Character State";

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

        public override float GetConditionValue(int index) => index switch
        {
            0 => Hp,
            1 => Mp,
            2 => Cp,
            3 => Gp,
            4 => Level,
            5 => Distance,
            _ => 0
        };

        public override float GetProgressValue(int index) => index switch
        {
            0 => Hp,
            1 => Mp,
            2 => Cp,
            3 => Gp,
            _ => 0
        };

        public override float GetMaxValue(int index) => index switch
        {
            0 => MaxHp,
            1 => MaxMp,
            2 => MaxCp,
            3 => MaxGp,
            _ => 0
        };

        public CharacterStateDataSource()
        {
            Name_First = new LazyString<string?>(() => Name, LazyStringConverters.FirstName);
            Name_Last = new LazyString<string?>(() => Name, LazyStringConverters.LastName);
            JobName = new LazyString<Job>(() => Job, LazyStringConverters.JobName);

            _conditionFieldNames = new() {
                nameof(Hp),
                nameof(Mp),
                nameof(Cp),
                nameof(Gp),
                nameof(Level),
                nameof(Distance)
            };

            _progressFieldNames = new() {
                nameof(Hp),
                nameof(Mp),
                nameof(Cp),
                nameof(Gp)
            };
        }
    }
}
