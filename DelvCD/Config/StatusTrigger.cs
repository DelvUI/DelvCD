using Dalamud.Game.ClientState;
using Dalamud.Game.ClientState.Objects;
using Dalamud.Game.ClientState.Objects.SubKinds;
using Dalamud.Game.ClientState.Objects.Types;
using Dalamud.Interface;
using DelvCD.Helpers;
using DelvCD.Helpers.DataSources;
using ImGuiNET;
using System;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Text.Json.Serialization;

namespace DelvCD.Config
{
    public class StatusTrigger : TriggerOptions
    {
        [JsonIgnore] private float _scale => ImGuiHelpers.GlobalScale;

        [JsonIgnore] private static readonly string[] _sourceOptions = Enum.GetNames<TriggerSource>();
        [JsonIgnore] private static readonly string[] _triggerConditions = new string[] { "Status Active", "Status Not Active" };

        [JsonIgnore] private string _triggerNameInput = string.Empty;
        [JsonIgnore] private string _triggerConditionValueInput = string.Empty;
        [JsonIgnore] private string _durationValueInput = string.Empty;
        [JsonIgnore] private string _stackCountValueInput = string.Empty;

        public TriggerSource TriggerSource = TriggerSource.Player;
        public string TriggerName = string.Empty;
        public int TriggerCondition = 0;

        public bool OnlyMine = true;

        public bool Duration = false;
        public TriggerDataOp DurationOp = TriggerDataOp.GreaterThan;
        public float DurationValue;

        public bool StackCount = false;
        public TriggerDataOp StackCountOp = TriggerDataOp.GreaterThan;
        public float StackCountValue;


        public override TriggerType Type => TriggerType.Status;
        public override TriggerSource Source => TriggerSource;

        [JsonIgnore] public override Type DataSourceType => typeof(CooldownDataSource);

        public override (bool, DataSource) IsTriggered(bool preview)
        {
            CooldownDataSource data = new CooldownDataSource();
            if (!TriggerData.Any())
            {
                return (false, data);
            }

            if (preview)
            {
                data.Value = 10;
                data.Stacks = 2;
                data.MaxStacks = 2;
                data.Icon = TriggerData.FirstOrDefault()?.Icon ?? 0;

                return (true, data);
            }

            PlayerCharacter? player = Singletons.Get<ClientState>().LocalPlayer;
            if (player is null)
            {
                return (false, data);
            }

            GameObject? actor = Source switch
            {
                TriggerSource.Player => player,
                TriggerSource.Target => Utils.FindTarget(),
                TriggerSource.TargetOfTarget => Utils.FindTargetOfTarget(),
                TriggerSource.FocusTarget => Singletons.Get<TargetManager>().FocusTarget,
                _ => null
            };
            if (actor is null)
            {
                return (false, data);
            }


            bool active = false;
            data.Icon = TriggerData.First().Icon;
            StatusHelpers helper = Singletons.Get<StatusHelpers>();
            foreach (TriggerData trigger in TriggerData)
            {
                var statusList = helper.GetStatusList(Source, trigger.Id);
                foreach (var status in statusList)
                {
                    if (status is not null &&
                        (status.SourceId == player.ObjectId || !OnlyMine))
                    {
                        active = true;
                        data.Id = status.StatusId;
                        data.Value = Math.Abs(status.RemainingTime);
                        data.Stacks = status.StackCount;
                        data.MaxStacks = trigger.MaxStacks;
                        data.Icon = trigger.Icon;
                        break;
                    }
                }
            }

            bool triggered = TriggerCondition switch
            {
                0 => active &&
                        (!Duration || Utils.GetResult(data.Value, DurationOp, DurationValue)) &&
                        (!StackCount || Utils.GetResult(data.Stacks, StackCountOp, StackCountValue)),
                1 => !active,
                _ => false
            };

            return (triggered, data);
        }

