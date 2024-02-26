using Dalamud.Interface.Utility;
using DelvCD.Helpers;
using DelvCD.Helpers.DataSources;
using ImGuiNET;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;

namespace DelvCD.Config
{
    public class BarStyleConfig : IConfigPage
    {
        [JsonIgnore] private float _scale => ImGuiHelpers.GlobalScale;

        [JsonIgnore] public string Name => "Bar";

        [JsonIgnore] private Vector2 _screenSize = ImGui.GetMainViewport().Size;

        [JsonIgnore] private string[] _directionOptions = Enum.GetNames<BarDirection>();
        [JsonIgnore] private string[] _thresholdOpOptions = new string[] { "<", ">", "<=", ">=" };

        [JsonIgnore] private DataSource[] _dataSources = new DataSource[] { };
        [JsonIgnore] private string[] _progressDataSourceOptions = new string[] { };
        [JsonIgnore] private string[] _progressDataSourceFieldOptions = new string[] { };
        [JsonIgnore] private string[] _chunkStyles = Enum.GetNames<ChunkStyles>();

        public int ProgressDataSourceIndex = 0;
        public int ProgressDataSourceFieldIndex = 0;
        public bool InvertValues = false;

        public Vector2 Position = Vector2.Zero;
        public Vector2 Size = new Vector2(200, 20);
        public BarDirection Direction = BarDirection.Right;
        public ConfigColor FillColor = new ConfigColor(1, 0.5f, 0.5f, 1);
        public ConfigColor BackgroundColor = new ConfigColor(0, 0, 0, 0.5f);

        public bool ShowBorder = true;
        public int BorderThickness = 1;
        public ConfigColor BorderColor = new ConfigColor(0, 0, 0, 1);

        public bool Chunked = false;
        public bool ChunkedStacksFromTrigger = true;
        public int ChunkCount = 5;
        public int ChunkPadding = 2;
        public ConfigColor IncompleteChunkColor = new ConfigColor(0.6f, 0.6f, 0.6f, 1);
        public string ChunkStyle = "Default";
        public int ChunkStylesIndex = 0;
        public float Radius = 0;
        public int NgonSides = 0;

        public bool Glow = false;
        public int GlowThickness = 2;
        public int GlowSegments = 8;
        public float GlowSpeed = 1f;
        public ConfigColor GlowColor = new ConfigColor(230f / 255f, 150f / 255f, 0f / 255f, 1f);
        public ConfigColor GlowColor2 = new ConfigColor(0f / 255f, 0f / 255f, 0f / 255f, 0f);

        public IConfigPage GetDefault() => new BarStyleConfig();


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

            ProgressDataSourceIndex = Math.Clamp(ProgressDataSourceIndex, 0, dataSources.Length - 1);

            List<string> list = new();
            for (int i = 0; i < dataSources.Length; i++)
            {
                list.Add("Trigger " + (i + 1));
            }

            _progressDataSourceOptions = list.ToArray();
            _progressDataSourceFieldOptions = dataSources[ProgressDataSourceIndex].ProgressFieldNames.ToArray();

            ProgressDataSourceFieldIndex = Math.Clamp(ProgressDataSourceFieldIndex, 0, _progressDataSourceFieldOptions.Length - 1);
        }

