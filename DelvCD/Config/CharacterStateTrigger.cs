using Dalamud.Game.ClientState.Objects;
using Dalamud.Game.ClientState.Objects.Types;
using Dalamud.Interface.Utility;
using Dalamud.Plugin.Services;
using DelvCD.Helpers;
using DelvCD.Helpers.DataSources;
using ImGuiNET;
using System;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Text.Json.Serialization;
using CharacterStruct = FFXIVClientStructs.FFXIV.Client.Game.Character.Character;


namespace DelvCD.Config
{
    public class CharacterStateTrigger : TriggerOptions
    {
        [JsonIgnore] private float _scale => ImGuiHelpers.GlobalScale;

        [JsonIgnore] private static readonly string[] _sourceOptions = Enum.GetNames<TriggerSource>();
        [JsonIgnore] private static readonly string[] _petOptions = new[] { "Has Pet", "Has No Pet" };

        [JsonIgnore] private string _hpValueInput = string.Empty;
        [JsonIgnore] private string _mpValueInput = string.Empty;
        [JsonIgnore] private string _levelValueInput = string.Empty;
        [JsonIgnore] private string _cpValueInput = string.Empty;
        [JsonIgnore] private string _gpValueInput = string.Empty;

        public TriggerSource TriggerSource = TriggerSource.Player;

        public override TriggerType Type => TriggerType.CharacterState;
        public override TriggerSource Source => TriggerSource;

        [JsonIgnore] private CharacterStateDataSource _dataSource = new();
        [JsonIgnore] public override DataSource DataSource => _dataSource;

        public bool Level = false;
        public TriggerDataOp LevelOp = TriggerDataOp.GreaterThan;
        public float LevelValue;

        public bool Hp = false;
        public TriggerDataOp HpOp = TriggerDataOp.GreaterThan;
        public float HpValue;
        public bool MaxHp;
        public bool HpPercent;

        public bool Mp = false;
        public TriggerDataOp MpOp = TriggerDataOp.GreaterThan;
        public float MpValue;
        public bool MaxMp;
        public bool MpPercent;

        public bool Cp = false;
        public TriggerDataOp CpOp = TriggerDataOp.GreaterThan;
        public float CpValue;
        public bool MaxCp;
        public bool CpPercent;

        public bool Gp = false;
        public TriggerDataOp GpOp = TriggerDataOp.GreaterThan;
        public float GpValue;
        public bool MaxGp;
        public bool GpPercent;

        public bool PetCheck;
        public int PetValue;

        public override bool IsTriggered(bool preview)
        {
            if (preview)
            {
                _dataSource.Hp = 50000;
                _dataSource.MaxHp = 100000;
                _dataSource.Mp = 5000;
                _dataSource.MaxMp = 10000;
                _dataSource.Cp = 50;
                _dataSource.MaxCp = 100;
                _dataSource.Gp = 50;
                _dataSource.MaxGp = 100;
                _dataSource.Level = 90;
                _dataSource.Distance = 10;
                _dataSource.HasPet = false;
                return true;
            }

            GameObject? actor = TriggerSource switch
            {
                TriggerSource.Player => Singletons.Get<IClientState>().LocalPlayer,
                TriggerSource.Target => Utils.FindTarget(),
                TriggerSource.TargetOfTarget => Utils.FindTargetOfTarget(),
                TriggerSource.FocusTarget => Singletons.Get<ITargetManager>().FocusTarget,
                _ => null
            };

            if (actor is null)
            {
                return false;
            }
            
            _dataSource.Name = actor.Name.ToString();
            var player = Singletons.Get<IClientState>().LocalPlayer;
            if (player is not null)
            {
                Vector3 difference = player.Position - actor.Position;
                _dataSource.Distance = MathF.Sqrt( difference.X * difference.X + difference.Z * difference.Z ) - actor.HitboxRadius;
            }

            if (actor is Character chara)
            {
                _dataSource.Hp = chara.CurrentHp;
                _dataSource.MaxHp = chara.MaxHp;
                _dataSource.Mp = chara.CurrentMp;
                _dataSource.MaxMp = chara.MaxMp;
                _dataSource.Cp = chara.CurrentCp;
                _dataSource.MaxCp = chara.MaxCp;
                _dataSource.Gp = chara.CurrentGp;
                _dataSource.MaxGp = chara.MaxGp;
                _dataSource.Level = chara.Level;
                _dataSource.HasPet = TriggerSource == TriggerSource.Player &&
                    Singletons.Get<IBuddyList>().PetBuddy != null;

                unsafe
                {
                    _dataSource.Job = (Job)((CharacterStruct*)chara.Address)->CharacterData.ClassJob;
                }
            }

            float hp = MaxHp || !HpPercent ? _dataSource.Hp : _dataSource.Hp / _dataSource.MaxHp * 100;
            float mp = MaxMp || !MpPercent ? _dataSource.Mp : _dataSource.Mp / _dataSource.MaxMp * 100;
            float cp = MaxCp || !CpPercent ? _dataSource.Cp : _dataSource.Cp / _dataSource.MaxCp * 100;
            float gp = MaxGp || !GpPercent ? _dataSource.Gp : _dataSource.Gp / _dataSource.MaxGp * 100;

            float hpValue = MaxHp ? _dataSource.MaxHp : HpValue;
            float mpValue = MaxMp ? _dataSource.MaxMp : MpValue;
            float cpValue = MaxCp ? _dataSource.MaxCp : CpValue;
            float gpValue = MaxGp ? _dataSource.MaxGp : GpValue;

            return
                (!Hp || Utils.GetResult(hp, HpOp, hpValue)) &&
                (!Mp || Utils.GetResult(mp, MpOp, mpValue)) &&
                (!Cp || Utils.GetResult(cp, CpOp, cpValue)) &&
                (!Gp || Utils.GetResult(gp, GpOp, gpValue)) &&
                (!Level || Utils.GetResult(_dataSource.Level, LevelOp, LevelValue)) &&
                (!PetCheck || (PetValue == 0 ? _dataSource.HasPet : !_dataSource.HasPet));
        }

