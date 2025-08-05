using Dalamud.Interface;
using Dalamud.Interface.Utility;
using DelvCD.Helpers;
using DelvCD.UIElements;
using Dalamud.Bindings.ImGui;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;

namespace DelvCD.Config
{
    public class LabelStyleConfig : IConfigPage
    {
        [JsonIgnore] private float _scale => ImGuiHelpers.GlobalScale;

        [JsonIgnore] private string[] _anchorOptions = Enum.GetNames(typeof(DrawAnchor));
        [JsonIgnore] private string[] _roundingOptions = new string[] { "Floor", "Ceiling", "Round" };

        [JsonIgnore] private int _selectedTextTagCategory = 0;

        public string Name => "Text";

        public string TextFormat = "";
        public Vector2 Position = new Vector2(0, 0);
        public DrawAnchor ParentAnchor = DrawAnchor.Center;
        public DrawAnchor TextAlign = DrawAnchor.Center;
        public int FontID = 0;
        public string FontKey = FontsManager.DefaultBigFontKey;
        public ConfigColor TextColor = new ConfigColor(1, 1, 1, 1);
        public bool ShowOutline = true;
        public ConfigColor OutlineColor = new ConfigColor(0, 0, 0, 1);
        public int Rounding = 0;

        public LabelStyleConfig() : this("") { }

        public LabelStyleConfig(string textFormat)
        {
            TextFormat = textFormat;
        }

        public IConfigPage GetDefault() => new LabelStyleConfig("[value]");

        public void DrawConfig(IConfigurable parent, Vector2 size, float padX, float padY)
        {
            if (ImGui.BeginChild("##LabelStyleConfig", new Vector2(size.X, size.Y), true))
            {
                ImGui.InputTextWithHint("Text Format", "Hover for Formatting Info", ref TextFormat, 64);
                
                if (ImGui.IsItemHovered())
                {
                    ImGui.SetTooltip(Utils.GetTagsTooltip());
                }

                ImGui.SameLine();
                if (ImGui.Button("Tags"))
                {
                    ImGui.OpenPopup("DelvCD_TextTagsPopup");
                }

                string? selectedTag = DrawTextTagsList();
                if (selectedTag != null)
                {
                    TextFormat += selectedTag;
                }

                ImGui.Combo("Number Format", ref Rounding, _roundingOptions, _roundingOptions.Length);
                ImGui.DragFloat2("Position", ref Position);
                ImGui.Combo("Parent Anchor", ref Unsafe.As<DrawAnchor, int>(ref ParentAnchor), _anchorOptions, _anchorOptions.Length);
                ImGui.Combo("Text Align", ref Unsafe.As<DrawAnchor, int>(ref TextAlign), _anchorOptions, _anchorOptions.Length);

                string[] fontOptions = FontsManager.GetFontList();
                if (!FontsManager.ValidateFont(fontOptions, FontID, FontKey))
                {
                    FontID = 0;
                    for (int i = 0; i < fontOptions.Length; i++)
                    {
                        if (FontKey.Equals(fontOptions[i]))
                        {
                            FontID = i;
                        }
                    }
                }

                ImGui.Combo("Font", ref FontID, fontOptions, fontOptions.Length);
                FontKey = fontOptions[FontID];

                DrawHelpers.DrawSpacing(1);
                Vector4 textColor = TextColor.Vector;
                ImGui.ColorEdit4("Text Color", ref textColor);
                TextColor.Vector = textColor;
                ImGui.Checkbox("Show Outline", ref ShowOutline);
                if (ShowOutline)
                {
                    DrawHelpers.DrawNestIndicator(1);
                    Vector4 outlineColor = OutlineColor.Vector;
                    ImGui.ColorEdit4("Outline Color", ref outlineColor);
                    OutlineColor.Vector = outlineColor;
                }
            }
            ImGui.EndChild();
        }

        private string? DrawTextTagsList()
        {
            string? selectedTag = null;

            ImGui.SetNextWindowSize(new(390 * _scale, 300 * _scale));

            if (ImGui.BeginPopup("DelvCD_TextTagsPopup", ImGuiWindowFlags.NoMove))
            {
                List<string> categories = TextTagFormatter.TextTagsHelpData.Keys.ToList();
                if (ImGui.BeginChild("##DelvCD_TextTags_Categories", new Vector2(180 * _scale, 284 * _scale), true))
                {
                    for (int i = 0; i < categories.Count; i++)
                    {
                        if (ImGui.Selectable(categories[i], i == _selectedTextTagCategory))
                        {
                            _selectedTextTagCategory = i;
                        }
                    }

                    ImGui.EndChild();
                }

                ImGui.SetCursorPos(new Vector2(200 * _scale, 8 * _scale));
                if (ImGui.BeginChild("##DelvCD_TextTags_List", new Vector2(180 * _scale, 284 * _scale), true))
                {
                    List<string> tags = TextTagFormatter.TextTagsHelpData[categories[_selectedTextTagCategory]];
                    foreach (string tag in tags)
                    {
                        if (ImGui.Selectable(tag))
                        {
                            selectedTag = tag;
                        }
                    }

                    ImGui.EndChild();
                }

                ImGui.EndPopup();
            }

            return selectedTag;
        }
    }
}