        public void DrawConfig(IConfigurable parent, Vector2 size, float padX, float padY)
        {
            if (ImGui.BeginChild("##BarStyleConfig", new Vector2(size.X, size.Y), true))
            {
                if (_dataSources.Length > 0)
                {
                    ImGui.PushItemWidth(100 * _scale);
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
                }
                else
                {
                    ImGui.Text("No applicable data found in triggers!");
                }

                // base config
                ImGui.NewLine();
                ImGui.DragFloat2("Position", ref Position, 1, -_screenSize.X / 2, _screenSize.X / 2);
                ImGui.DragFloat2("Size", ref Size, 1, 0, _screenSize.Y);

                ImGui.Combo("Fill Direction", ref Unsafe.As<BarDirection, int>(ref Direction), _directionOptions, _directionOptions.Length);

                Vector4 vector = FillColor.Vector;
                if (ImGui.ColorEdit4("Fill Color", ref vector, ImGuiColorEditFlags.AlphaPreview | ImGuiColorEditFlags.AlphaBar))
                {
                    FillColor.Vector = vector;
                }

                vector = BackgroundColor.Vector;
                if (ImGui.ColorEdit4("Background Color", ref vector, ImGuiColorEditFlags.AlphaPreview | ImGuiColorEditFlags.AlphaBar))
                {
                    BackgroundColor.Vector = vector;
                }

                // border
                ImGui.NewLine();
                ImGui.Checkbox("Show Border", ref ShowBorder);
                if (ShowBorder)
                {
                    DrawHelpers.DrawNestIndicator(1);
                    ImGui.DragInt("Border Thickness", ref BorderThickness, 1, 1, 100);

                    DrawHelpers.DrawNestIndicator(1);
                    vector = BorderColor.Vector;
                    if (ImGui.ColorEdit4("Border Color", ref vector, ImGuiColorEditFlags.AlphaPreview | ImGuiColorEditFlags.AlphaBar))
                    {
                        BorderColor.Vector = vector;
                    }
                }

                // chunked
                ImGui.NewLine();
                ImGui.Checkbox("Draw in Chunks", ref Chunked);
                if (Chunked)
                {

                    ImGui.PushItemWidth(100 * _scale);
                    ImGui.Combo("Chunk Style", ref ChunkStylesIndex, _chunkStyles, _chunkStyles.Length);
                    ImGui.PopItemWidth();

                    DrawHelpers.DrawNestIndicator(1);
                    if (ImGui.RadioButton("Stacks from Trigger", ChunkedStacksFromTrigger))
                    {
                        ChunkedStacksFromTrigger = true;
                    }

                    if (ImGui.IsItemHovered())
                    {
                        ImGui.SetTooltip("Assumes the trigger data to be stacks and each chunk will represent one stack.");
                    }

                    ImGui.SameLine();
                    if (ImGui.RadioButton("Custom", !ChunkedStacksFromTrigger))
                    {
                        ChunkedStacksFromTrigger = false;
                    }

                    if (!ChunkedStacksFromTrigger)
                    {
                        DrawHelpers.DrawNestIndicator(1);
                        ImGui.DragInt("Chunk Count", ref ChunkCount, 0.1f, 1, 200);
                    }

                    if (ChunkStylesIndex > 0)
                    {
                        DrawHelpers.DrawNestIndicator(1);
                        if (Direction == BarDirection.Left || Direction == BarDirection.Right)
                        {
                            ImGui.DragFloat("Radius", ref Size.Y, 1, 0, _screenSize.X / 2); Radius = Size.Y;
                            if (ChunkStylesIndex == 2)
                            {
                                DrawHelpers.DrawNestIndicator(1);
                                ImGui.DragInt("Sides", ref NgonSides, 1, 3, 12);
                            }
                            DrawHelpers.DrawNestIndicator(1);
                            ImGui.DragFloat("Chunk Padding", ref Size.X, 1, 0, _screenSize.X / 2);
                        }
                        if (Direction == BarDirection.Up || Direction == BarDirection.Down)
                        {
                            ImGui.DragFloat("Radius", ref Size.X, 1, 0, _screenSize.X / 2); Radius = Size.X;
                            if (ChunkStylesIndex == 2)
                            {
                                DrawHelpers.DrawNestIndicator(1);
                                ImGui.DragInt("Sides", ref NgonSides, 1, 3, 12);
                            }
                            DrawHelpers.DrawNestIndicator(1);
                            ImGui.DragFloat("Chunk Padding", ref Size.Y, 1, 0, _screenSize.X / 2);
                        }
                    }
                    else
                    {
                        DrawHelpers.DrawNestIndicator(1);
                        ImGui.DragInt("Chunk Padding", ref ChunkPadding, 0.1f, 0, 50);
                    }

                    DrawHelpers.DrawNestIndicator(1);
                    vector = IncompleteChunkColor.Vector;
                    if (ImGui.ColorEdit4("Incomplete Chunk Color", ref vector, ImGuiColorEditFlags.AlphaPreview | ImGuiColorEditFlags.AlphaBar))
                    {
                        IncompleteChunkColor.Vector = vector;
                    }
                }

                // glow
                ImGui.NewLine();
                ImGui.Checkbox("Glow", ref Glow);
                if (Glow)
                {
                    DrawHelpers.DrawNestIndicator(1);
                    ImGui.DragInt("Thickness##Glow", ref GlowThickness, 1, 1, 16);


                    if (ChunkStylesIndex == 0)
                    {
                        DrawHelpers.DrawNestIndicator(1);
                        ImGui.DragInt("Glow Segments##Glow", ref GlowSegments, 1, 2, 16);
                    }

                    DrawHelpers.DrawNestIndicator(1);
                    ImGui.DragFloat("Animation Speed##Glow", ref GlowSpeed, 0.05f, 0, 2f);

                    DrawHelpers.DrawNestIndicator(1);
                    vector = GlowColor.Vector;
                    if (ImGui.ColorEdit4("Glow Color##Glow", ref vector, ImGuiColorEditFlags.AlphaPreview | ImGuiColorEditFlags.AlphaBar))
                    {
                        GlowColor.Vector = vector;
                    }

                    DrawHelpers.DrawNestIndicator(1);
                    vector = GlowColor2.Vector;
                    if (ImGui.ColorEdit4("Glow Color 2##Glow", ref vector, ImGuiColorEditFlags.AlphaPreview | ImGuiColorEditFlags.AlphaBar))
                    {
                        GlowColor2.Vector = vector;
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
