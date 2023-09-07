using DelvCD.Helpers;
using DelvCD.UIElements;
using ImGuiNET;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Runtime.CompilerServices;

namespace DelvCD.Config
{
    public class LabelStyleConfig : IConfigPage
    {
        [JsonIgnore] private string[] _anchorOptions = Enum.GetNames(typeof(DrawAnchor));
        [JsonIgnore] private string[] _roundingOptions = new string[] { "Floor", "Ceiling", "Round" };

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
                    ImGui.SetTooltip(Utils.GetTagsTooltip(TextTagFormatter.TextTagsList()));
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
    }
}