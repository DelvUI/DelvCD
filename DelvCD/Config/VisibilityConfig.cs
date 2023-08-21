﻿using Dalamud.Interface;
using DelvCD.Helpers;
using ImGuiNET;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;

namespace DelvCD.Config
{

    public class VisibilityConfig : IConfigPage
    {
        [JsonIgnore] private float _scale => ImGuiHelpers.GlobalScale;

        [JsonIgnore] private string _customJobInput = string.Empty;

        public string Name => "Visibility";

        public bool AlwaysHide = false;
        public bool HideInPvP = false;
        public bool HideOutsidePvP = false;
        public bool HideInCombat = false;
        public bool HideOutsideCombat = false;
        public bool HideOutsideDuty = false;
        public bool HideWhilePerforming = false;
        public bool HideInGoldenSaucer = false;
        public bool HideWhenSheathed = false;
        public bool IgnoreInCombat = false;
        public bool IgnoreInDuty = false;
        public bool Clip = false;

        public bool HideIfLevel = false;
        public TriggerDataOp HideIfLevelOp = TriggerDataOp.LessThan;
        public int HideIfLevelValue = 90;

        public JobType ShowForJobTypes = JobType.All;
        public string CustomJobString = string.Empty;
        public List<Job> CustomJobList = new List<Job>();

        public IConfigPage GetDefault() => new VisibilityConfig();

        public bool IsVisible(bool parentVisible)
        {
            if (this.AlwaysHide)
            {
                return false;
            }

            if (this.HideInPvP && CharacterState.IsInPvP())
            {
                return false;
            }

            if (this.HideOutsidePvP && !CharacterState.IsInPvP())
            {
                return false;
            }

            if (this.HideInCombat && CharacterState.IsInCombat())
            {
                return false;
            }

            if (this.HideOutsideCombat && !CharacterState.IsInCombat())
            {
                return false;
            }

            if (this.HideOutsideDuty && !CharacterState.IsInDuty())
            {
                return false;
            }

            if (this.HideWhilePerforming && CharacterState.IsPerforming())
            {
                return false;
            }

            if (this.HideInGoldenSaucer && CharacterState.IsInGoldenSaucer())
            {
                return false;
            }

            if (this.HideWhenSheathed && !CharacterState.IsWeaponDrawn()
                && (!this.IgnoreInCombat || (this.IgnoreInCombat && !CharacterState.IsInCombat()))
                && (!this.IgnoreInDuty) || (this.IgnoreInDuty && !CharacterState.IsInDuty()))
            {
                return false;
            }

            if (this.HideIfLevel &&
                Utils.GetResult(CharacterState.GetCharacterLevel(), this.HideIfLevelOp, this.HideIfLevelValue))
            {
                return false;
            }

            return parentVisible && CharacterState.IsJobType(CharacterState.GetCharacterJob(), this.ShowForJobTypes, this.CustomJobList);
        }

        public void DrawConfig(IConfigurable parent, Vector2 size, float padX, float padY)
        {
            if (ImGui.BeginChild("##VisibilityConfig", new Vector2(size.X, size.Y), true))
            {
                ImGui.Checkbox("Always Hide", ref this.AlwaysHide);
                ImGui.Checkbox("Hide In PvP", ref this.HideInPvP);
                ImGui.Checkbox("Hide Outside PvP", ref this.HideOutsidePvP);
                ImGui.Checkbox("Hide In Combat", ref this.HideInCombat);
                ImGui.Checkbox("Hide Outside Combat", ref this.HideOutsideCombat);
                ImGui.Checkbox("Hide Outside Duty", ref this.HideOutsideDuty);
                ImGui.Checkbox("Hide While Performing", ref this.HideWhilePerforming);
                ImGui.Checkbox("Hide In Golden Saucer", ref this.HideInGoldenSaucer);
                ImGui.Checkbox("Hide While Weapon Sheathed", ref this.HideWhenSheathed);
                if (this.HideWhenSheathed)
                {
                    DrawHelpers.DrawNestIndicator(1);
                    ImGui.Checkbox("Ignore Sheathed status in Combat", ref this.IgnoreInCombat);
                    if (ImGui.IsItemHovered()) { ImGui.SetTooltip("Show when weapon is sheathed during combat."); }

                    DrawHelpers.DrawNestIndicator(1);
                    ImGui.Checkbox("Ignore Sheathed status in Duty", ref this.IgnoreInDuty);
                    if (ImGui.IsItemHovered()) { ImGui.SetTooltip("Show when weapon is sheathed in a duty."); }
                }
                else { this.IgnoreInCombat = false; this.IgnoreInDuty = false; }

                DrawHelpers.DrawSpacing();
                ImGui.Checkbox("Hide if Level", ref this.HideIfLevel);
                if (this.HideIfLevel)
                {
                    ImGui.SameLine();
                    string[] options = TriggerOptions.OperatorOptions;
                    ImGui.PushItemWidth(55 * _scale);
                    ImGui.Combo("##HideIfLevelOpCombo", ref Unsafe.As<TriggerDataOp, int>(ref this.HideIfLevelOp), options, options.Length);
                    ImGui.PopItemWidth();

                    ImGui.SameLine();
                    ImGui.PushItemWidth(45 * _scale);
                    ImGui.InputInt(string.Empty, ref this.HideIfLevelValue, 0, 0);
                    ImGui.PopItemWidth();
                }

                DrawHelpers.DrawSpacing();
                string[] jobTypeOptions = Enum.GetNames(typeof(JobType));
                ImGui.Combo("Show for Jobs", ref Unsafe.As<JobType, int>(ref this.ShowForJobTypes), jobTypeOptions, jobTypeOptions.Length);

                if (this.ShowForJobTypes == JobType.Custom)
                {
                    if (string.IsNullOrEmpty(_customJobInput))
                    {
                        _customJobInput = this.CustomJobString.ToUpper();
                    }

                    if (ImGui.InputTextWithHint("Custom Job List", "Comma Separated List (ex: WAR, SAM, BLM)", ref _customJobInput, 100, ImGuiInputTextFlags.EnterReturnsTrue))
                    {
                        IEnumerable<string> jobStrings = _customJobInput.Split(',').Select(j => j.Trim());
                        List<Job> jobList = new List<Job>();
                        foreach (string j in jobStrings)
                        {
                            if (Enum.TryParse(j, true, out Job parsed))
                            {
                                jobList.Add(parsed);
                            }
                            else
                            {
                                jobList.Clear();
                                _customJobInput = string.Empty;
                                break;
                            }
                        }

                        _customJobInput = _customJobInput.ToUpper();
                        this.CustomJobString = _customJobInput;
                        this.CustomJobList = jobList;
                    }
                }

                if (parent is DelvCDConfig)
                {
                    DrawHelpers.DrawSpacing();
                    ImGui.Checkbox("Enable Window Clipping", ref this.Clip);
                }

                ImGui.EndChild();
            }
        }
    }
}
