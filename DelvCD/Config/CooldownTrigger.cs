using Dalamud.Interface;
using Dalamud.Interface.Utility;
using Dalamud.Plugin.Services;
using DelvCD.Helpers;
using DelvCD.Helpers.DataSources;
using Dalamud.Bindings.ImGui;
using System;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Text.Json.Serialization;

namespace DelvCD.Config
{
    public class CooldownTrigger : TriggerOptions
    {
        [JsonIgnore] private float _scale => ImGuiHelpers.GlobalScale;

        [JsonIgnore] private static readonly string[] _comboOptions = new[] { "Ready", "Not Ready" };
        [JsonIgnore] private static readonly string[] _usableOptions = new[] { "Usable", "Not Usable" };
        [JsonIgnore] private static readonly string[] _rangeOptions = new[] { "In Range", "Not in Range" };
        [JsonIgnore] private static readonly string[] _losOptions = new[] { "In LoS", "Not in LoS" };
        [JsonIgnore] private static readonly string[] _highlightOptions = new[] { "Highlighted", "Not Highlighted" };
        [JsonIgnore] private static readonly string[] _combatTypeOptions = new[] { "PvE", "PvP" };

        [JsonIgnore] private string _triggerNameInput = string.Empty;
        [JsonIgnore] private string _cooldownValueInput = string.Empty;
        [JsonIgnore] private string _chargeCountValueInput = string.Empty;

        public string TriggerName = string.Empty;

        public CombatType CombatType = CombatType.PvE;
        public bool Adjust = false;

        public bool Cooldown = false;
        public TriggerDataOp CooldownOp = TriggerDataOp.GreaterThan;
        public float CooldownValue;

        public bool ChargeCount = false;
        public TriggerDataOp ChargeCountOp = TriggerDataOp.GreaterThan;
        public float ChargeCountValue;

        public bool Combo = false;
        public int ComboValue;

        public bool Usable = false;
        public int UsableValue;

        public bool RangeCheck;
        public int RangeValue;

        public bool LosCheck;
        public int LosValue;
        
        public bool HighlightCheck;
        public int HighlightValue;

        public override TriggerType Type => TriggerType.Cooldown;
        public override TriggerSource Source => TriggerSource.Player;

        [JsonIgnore] private CooldownDataSource _dataSource = new();
        [JsonIgnore] public override DataSource DataSource => _dataSource;

        public override bool IsTriggered(bool preview)
        {
            if (preview)
            {
                _dataSource.Cooldown_Timer = 10;
                _dataSource.Cooldown_Stacks = 2;
                _dataSource.Max_Cooldown_Stacks = 2;
                _dataSource.Icon = TriggerData.FirstOrDefault()?.Icon ?? 0;
                _dataSource.Name = TriggerData.FirstOrDefault()?.Name ?? string.Empty;

                return true;
            }

            TriggerData? actionTrigger = TriggerData.FirstOrDefault(t => t.CombatType == CombatType);
            if (actionTrigger is null)
            {
                return false;
            }

            ActionHelpers helper = Singletons.Get<ActionHelpers>();
            uint actionId = Adjust ? helper.GetAdjustedActionId(actionTrigger.Id) : actionTrigger.Id;
            helper.GetAdjustedRecastInfo(actionId, out RecastInfo recastInfo);

            int stacks = recastInfo.RecastTime == 0f
                ? recastInfo.MaxCharges
                : (int)(recastInfo.MaxCharges * (recastInfo.RecastTimeElapsed / recastInfo.RecastTime));

            float chargeTime = recastInfo.MaxCharges != 0
                ? recastInfo.RecastTime / recastInfo.MaxCharges
                : recastInfo.RecastTime;

            float cooldown = chargeTime != 0
                ? Math.Abs(recastInfo.RecastTime - recastInfo.RecastTimeElapsed) % chargeTime
                : 0;

            bool comboActive = false;
            bool usable = false;
            bool inRange = false;
            bool inLos = false;
            bool isHighlighted = false;

            if (Usable)
            {
                usable = helper.CanUseAction(actionId);
            }

            if (RangeCheck)
            {
                inRange = helper.GetActionInRange(actionId, Singletons.Get<IObjectTable>().LocalPlayer, Utils.FindTarget());
            }

            if (LosCheck)
            {
                inLos = helper.IsTargetInLos(Singletons.Get<IObjectTable>().LocalPlayer, Utils.FindTarget(), actionId);
            }

            if (HighlightCheck)
            {
                isHighlighted = helper.IsActionHighlighted(actionId);
            }

            if (Combo && actionTrigger.ComboId.Length > 0)
            {
                uint lastAction = helper.GetLastUsedActionId();
                foreach (uint id in actionTrigger.ComboId)
                {
                    if (id == lastAction)
                    {
                        comboActive = true;
                        break;
                    }
                }
            }

            _dataSource.Id = actionId;
            _dataSource.Cooldown_Timer = cooldown;
            _dataSource.Value = cooldown;
            _dataSource.Max_Cooldown_Timer = chargeTime;
            _dataSource.Cooldown_Stacks = stacks;
            _dataSource.Max_Cooldown_Stacks = recastInfo.MaxCharges;
            _dataSource.Icon = Adjust ? helper.GetIconIdForAction(actionId) : actionTrigger.Icon;
            _dataSource.Name = Adjust ? helper.GetAdjustedActionName(actionId) : actionTrigger.Name;
            
            
            KeybindHelper keybindHelper = Singletons.Get<KeybindHelper>();
            _dataSource.Keybind = keybindHelper.GetKeybindHint(actionTrigger.Id, KeybindHelper.KeybindType.Action);
            _dataSource.Keybind_Formatted = keybindHelper.GetKeybindHintFormatted(actionTrigger.Id, KeybindHelper.KeybindType.Action);

            return
                (!Combo || (ComboValue == 0 ? comboActive : !comboActive)) &&
                (!Usable || (UsableValue == 0 ? usable : !usable)) &&
                (!RangeCheck || (RangeValue == 0 ? inRange : !inRange)) &&
                (!LosCheck || (LosValue == 0 ? inLos : !inLos)) &&
                (!HighlightCheck || (HighlightValue == 0 ? isHighlighted : !isHighlighted)) &&
                (!Cooldown || Utils.GetResult(_dataSource.Cooldown_Timer, CooldownOp, CooldownValue)) &&
                (!ChargeCount || Utils.GetResult(_dataSource.Cooldown_Stacks, ChargeCountOp, ChargeCountValue));
        }