        public override void DrawTriggerOptions(Vector2 size, float padX, float padY)
        {
            ImGui.Combo("Trigger Source", ref Unsafe.As<TriggerSource, int>(ref TriggerSource), _sourceOptions, _sourceOptions.Length);

            if (string.IsNullOrEmpty(_triggerNameInput))
            {
                _triggerNameInput = TriggerName;
            }

            if (ImGui.InputTextWithHint("Status", "Status Name or ID", ref _triggerNameInput, 32, ImGuiInputTextFlags.EnterReturnsTrue))
            {
                TriggerData.Clear();
                if (!string.IsNullOrEmpty(_triggerNameInput))
                {
                    StatusHelpers.FindStatusEntries(_triggerNameInput).ForEach(t => AddTriggerData(t));
                }
            }

            ImGui.Checkbox("Only Mine", ref OnlyMine);
            DrawHelpers.DrawSpacing(1);
            ImGui.Combo("Trigger Condition", ref TriggerCondition, _triggerConditions, _triggerConditions.Length);
            if (TriggerCondition == 0)
            {
                string[] operatorOptions = TriggerOptions.OperatorOptions;
                float optionsWidth = 100 * _scale + padX;
                float opComboWidth = 55 * _scale;
                float valueInputWidth = 45 * _scale;
                float padWidth = 0;

                ImGui.Checkbox("Duration Remaining", ref Duration);
                if (Duration)
                {
                    ImGui.SameLine();
                    padWidth = ImGui.CalcItemWidth() - ImGui.GetCursorPosX() - optionsWidth + padX;
                    ImGui.SetCursorPosX(ImGui.GetCursorPosX() + padWidth);
                    ImGui.PushItemWidth(opComboWidth);
                    ImGui.Combo("##DurationOpCombo", ref Unsafe.As<TriggerDataOp, int>(ref DurationOp), operatorOptions, operatorOptions.Length);
                    ImGui.PopItemWidth();
                    ImGui.SameLine();

                    if (string.IsNullOrEmpty(_durationValueInput))
                    {
                        _durationValueInput = DurationValue.ToString();
                    }

                    ImGui.PushItemWidth(valueInputWidth);
                    if (ImGui.InputText("Seconds##DurationValue", ref _durationValueInput, 10, ImGuiInputTextFlags.CharsDecimal))
                    {
                        if (float.TryParse(_durationValueInput, out float value))
                        {
                            DurationValue = value;
                        }

                        _durationValueInput = DurationValue.ToString();
                    }

                    ImGui.PopItemWidth();
                }

                ImGui.Checkbox("Stack Count", ref StackCount);
                if (StackCount)
                {
                    ImGui.SameLine();
                    padWidth = ImGui.CalcItemWidth() - ImGui.GetCursorPosX() - optionsWidth + padX;
                    ImGui.SetCursorPosX(ImGui.GetCursorPosX() + padWidth);
                    ImGui.PushItemWidth(opComboWidth);
                    ImGui.Combo("##StackCountOpCombo", ref Unsafe.As<TriggerDataOp, int>(ref StackCountOp), operatorOptions, operatorOptions.Length);
                    ImGui.PopItemWidth();
                    ImGui.SameLine();

                    if (string.IsNullOrEmpty(_stackCountValueInput))
                    {
                        _stackCountValueInput = StackCountValue.ToString();
                    }

                    ImGui.PushItemWidth(valueInputWidth);
                    if (ImGui.InputText("Stacks##StackCountValue", ref _stackCountValueInput, 10, ImGuiInputTextFlags.CharsDecimal))
                    {
                        if (float.TryParse(_stackCountValueInput, out float value))
                        {
                            StackCountValue = value;
                        }

                        _stackCountValueInput = StackCountValue.ToString();
                    }

                    ImGui.PopItemWidth();
                }
            }
        }

        private void AddTriggerData(TriggerData triggerData)
        {
            TriggerName = triggerData.Name.ToString();
            _triggerNameInput = TriggerName;
            TriggerData.Add(triggerData);
        }
    }
}