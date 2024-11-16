using System;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;
using Dalamud.Game.ClientState.Objects.SubKinds;
using Dalamud.Interface.Utility;
using Dalamud.Plugin.Services;
using DelvCD.Config.JobGauges;
using DelvCD.Helpers;
using DelvCD.Helpers.DataSources;
using ImGuiNET;
using Newtonsoft.Json;

namespace DelvCD.Config
{
    internal class JobGaugeTrigger : TriggerOptions
    {
        [JsonIgnore] private float _scale => ImGuiHelpers.GlobalScale;

        [JsonIgnore] private string _triggerNameInput = string.Empty;

        public string TriggerName = string.Empty;

        public override TriggerType Type => TriggerType.JobGauge;
        public override TriggerSource Source => TriggerSource.Player;

        [JsonIgnore] private JobGauge? _jobGauge = new AstrologianJobGauge("");
        [JsonIgnore] public override DataSource DataSource => _jobGauge?.DataSource ?? new CooldownDataSource();


        private int _jobIndex = 0;
        public int JobIndex
        {
            get { return _jobIndex; }
            set
            {
                JobGauge? newDataSource = JobGauge.CreateDataSource(_typesArray[value], _rawData);
                if (newDataSource != null)
                {
                    _jobGauge = newDataSource;
                    _jobIndex = value;

                    OnDataSourceChange?.Invoke();
                }
            }
        }

        private string _rawData = string.Empty;
        public string RawData
        {
            get { return _jobGauge != null ? _jobGauge.RawData : _rawData; }
            set
            {
                _rawData = value;

                if (_jobGauge != null)
                {
                    _jobGauge.RawData = _rawData;
                }
            }
        }

        [JsonIgnore] private string[] _booleanOptions = new string[] { "Inactive", "Active" };

        public override bool IsTriggered(bool preview)
        {
            if (_jobGauge == null)
            {
                return false;
            }

            IPlayerCharacter? player = Singletons.Get<IClientState>().LocalPlayer;
            if (player == null || player.ClassJob.RowId != (uint)_jobGauge.Job)
            {
                return false;
            }

            try
            {
                return _jobGauge.IsTriggered(preview);
            }
            catch (Exception e)
            {
                Singletons.Get<IPluginLog>().Error(e.Message);
            }

            return false;
        }

        public override void DrawTriggerOptions(Vector2 size, float padX, float padY)
        {
            if (string.IsNullOrEmpty(_triggerNameInput))
            {
                _triggerNameInput = TriggerName;
            }

            int index = _jobIndex;
            if (ImGui.Combo("Job", ref index, _jobNamesArray, _jobNamesArray.Length))
            {
                TriggerData.Clear();

                JobIndex = index;

                AddTriggerData(new TriggerData(_jobNamesArray[index], 0, 0));
            }

            if (_jobGauge == null) { return; }

            DrawHelpers.DrawSpacing(1);
            ImGui.Text("Trigger Conditions");
            string[] operatorOptions = TriggerOptions.OperatorOptions;
            float optionsWidth = 100 * _scale + padX;
            float opComboWidth = 55 * _scale;
            float valueInputWidth = 45 * _scale;
            float padWidth = 0;

            for (int i = 0; i < _jobGauge.ConditionCount; i++)
            {
                DrawHelpers.DrawNestIndicator(1);

                bool enabled = _jobGauge.ConditionEnabledforIndex(i);
                if (ImGui.Checkbox(_jobGauge.ConditionNameForIndex(i), ref enabled))
                {
                    _jobGauge.SetEnabledForIndex(i, enabled);
                }

                if (enabled)
                {
                    ImGui.SameLine();
                    padWidth = ImGui.CalcItemWidth() - ImGui.GetCursorPosX() - optionsWidth + padX;
                    ImGui.SetCursorPosX(ImGui.GetCursorPosX() + padWidth);
                    ImGui.PushItemWidth(optionsWidth);

                    TriggerConditionType conditionType = _jobGauge.ConditionTypeForIndex(i);
                    if (conditionType == TriggerConditionType.Numeric)
                    {
                        TriggerDataOp op = _jobGauge.OperatorForIndex(i);
                        if (ImGui.Combo("##OpCombo" + i.ToString(), ref Unsafe.As<TriggerDataOp, int>(ref op), operatorOptions, operatorOptions.Length))
                        {
                            _jobGauge.SetOperatorForIndex(i, op);
                        }
                        ImGui.PopItemWidth();
                        ImGui.SameLine();

                        ImGui.PushItemWidth(valueInputWidth);
                        string value = _jobGauge.ValueForIndex(i).ToString();
                        if (ImGui.InputText("##Value" + i.ToString(), ref value, 10, ImGuiInputTextFlags.CharsDecimal))
                        {
                            if (int.TryParse(value, out int v))
                            {
                                _jobGauge.SetValueForIndex(i, v);
                            }
                        }

                        ImGui.PopItemWidth();
                    }
                    else
                    {
                        int value = _jobGauge.ValueForIndex(i);
                        string[] options = conditionType == TriggerConditionType.Boolean ? _booleanOptions : _jobGauge.ComboOptionsForIndex(i);
                        if (ImGui.Combo("##Combo" + i.ToString(), ref value, options, options.Length))
                        {
                            _jobGauge.SetValueForIndex(i, value);
                        }

                        ImGui.PopItemWidth();
                    }
                }
            }
        }

