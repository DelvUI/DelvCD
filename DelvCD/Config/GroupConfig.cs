using Dalamud.Interface.Utility;
using DelvCD.UIElements;
using ImGuiNET;
using Newtonsoft.Json;
using System.Numerics;

namespace DelvCD.Config
{
    public class GroupConfig : IConfigPage
    {
        [JsonIgnore] private float _scale => ImGuiHelpers.GlobalScale;
        [JsonIgnore] private static Vector2 _screenSize = ImGui.GetMainViewport().Size;

        public string Name => "Group";
        public bool IsDynamic = false;
        public Vector2 DynamicOffset = new Vector2(0, 0);

        public Vector2 Position = new Vector2(0, 0);

        [JsonIgnore] private Vector2 _iconSize = new Vector2(40, 40);
        [JsonIgnore] private float _mX = 1f;
        [JsonIgnore] private float _mY = 1f;
        [JsonIgnore] private bool _recusiveResize = false;
        [JsonIgnore] private bool _conditionsResize = false;
        [JsonIgnore] private bool _positionOnly = false;

        public IConfigPage GetDefault() => new GroupConfig();

        public void DrawConfig(IConfigurable parent, Vector2 size, float padX, float padY)
        {
            if (ImGui.BeginChild("##GroupConfig", new Vector2(size.X, size.Y), true))
            {
                ImGui.DragFloat2("Group Position", ref Position);

                ImGui.NewLine();
                ImGui.Checkbox("Dynamic Grouping", ref IsDynamic);
                
                if (IsDynamic) {
                    ImGui.DragFloat2("Dynamic Offset", ref DynamicOffset);
                }

                ImGui.NewLine();
                ImGui.Text("Resize Icons");
                ImGui.DragFloat2("Icon Size##Size", ref _iconSize, 1, 0, _screenSize.Y);
                ImGui.Checkbox("Recursive##Size", ref _recusiveResize);
                if (ImGui.IsItemHovered())
                {
                    ImGui.SetTooltip("Check to recursively resize icons in sub-groups");
                }

                ImGui.SameLine();
                ImGui.Checkbox("Conditions##Size", ref _conditionsResize);
                if (ImGui.IsItemHovered())
                {
                    ImGui.SetTooltip("Check to resize conditions");
                }

                ImGui.SameLine();
                float padWidth = ImGui.CalcItemWidth() - ImGui.GetCursorPosX() - 60 + padX;
                ImGui.SetCursorPosX(ImGui.GetCursorPosX() + padWidth);
                if (ImGui.Button("Resize", new Vector2(60 * _scale, 0)))
                {
                    if (parent is Group g)
                    {
                        g.ResizeIcons(_iconSize, _recusiveResize, _conditionsResize);
                    }
                }

                if (parent is Group group)
                {
                    ImGui.NewLine();
                    ImGui.Text("Scale Resolution (BACK UP YOUR CONFIG FIRST!)");
                    ImGui.DragFloat("X Multiplier", ref _mX, 0.01f, 0.01f, 100f);
                    ImGui.DragFloat("Y Multiplier", ref _mY, 0.01f, 0.01f, 100f);
                    ImGui.Checkbox("Scale positions only", ref _positionOnly);
                    ImGui.SameLine();
                    padWidth = ImGui.CalcItemWidth() - ImGui.GetCursorPosX() - 60 * _scale + padX;
                    ImGui.SetCursorPosX(ImGui.GetCursorPosX() + padWidth);
                    if (ImGui.Button("Scale", new Vector2(60 * _scale, 0)))
                    {
                        group.ScaleResolution(new(_mX, _mY), _positionOnly);
                    }
                }


                ImGui.EndChild();
            }
        }
    }
}