        public override void DrawTriggerOptions(Vector2 size, float padX, float padY)
        {
            if (string.IsNullOrEmpty(_triggerNameInput))
            {
                _triggerNameInput = TriggerName;
            }

            ImGui.Combo("Combat Type", ref Unsafe.As<CombatType, int>(ref CombatType), _combatTypeOptions, _combatTypeOptions.Length);
            if (ImGui.InputTextWithHint("Action", "Action Name or ID", ref _triggerNameInput, 64))
            {
                TriggerData.Clear();
                if (!string.IsNullOrEmpty(_triggerNameInput))
                {
                    foreach (var triggerData in ActionHelpers.FindActionEntries(_triggerNameInput))
                    {
                        AddTriggerData(triggerData);
                    }
                }
            }

            bool valid = TriggerData.Count > 0;
            string validText = valid ? FontAwesomeIcon.CheckSquare.ToIconString() : FontAwesomeIcon.SquareXmark.ToIconString();
            ImGui.SameLine();
            ImGui.PushFont(UiBuilder.IconFont);
            ImGui.Text(validText);
            ImGui.PopFont();

            Vector2 cursorPos = ImGui.GetCursorPos();
            if (valid)
            {
                ImDrawListPtr drawList = ImGui.GetWindowDrawList();
                float width = ImGui.GetWindowWidth();
                Vector2 iconPos = ImGui.GetWindowPos() + new Vector2(width - 100 * _scale, 20 * _scale);
                DrawHelpers.DrawIcon(
                    TriggerData[0].Icon,
                    iconPos,
                    new Vector2(40 * _scale, 40 * _scale),
                    false,
                    0,
                    false,
                    1f,
                    drawList
                );
            }

            ImGui.Checkbox("Use Adjusted Action", ref Adjust);
            if (ImGui.IsItemHovered())
            {
                ImGui.SetTooltip("Enable to dynamically track abilities that upgrade or change during combat.\n" +
                                 "Examples: Standard Step -> Standard Finish, Gallows -> Cross Reaping, etc.\n\n" +
                                 "Best when used with the 'Automatic Icon' option.\n" +
                                 "WARNING: May have unexpected behavior when used with XIVCombo!");
            }

            DrawHelpers.DrawSpacing(1);
            ImGui.Text("Trigger Conditions");
            string[] operatorOptions = TriggerOptions.OperatorOptions;
            float optionsWidth = 100 * _scale + padX;
            float opComboWidth = 55 * _scale;
            float valueInputWidth = 45 * _scale;
            float padWidth = 0;

            DrawHelpers.DrawNestIndicator(1);
            ImGui.Checkbox("Cooldown", ref Cooldown);
            if (Cooldown)
            {
                ImGui.SameLine();
                padWidth = ImGui.CalcItemWidth() - ImGui.GetCursorPosX() - optionsWidth + padX;
                ImGui.SetCursorPosX(ImGui.GetCursorPosX() + padWidth);
                ImGui.PushItemWidth(opComboWidth);
                ImGui.Combo("##CooldownOpCombo", ref Unsafe.As<TriggerDataOp, int>(ref CooldownOp), operatorOptions, operatorOptions.Length);
                ImGui.PopItemWidth();
                ImGui.SameLine();

                if (string.IsNullOrEmpty(_cooldownValueInput))
                {
                    _cooldownValueInput = CooldownValue.ToString();
                }

                ImGui.PushItemWidth(valueInputWidth);
                if (ImGui.InputText("Seconds##CooldownValue", ref _cooldownValueInput, 10, ImGuiInputTextFlags.CharsDecimal))
                {
                    if (float.TryParse(_cooldownValueInput, out float value))
                    {
                        CooldownValue = value;
                    }

                    _cooldownValueInput = CooldownValue.ToString();
                }

                ImGui.PopItemWidth();
            }

            DrawHelpers.DrawNestIndicator(1);
            ImGui.Checkbox("Charge Count", ref ChargeCount);
            if (ChargeCount)
            {
                ImGui.SameLine();
                padWidth = ImGui.CalcItemWidth() - ImGui.GetCursorPosX() - optionsWidth + padX;
                ImGui.SetCursorPosX(ImGui.GetCursorPosX() + padWidth);
                ImGui.PushItemWidth(opComboWidth);
                ImGui.Combo("##ChargeCountOpCombo", ref Unsafe.As<TriggerDataOp, int>(ref ChargeCountOp), operatorOptions, operatorOptions.Length);
                ImGui.PopItemWidth();
                ImGui.SameLine();

                if (string.IsNullOrEmpty(_chargeCountValueInput))
                {
                    _chargeCountValueInput = ChargeCountValue.ToString();
                }

                ImGui.PushItemWidth(valueInputWidth);
                if (ImGui.InputText("Stacks##ChargeCountValue", ref _chargeCountValueInput, 10, ImGuiInputTextFlags.CharsDecimal))
                {
                    if (float.TryParse(_chargeCountValueInput, out float value))
                    {
                        ChargeCountValue = value;
                    }

                    _chargeCountValueInput = ChargeCountValue.ToString();
                }

                ImGui.PopItemWidth();
            }

            DrawHelpers.DrawNestIndicator(1);
            ImGui.Checkbox("Combo Ready", ref Combo);
            if (Combo)
            {
                ImGui.SameLine();
                padWidth = ImGui.CalcItemWidth() - ImGui.GetCursorPosX() - optionsWidth + padX;
                ImGui.SetCursorPosX(ImGui.GetCursorPosX() + padWidth);
                ImGui.PushItemWidth(optionsWidth);
                ImGui.Combo("##ComboCombo", ref ComboValue, _comboOptions, _comboOptions.Length);
                ImGui.PopItemWidth();
            }

            DrawHelpers.DrawNestIndicator(1);
            ImGui.Checkbox("Action Usable", ref Usable);
            if (ImGui.IsItemHovered())
            {
                ImGui.SetTooltip("Usable means resource/proc requirements to use the Action are met.");
            }

            if (Usable)
            {
                ImGui.SameLine();
                padWidth = ImGui.CalcItemWidth() - ImGui.GetCursorPosX() - optionsWidth + padX;
                ImGui.SetCursorPosX(ImGui.GetCursorPosX() + padWidth);
                ImGui.PushItemWidth(optionsWidth);
                ImGui.Combo("##UsableCombo", ref UsableValue, _usableOptions, _usableOptions.Length);
                ImGui.PopItemWidth();
            }

            DrawHelpers.DrawNestIndicator(1);
            ImGui.Checkbox("Action Highlighted", ref HighlightCheck);
            if (ImGui.IsItemHovered())
            {
                ImGui.SetTooltip("Highlighted means when the icon is glowing / has combo ants on the hotbar.");
            }

            if (HighlightCheck)
            {
                ImGui.SameLine();
                padWidth = ImGui.CalcItemWidth() - ImGui.GetCursorPosX() - optionsWidth + padX;
                ImGui.SetCursorPosX(ImGui.GetCursorPosX() + padWidth);
                ImGui.PushItemWidth(optionsWidth);
                ImGui.Combo("##HightlightCombo", ref HighlightValue, _highlightOptions, _highlightOptions.Length);
                ImGui.PopItemWidth();
            }

            DrawHelpers.DrawNestIndicator(1);
            ImGui.Checkbox("Target Range Check", ref RangeCheck);
            if (RangeCheck)
            {
                ImGui.SameLine();
                padWidth = ImGui.CalcItemWidth() - ImGui.GetCursorPosX() - optionsWidth + padX;
                ImGui.SetCursorPosX(ImGui.GetCursorPosX() + padWidth);
                ImGui.PushItemWidth(optionsWidth);
                ImGui.Combo("##RangeCombo", ref RangeValue, _rangeOptions, _rangeOptions.Length);
                ImGui.PopItemWidth();
            }

            DrawHelpers.DrawNestIndicator(1);
            ImGui.Checkbox("Target LoS Check", ref LosCheck);
            if (LosCheck)
            {
                ImGui.SameLine();
                padWidth = ImGui.CalcItemWidth() - ImGui.GetCursorPosX() - optionsWidth + padX;
                ImGui.SetCursorPosX(ImGui.GetCursorPosX() + padWidth);
                ImGui.PushItemWidth(optionsWidth);
                ImGui.Combo("##LosCombo", ref LosValue, _losOptions, _losOptions.Length);
                ImGui.PopItemWidth();
            }
        }

        private void ResetTrigger()
        {
            TriggerData.Clear();
            TriggerName = string.Empty;
            _triggerNameInput = string.Empty;
        }

        private void AddTriggerData(TriggerData triggerData)
        {
            TriggerName = triggerData.Name.ToString();
            TriggerData.Add(triggerData);
            _triggerNameInput = TriggerName;
        }
    }
}