        private void ResetTrigger()
        {
            TriggerData.Clear();
            TriggerName = string.Empty;
            _triggerNameInput = string.Empty;
            _rawData = "";

            if (_jobGauge != null)
            {
                _jobGauge.RawData = "";
            }
        }

        private void AddTriggerData(TriggerData triggerData)
        {
            TriggerName = triggerData.Name.ToString();
            _triggerNameInput = TriggerName;
            TriggerData.Add(triggerData);
            Singletons.Get<IPluginLog>().Information($"{triggerData.Name}: {triggerData.Icon}");
        }


        [JsonIgnore]
        private static int[] _jobIdsArray = new int[]
        {
            (int)Job.AST,
            (int)Job.BRD,
            (int)Job.BLM,
            (int)Job.DNC,
            (int)Job.DRK,
            (int)Job.DRG,
            (int)Job.GNB,
            (int)Job.MCH,
            (int)Job.MNK,
            (int)Job.NIN,
            (int)Job.PLD,
            (int)Job.PCT,
            (int)Job.RPR,
            (int)Job.RDM,
            (int)Job.SGE,
            (int)Job.SAM,
            (int)Job.SCH,
            (int)Job.SMN,
            (int)Job.VPR,
            (int)Job.WAR,
            (int)Job.WHM,
        };

        [JsonIgnore]
        private static string[] _jobNamesArray = new string[]
        {
            "Astrologian",
            "Bard",
            "Black Mage",
            "Dancer",
            "Dark Knight",
            "Dragoon",
            "Gunbreaker",
            "Machinist",
            "Monk",
            "Ninja",
            "Paladin",
            "Pictomancer",
            "Reaper",
            "Red Mage",
            "Sage",
            "Samurai",
            "Scholar",
            "Summoner",
            "Viper",
            "Warrior",
            "White Mage"
        };

        [JsonIgnore]
        private static Type[] _typesArray = new Type[]
        {
            typeof(AstrologianJobGauge),
            typeof(BardJobGauge),
            typeof(BlackMageJobGauge),
            typeof(DancerJobGauge),
            typeof(DarkKnightJobGauge),
            typeof(DragoonJobGauge),
            typeof(GunbreakerJobGauge),
            typeof(MachinistJobGauge),
            typeof(MonkJobGauge),
            typeof(NinjaJobGauge),
            typeof(PaladinJobGauge),
            typeof(PictomancerJobGauge),
            typeof(ReaperJobGauge),
            typeof(RedMageJobGauge),
            typeof(SageJobGauge),
            typeof(SamuraiJobGauge),
            typeof(ScholarJobGauge),
            typeof(SummonerJobGauge),
            typeof(ViperJobGauge),
            typeof(WarriorJobGauge),
            typeof(WhiteMageJobGauge),
        };
    }
}