        public override void DrawTriggerOptions(Vector2 size, float padX, float padY)
        {
            ImGui.Combo("Trigger Source", ref Unsafe.As<TriggerSource, int>(ref TriggerSource), _sourceOptions, _sourceOptions.Length);
            DrawHelpers.DrawSpacing(1);

            ImGui.Text("Trigger Conditions");
            string[] operatorOptions = TriggerOptions.OperatorOptions;
            float optionsWidth = 100 * _scale + padX;
            float opComboWidth = 55 * _scale;
            float valueInputWidth = 45 * _scale;
            float padWidth = 0;

            DrawHelpers.DrawNestIndicator(1);
            ImGui.Checkbox("Level", ref Level);
            if (Level)
            {
                ImGui.SameLine();
                padWidth = ImGui.CalcItemWidth() - ImGui.GetCursorPosX() - optionsWidth + padX;
                ImGui.SetCursorPosX(ImGui.GetCursorPosX() + padWidth);
                ImGui.PushItemWidth(opComboWidth);
                ImGui.Combo("##LevelOpCombo", ref Unsafe.As<TriggerDataOp, int>(ref LevelOp), operatorOptions, operatorOptions.Length);
                ImGui.PopItemWidth();
                ImGui.SameLine();

                if (string.IsNullOrEmpty(_levelValueInput))
                {
                    _levelValueInput = LevelValue.ToString();
                }

                ImGui.PushItemWidth(valueInputWidth);
                if (ImGui.InputText("##LevelValue", ref _levelValueInput, 10, ImGuiInputTextFlags.CharsDecimal))
                {
                    if (float.TryParse(_levelValueInput, out float value))
                    {
                        LevelValue = value;
                    }

                    _levelValueInput = LevelValue.ToString();
                }

                ImGui.PopItemWidth();
            }

            DrawHelpers.DrawNestIndicator(1);
            ImGui.Checkbox("HP", ref Hp);
            if (Hp)
            {
                ImGui.SameLine();
                padWidth = ImGui.CalcItemWidth() - ImGui.GetCursorPosX() - optionsWidth + padX;
                ImGui.SetCursorPosX(ImGui.GetCursorPosX() + padWidth);
                ImGui.PushItemWidth(opComboWidth);
                ImGui.Combo("##HpOpCombo", ref Unsafe.As<TriggerDataOp, int>(ref HpOp), operatorOptions, operatorOptions.Length);
                ImGui.PopItemWidth();
                ImGui.SameLine();

                if (string.IsNullOrEmpty(_hpValueInput))
                {
                    _hpValueInput = HpValue.ToString();
                }

                if (!MaxHp)
                {
                    ImGui.PushItemWidth(valueInputWidth);
                    if (ImGui.InputText("##HpValue", ref _hpValueInput, 10, ImGuiInputTextFlags.CharsDecimal))
                    {
                        if (float.TryParse(_hpValueInput, out float value))
                        {
                            HpValue = value;
                        }

                        _hpValueInput = HpValue.ToString();
                    }

                    ImGui.PopItemWidth();

                    ImGui.SameLine();
                    ImGui.Checkbox("%##HPPercent", ref HpPercent);
                }

                ImGui.SameLine();
                ImGui.Checkbox("Max HP", ref MaxHp);
            }

            DrawHelpers.DrawNestIndicator(1);
            ImGui.Checkbox("MP", ref Mp);
            if (Mp)
            {
                ImGui.SameLine();
                padWidth = ImGui.CalcItemWidth() - ImGui.GetCursorPosX() - optionsWidth + padX;
                ImGui.SetCursorPosX(ImGui.GetCursorPosX() + padWidth);
                ImGui.PushItemWidth(opComboWidth);
                ImGui.Combo("##MpOpCombo", ref Unsafe.As<TriggerDataOp, int>(ref MpOp), operatorOptions, operatorOptions.Length);
                ImGui.PopItemWidth();
                ImGui.SameLine();

                if (string.IsNullOrEmpty(_mpValueInput))
                {
                    _mpValueInput = MpValue.ToString();
                }

                if (!MaxMp)
                {
                    ImGui.PushItemWidth(valueInputWidth);
                    if (ImGui.InputText("##MpValue", ref _mpValueInput, 10, ImGuiInputTextFlags.CharsDecimal))
                    {
                        if (float.TryParse(_mpValueInput, out float value))
                        {
                            MpValue = value;
                        }

                        _mpValueInput = MpValue.ToString();
                    }

                    ImGui.PopItemWidth();

                    ImGui.SameLine();
                    ImGui.Checkbox("%##MPPercent", ref MpPercent);
                }

                ImGui.SameLine();
                ImGui.Checkbox("Max MP", ref MaxMp);
            }

            DrawHelpers.DrawNestIndicator(1);
            ImGui.Checkbox("CP", ref Cp);
            if (Cp)
            {
                ImGui.SameLine();
                padWidth = ImGui.CalcItemWidth() - ImGui.GetCursorPosX() - optionsWidth + padX;
                ImGui.SetCursorPosX(ImGui.GetCursorPosX() + padWidth);
                ImGui.PushItemWidth(opComboWidth);
                ImGui.Combo("##CpOpCombo", ref Unsafe.As<TriggerDataOp, int>(ref CpOp), operatorOptions, operatorOptions.Length);
                ImGui.PopItemWidth();
                ImGui.SameLine();

                if (string.IsNullOrEmpty(_cpValueInput))
                {
                    _cpValueInput = CpValue.ToString();
                }

                if (!MaxCp)
                {
                    ImGui.PushItemWidth(valueInputWidth);
                    if (ImGui.InputText("##CpValue", ref _cpValueInput, 10, ImGuiInputTextFlags.CharsDecimal))
                    {
                        if (float.TryParse(_cpValueInput, out float value))
                        {
                            CpValue = value;
                        }

                        _cpValueInput = CpValue.ToString();
                    }

                    ImGui.PopItemWidth();

                    ImGui.SameLine();
                    ImGui.Checkbox("%##CPPercent", ref CpPercent);
                }

                ImGui.SameLine();
                ImGui.Checkbox("Max CP", ref MaxCp);
            }

            DrawHelpers.DrawNestIndicator(1);
            ImGui.Checkbox("GP", ref Gp);
            if (Gp)
            {
                ImGui.SameLine();
                padWidth = ImGui.CalcItemWidth() - ImGui.GetCursorPosX() - optionsWidth + padX;
                ImGui.SetCursorPosX(ImGui.GetCursorPosX() + padWidth);
                ImGui.PushItemWidth(opComboWidth);
                ImGui.Combo("##GpOpCombo", ref Unsafe.As<TriggerDataOp, int>(ref GpOp), operatorOptions, operatorOptions.Length);
                ImGui.PopItemWidth();
                ImGui.SameLine();

                if (string.IsNullOrEmpty(_gpValueInput))
                {
                    _gpValueInput = GpValue.ToString();
                }

                if (!MaxGp)
                {
                    ImGui.PushItemWidth(valueInputWidth);
                    if (ImGui.InputText("##GpValue", ref _gpValueInput, 10, ImGuiInputTextFlags.CharsDecimal))
                    {
                        if (float.TryParse(_gpValueInput, out float value))
                        {
                            GpValue = value;
                        }

                        _gpValueInput = GpValue.ToString();
                    }

                    ImGui.PopItemWidth();

                    ImGui.SameLine();
                    ImGui.Checkbox("%##GPPercent", ref GpPercent);
                }

                ImGui.SameLine();
                ImGui.Checkbox("Max GP", ref MaxGp);
            }

            if (TriggerSource == TriggerSource.Player)
            {
                DrawHelpers.DrawNestIndicator(1);
                ImGui.Checkbox("Pet", ref PetCheck);
                if (PetCheck)
                {
                    ImGui.SameLine();
                    padWidth = ImGui.CalcItemWidth() - ImGui.GetCursorPosX() - optionsWidth + padX;
                    ImGui.SetCursorPosX(ImGui.GetCursorPosX() + padWidth);
                    ImGui.PushItemWidth(optionsWidth);
                    ImGui.Combo("##PetCombo", ref PetValue, _petOptions, _petOptions.Length);
                    ImGui.PopItemWidth();
                }
            }
        }
    }
}