using Dalamud.Interface;
using DelvCD.Helpers;
using DelvCD.Helpers.DataSources;
using ImGuiNET;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace DelvCD.Config
{
    public class IconStyleConfig : IConfigPage
    {
        [JsonIgnore] private float _scale => ImGuiHelpers.GlobalScale;

        [JsonIgnore] public string Name => "Icon";

        [JsonIgnore] private string _iconSearchInput = string.Empty;
        [JsonIgnore] private List<TriggerData> _iconSearchResults = new List<TriggerData>();
        [JsonIgnore] private Vector2 _screenSize = ImGui.GetMainViewport().Size;

        [JsonIgnore] private DataSource[] _dataSources = new DataSource[] { };
        [JsonIgnore] private string[] _progressDataSourceOptions = new string[] { };
        [JsonIgnore] private string[] _progressDataSourceFieldOptions = new string[] { };

        public Vector2 Position = Vector2.Zero;
        public Vector2 Size = new Vector2(40, 40);
        public bool ShowBorder = true;
        public int BorderThickness = 1;
        public ConfigColor BorderColor = new ConfigColor(0, 0, 0, 1);
        public bool ShowProgressSwipe = true;
        public int ProgressDataSourceIndex = 0;
        public int ProgressDataSourceFieldIndex = 0;
        public bool InvertValues = false;
        public float ProgressSwipeOpacity = 0.6f;
        public bool InvertSwipe = false;
        public bool ShowSwipeLines = false;
        public ConfigColor ProgressLineColor = new ConfigColor(1, 1, 1, 1);
        public int ProgressLineThickness = 2;
        public bool GcdSwipe = false;
        public bool GcdSwipeOnly = false;

        public bool DesaturateIcon = false;
        public float Opacity = 1f;

        public int IconOption = 0;
        public uint CustomIcon = 0;
        public bool CropIcon = false;

        public bool Glow = false;
        public int GlowThickness = 2;
        public int GlowSegments = 8;
        public float GlowSpeed = 1f;
        public ConfigColor GlowColor = new ConfigColor(230f / 255f, 150f / 255f, 0f / 255f, 1f);
        public ConfigColor GlowColor2 = new ConfigColor(0f / 255f, 0f / 255f, 0f / 255f, 0f);

        public ConfigColor IconColor = new ConfigColor(1, 0, 0, 1);

        public IConfigPage GetDefault() => new IconStyleConfig();


        public void UpdateDataSources(DataSource[] dataSources)
        {
            _dataSources = dataSources.Where(x => x.ProgressFieldNames.Count > 0).ToArray();

            if (_dataSources.Length == 0)
            {
                _progressDataSourceOptions = new string[] { };
                _progressDataSourceFieldOptions = new string[] { };
                ProgressDataSourceIndex = 0;
                ProgressDataSourceFieldIndex = 0;
                return;
            }

            ProgressDataSourceIndex = Math.Clamp(ProgressDataSourceIndex, 0, dataSources.Length);

            _progressDataSourceOptions = new string[dataSources.Length];
            for (int i = 0; i < dataSources.Length; i++)
            {
                _progressDataSourceOptions[i] = $"Trigger {i + 1}";
            }
            
            _progressDataSourceFieldOptions = dataSources[ProgressDataSourceIndex].ProgressFieldNames.ToArray();

            ProgressDataSourceFieldIndex = Math.Clamp(ProgressDataSourceFieldIndex, 0, _progressDataSourceFieldOptions.Length);
        }

        public void DrawConfig(IConfigurable parent, Vector2 size, float padX, float padY)
        {
            if (ImGui.BeginChild("##IconStyleConfig", new Vector2(size.X, size.Y), true))
            {
                float height = 50 * _scale;
                if (IconOption == 1 && CustomIcon > 0)
                {
                    Vector2 iconPos = ImGui.GetWindowPos() + new Vector2(padX, padX);
                    Vector2 iconSize = new Vector2(height, height);
                    DrawIconPreview(iconPos, iconSize, CustomIcon, CropIcon, DesaturateIcon, false);
                    ImGui.GetWindowDrawList().AddRect(
                        iconPos,
                        iconPos + iconSize,
                        ImGui.ColorConvertFloat4ToU32(ImGui.GetStyle().Colors[(int)ImGuiCol.Border]));

                    ImGui.SetCursorPosX(ImGui.GetCursorPosX() + height + padX);
                }

                ImGui.RadioButton("Automatic Icon", ref IconOption, 0);
                ImGui.SameLine();
                ImGui.RadioButton("Custom Icon", ref IconOption, 1);
                ImGui.SameLine();
                ImGui.RadioButton("No Icon", ref IconOption, 2);
                ImGui.SameLine();
                ImGui.RadioButton("Solid Color", ref IconOption, 3);

                if (IconOption == 1)
                {
                    float width = ImGui.CalcItemWidth();
                    if (CustomIcon > 0)
                    {
                        width -= height + padX;
                        ImGui.SetCursorPosX(ImGui.GetCursorPosX() + height + padX);
                    }

                    ImGui.PushItemWidth(width);
                    if (ImGui.InputTextWithHint("Search", "Search Icons by Name or ID", ref _iconSearchInput, 32, ImGuiInputTextFlags.EnterReturnsTrue))
                    {
                        _iconSearchResults.Clear();
                        if (ushort.TryParse(_iconSearchInput, out ushort iconId))
                        {
                            _iconSearchResults.Add(new TriggerData("", 0, iconId));
                        }
                        else if (!string.IsNullOrEmpty(_iconSearchInput))
                        {
                            _iconSearchResults.AddRange(ActionHelpers.FindActionEntries(_iconSearchInput));
                            _iconSearchResults.AddRange(StatusHelpers.FindStatusEntries(_iconSearchInput));
                            _iconSearchResults.AddRange(ActionHelpers.FindItemEntries(_iconSearchInput));
                        }
                    }
                    ImGui.PopItemWidth();

                    if (_iconSearchResults.Any() && ImGui.BeginChild("##IconPicker", new Vector2(size.X - padX * 2, 60 * _scale), true))
                    {
                        List<uint> icons = _iconSearchResults.Select(t => t.Icon).Distinct().ToList();
                        for (int i = 0; i < icons.Count; i++)
                        {
                            Vector2 iconPos = ImGui.GetWindowPos().AddX(10 * _scale) + new Vector2(i * (40 * _scale + padX), padY);
                            Vector2 iconSize = new Vector2(40 * _scale, 40 * _scale);
                            DrawIconPreview(iconPos, iconSize, icons[i], CropIcon, false, true);

                            if (ImGui.IsMouseHoveringRect(iconPos, iconPos + iconSize))
                            {
                                if (ImGui.IsMouseClicked(ImGuiMouseButton.Left))
                                {
                                    CustomIcon = icons[i];
                                    _iconSearchResults.Clear();
                                    _iconSearchInput = string.Empty;
                                }
                            }
                        }

                        ImGui.EndChild();
                    }
                }

                if (IconOption != 2)
                {
                    if (IconOption < 2)
                    {
                        ImGui.Checkbox("Crop Icon", ref CropIcon);
                    }
                    else if (IconOption == 3)
                    {
                        Vector4 vector = IconColor.Vector;
                        ImGui.ColorEdit4("Icon Color", ref vector, ImGuiColorEditFlags.AlphaPreview | ImGuiColorEditFlags.AlphaBar);
                        IconColor.Vector = vector;
                    }

                    DrawHelpers.DrawSpacing(1);
                    ImGui.DragFloat2("Position", ref Position, 1, -_screenSize.X / 2, _screenSize.X / 2);
                    ImGui.DragFloat2("Icon Size", ref Size, 1, 0, _screenSize.Y);

                    if (IconOption < 2)
                    {
                        ImGui.DragFloat("Icon Opacity", ref Opacity, .01f, 0, 1);
                        ImGui.Checkbox("Desaturate Icon", ref DesaturateIcon);
                    }

                    ImGui.Checkbox("Glow", ref Glow);
                    if (Glow)
                    {
                        DrawHelpers.DrawNestIndicator(1);
                        ImGui.DragInt("Thickness##Glow", ref GlowThickness, 1, 1, 16);

                        DrawHelpers.DrawNestIndicator(1);
                        ImGui.DragInt("Glow Segments##Glow", ref GlowSegments, 1, 2, 16);

                        DrawHelpers.DrawNestIndicator(1);
                        ImGui.DragFloat("Animation Speed##Glow", ref GlowSpeed, 0.05f, 0, 2f);

                        DrawHelpers.DrawNestIndicator(1);
                        Vector4 vector = GlowColor.Vector;
                        ImGui.ColorEdit4("Glow Color##Glow", ref vector, ImGuiColorEditFlags.AlphaPreview | ImGuiColorEditFlags.AlphaBar);
                        GlowColor.Vector = vector;

                        DrawHelpers.DrawNestIndicator(1);
                        vector = GlowColor2.Vector;
                        ImGui.ColorEdit4("Glow Color 2##Glow", ref vector, ImGuiColorEditFlags.AlphaPreview | ImGuiColorEditFlags.AlphaBar);
                        GlowColor2.Vector = vector;
                    }

                    DrawHelpers.DrawSpacing(1);
                    ImGui.Checkbox("Show Border", ref ShowBorder);
                    if (ShowBorder)
                    {
                        DrawHelpers.DrawNestIndicator(1);
                        ImGui.DragInt("Border Thickness", ref BorderThickness, 1, 1, 100);

                        DrawHelpers.DrawNestIndicator(1);
                        Vector4 vector = BorderColor.Vector;
                        ImGui.ColorEdit4("Border Color", ref vector, ImGuiColorEditFlags.AlphaPreview | ImGuiColorEditFlags.AlphaBar);
                        BorderColor.Vector = vector;
                    }

                    if (_dataSources.Length > 0)
                    {
                        ImGui.NewLine();
                        ImGui.Checkbox("Show Progress Swipe", ref ShowProgressSwipe);
                        if (ShowProgressSwipe)
                        {
                            ImGui.PushItemWidth(100 * _scale);
                            DrawHelpers.DrawNestIndicator(1);
                            if (ImGui.Combo("##DataSourceCombo", ref ProgressDataSourceIndex, _progressDataSourceOptions, _progressDataSourceOptions.Length))
                            {
                                ProgressDataSourceFieldIndex = 0;
                                _progressDataSourceFieldOptions = _dataSources[ProgressDataSourceIndex].ProgressFieldNames.ToArray();
                            }
                            ImGui.PopItemWidth();

                            ImGui.SameLine();
                            ImGui.PushItemWidth(200 * _scale);
                            ImGui.Combo("##DataSourceFieldCombo", ref ProgressDataSourceFieldIndex, _progressDataSourceFieldOptions, _progressDataSourceFieldOptions.Length);
                            ImGui.PopItemWidth();

                            ImGui.SameLine();
                            ImGui.Checkbox("Invert Values", ref InvertValues);

                            DrawHelpers.DrawNestIndicator(1);
                            ImGui.DragFloat("Swipe Opacity", ref ProgressSwipeOpacity, .01f, 0, 1);
                            DrawHelpers.DrawNestIndicator(1);
                            ImGui.Checkbox("Invert Swipe", ref InvertSwipe);
                            DrawHelpers.DrawNestIndicator(1);
                            ImGui.Checkbox("Show GCD Swipe When Inactive", ref GcdSwipe);
                            if (GcdSwipe)
                            {
                                DrawHelpers.DrawNestIndicator(2);
                                ImGui.Checkbox("Only show GCD swipe", ref GcdSwipeOnly);
                            }

                            DrawHelpers.DrawNestIndicator(1);
                            ImGui.Checkbox("Show Swipe Lines", ref ShowSwipeLines);
                            if (ShowSwipeLines)
                            {
                                DrawHelpers.DrawNestIndicator(2);
                                Vector4 vector = ProgressLineColor.Vector;
                                ImGui.ColorEdit4("Line Color", ref vector, ImGuiColorEditFlags.AlphaPreview | ImGuiColorEditFlags.AlphaBar);
                                ProgressLineColor.Vector = vector;
                                DrawHelpers.DrawNestIndicator(2);
                                ImGui.DragInt("Thickness", ref ProgressLineThickness, 1, 1, 5);
                            }
                        }
                    }
                }
            }

            ImGui.EndChild();
        }

        private void DrawIconPreview(Vector2 iconPos, Vector2 iconSize, uint icon, bool crop, bool desaturate, bool text)
        {
            ImDrawListPtr drawList = ImGui.GetWindowDrawList();
            DrawHelpers.DrawIcon(icon, iconPos, iconSize, crop, 0, desaturate, 1f, drawList);
            if (text)
            {
                string iconText = icon.ToString();
                Vector2 iconTextPos = iconPos + new Vector2(20 - ImGui.CalcTextSize(iconText).X / 2, 38);
                drawList.AddText(iconTextPos, 0xFFFFFFFF, iconText);
            }
        }
    }
}