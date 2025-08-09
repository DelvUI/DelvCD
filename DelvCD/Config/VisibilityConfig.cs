using Dalamud.Interface.Utility;
using DelvCD.Helpers;
using Dalamud.Bindings.ImGui;
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
        public int HideIfLevelValue = 100;

        public JobType ShowForJobTypes = JobType.All;
        public string CustomJobString = string.Empty;
        public List<Job> CustomJobList = new List<Job>();

        public IConfigPage GetDefault() => new VisibilityConfig();

        public bool IsVisible(bool parentVisible)
        {
            if (AlwaysHide)
            {
                return false;
            }

            if (HideInPvP && CharacterState.IsInPvP())
            {
                return false;
            }

            if (HideOutsidePvP && !CharacterState.IsInPvP())
            {
                return false;
            }

            if (HideInCombat && CharacterState.IsInCombat())
            {
                return false;
            }

            if (HideOutsideCombat && !CharacterState.IsInCombat())
            {
                return false;
            }

            if (HideOutsideDuty && !CharacterState.IsInDuty())
            {
                return false;
            }

            if (HideWhilePerforming && CharacterState.IsPerforming())
            {
                return false;
            }

            if (HideInGoldenSaucer && CharacterState.IsInGoldenSaucer())
            {
                return false;
            }

            if (HideWhenSheathed && !CharacterState.IsWeaponDrawn()
                && (!IgnoreInCombat || (IgnoreInCombat && !CharacterState.IsInCombat()))
                && (!IgnoreInDuty || (IgnoreInDuty && !CharacterState.IsInDuty())))
            {
                return false;
            }

            if (HideIfLevel &&
                Utils.GetResult(CharacterState.GetCharacterLevel(), HideIfLevelOp, HideIfLevelValue))
            {
                return false;
            }

            return parentVisible && CharacterState.IsJobType(CharacterState.GetCharacterJob(), ShowForJobTypes, CustomJobList);
        }

        public void DrawConfig(IConfigurable parent, Vector2 size, float padX, float padY)
        {
            if (ImGui.BeginChild("##VisibilityConfig", new Vector2(size.X, size.Y), true))
            {
                ImGui.Checkbox("Always Hide", ref AlwaysHide);
                ImGui.Checkbox("Hide In PvP", ref HideInPvP);
                ImGui.Checkbox("Hide Outside PvP", ref HideOutsidePvP);
                ImGui.Checkbox("Hide In Combat", ref HideInCombat);
                ImGui.Checkbox("Hide Outside Combat", ref HideOutsideCombat);
                ImGui.Checkbox("Hide Outside Duty", ref HideOutsideDuty);
                ImGui.Checkbox("Hide While Performing", ref HideWhilePerforming);
                ImGui.Checkbox("Hide In Golden Saucer", ref HideInGoldenSaucer);
                ImGui.Checkbox("Hide While Weapon Sheathed", ref HideWhenSheathed);
                if (HideWhenSheathed)
                {
                    DrawHelpers.DrawNestIndicator(1);
                    ImGui.Checkbox("Ignore Sheathed status in Combat", ref IgnoreInCombat);
                    if (ImGui.IsItemHovered()) { ImGui.SetTooltip("Show when weapon is sheathed during combat."); }

                    DrawHelpers.DrawNestIndicator(1);
                    ImGui.Checkbox("Ignore Sheathed status in Duty", ref IgnoreInDuty);
                    if (ImGui.IsItemHovered()) { ImGui.SetTooltip("Show when weapon is sheathed in a duty."); }
                }
                else { IgnoreInCombat = false; IgnoreInDuty = false; }

                DrawHelpers.DrawSpacing();
                ImGui.Checkbox("Hide if Level", ref HideIfLevel);
                if (HideIfLevel)
                {
                    ImGui.SameLine();
                    string[] options = TriggerOptions.OperatorOptions;
                    ImGui.PushItemWidth(55 * _scale);
                    ImGui.Combo("##HideIfLevelOpCombo", ref Unsafe.As<TriggerDataOp, int>(ref HideIfLevelOp), options, options.Length);
                    ImGui.PopItemWidth();

                    ImGui.SameLine();
                    ImGui.PushItemWidth(45 * _scale);
                    ImGui.InputInt(string.Empty, ref HideIfLevelValue, 0, 0);
                    ImGui.PopItemWidth();
                }

                DrawHelpers.DrawSpacing();
                string[] jobTypeOptions = Enum.GetNames(typeof(JobType));
                ImGui.Combo("Show for Jobs", ref Unsafe.As<JobType, int>(ref ShowForJobTypes), jobTypeOptions, jobTypeOptions.Length);

                if (ShowForJobTypes == JobType.Custom)
                {
                    if (string.IsNullOrEmpty(_customJobInput))
                    {
                        _customJobInput = CustomJobString.ToUpper();
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
                        CustomJobString = _customJobInput;
                        CustomJobList = jobList;
                    }
                }

                if (parent is DelvCDConfig)
                {
                    DrawHelpers.DrawSpacing();
                    ImGui.Checkbox("Enable Window Clipping", ref Clip);
                }
            }

            ImGui.EndChild();
        }
    }
}
