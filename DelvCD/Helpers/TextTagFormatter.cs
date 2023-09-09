using DelvCD.Helpers.DataSources;
using DelvCD.Helpers.DataSources.JobDataSources;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

namespace DelvCD.Helpers
{
    public class TextTagFormatter
    {
        public static Regex TextTagRegex { get; } = new Regex(@"\[(\w*)(:\w)?\.?(\d+)?\]", RegexOptions.Compiled);

        private static Dictionary<Type, Dictionary<string, FieldInfo>> _fieldsMap = new();
        
        private static Dictionary<string, List<string>> _textTagsHelpData = new();
        public static Dictionary<string, List<string>> TextTagsHelpData => _textTagsHelpData;

        private string _format;
        private Dictionary<string, FieldInfo> _fields;
        private object _source;
        private int _rounding;

        public TextTagFormatter(
            object source,
            string format,
            int rounding,
            Type type)
        {
            _source = source;
            _format = format;
            _rounding = rounding;

            if (_fieldsMap.TryGetValue(type, out Dictionary<string, FieldInfo>? fields) && fields != null)
            {
                _fields = fields;
            }
            else
            {
                _fields = new();
            }
        }

        public string Evaluate(Match m)
        {
            if (m.Groups.Count != 4)
            {
                return m.Value;
            }

            string key = m.Groups[1].Value;
            string? value = null;

            if (_fields.ContainsKey(key))
            {
                object? propValue = _fields[key].GetValue(_source);

                if (propValue is null)
                {
                    return string.Empty;
                }

                if (propValue is float f)
                {
                    int decimals = int.TryParse(m.Groups[3].Value, out int dec) ? dec : 0;
                    value = m.Groups[2].Value switch
                    {
                        ":k" => KiloFormat(f, _format, decimals, _rounding) ?? m.Value,
                        ":t" => TimeFormat(f, _rounding, decimals),
                        _ => FloatFormat(f, _format, decimals, _rounding)
                    };
                }
                else
                {
                    value = propValue?.ToString();
                    if (!string.IsNullOrEmpty(value) &&
                        int.TryParse(m.Groups[3].Value, out int trim) &&
                        trim < value.Length)
                    {
                        value = propValue?.ToString().AsSpan(0, trim).ToString();
                    }
                }
            }

            return value ?? m.Value;
        }

        private static string FloatFormat(float value, string format, int decimals, int rounding)
        {
            int m = (int)Math.Pow(10, decimals);

            double temp = rounding switch
            {
                0 => Math.Truncate(value * m),
                1 => Math.Ceiling(value * m),
                2 => Math.Round(value * m),
                _ => Math.Truncate(value * m)
            } / m;

            return temp.ToString($"{format}{decimals}", CultureInfo.InvariantCulture);
        }

        private static string TimeFormat(float seconds, int rounding, int decimals) => seconds switch
        {
            > 3600 => $"{(int)seconds / 3600:0}:{((int)seconds % 3600) / 60:00}:{(int)seconds % 60:00}",
            > 60 => $"{(int)seconds / 60:0}:{(int)seconds % 60:00}",
            _ => FloatFormat(seconds, "F", decimals, rounding)
        };

        private static string KiloFormat(float num, string format, int decimals, int rounding) => num switch
        {
            >= 1000000 => FloatFormat(num / 1000000f, format, decimals, rounding) + "M",
            >= 1000 => FloatFormat(num / 1000f, format, decimals, rounding) + "K",
            _ => FloatFormat(num, format, decimals, rounding)
        };

        public static string GetFormattedString(DataSource[] dataSources, string format, string numberFormat, int rounding)
        {
            string result = format;

            foreach (DataSource dataSource in dataSources)
            {
                result = TextTagRegex.Replace(
                    result,
                    new TextTagFormatter(dataSource, numberFormat, rounding, dataSource.GetType()).Evaluate
                );
            }

            return result;
        }


        public static void InitializeTextTags()
        {
            Type[] types = new Type[]
            {
                typeof(CooldownDataSource),
                typeof(CharacterStateDataSource),
                typeof(AstrologianDataSource),
                typeof(BardDataSource),
                typeof(BlackMageDataSource),
                typeof(DancerDataSource),
                typeof(DarkKnightDataSource),
                typeof(DragoonDataSource),
                typeof(GunbreakerDataSource),
                typeof(MachinistDataSource),
                typeof(MonkDataSource),
                typeof(NinjaDataSource),
                typeof(PaladinDataSource),
                typeof(ReaperDataSource),
                typeof(RedMageDataSource),
                typeof(SageDataSource),
                typeof(SamuraiDataSource),
                typeof(ScholarDataSource),
                typeof(SummonerDataSource),
                typeof(WarriorDataSource),
                typeof(WhiteMageDataSource)
            };

            for (int i = 0; i < types.Length; i++)
            {
                Type type = types[i];

                Dictionary<string, FieldInfo> dict = type.GetFields().ToDictionary((x) => x.Name.ToLower());
                _fieldsMap[type] = dict;

                string name = type.Name;
                MethodInfo? method = type.GetMethod("GetDisplayName");
                if (method != null)
                {
                    object? obj = method.Invoke(type, null);
                    if (obj is string str)
                    {
                        name = str;
                    }
                }

                List<string> list = dict.Keys.Select(x => $"[{x}]").ToList();
                list.Remove("[id]");
                list.Remove("[icon]");
                list.Remove("[prewviewvalue]");
                list.Remove("[prewviewmaxvalue]");

                _textTagsHelpData[name] = list;
            }
        }
    }
}