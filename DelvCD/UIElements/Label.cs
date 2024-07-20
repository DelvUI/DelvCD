using DelvCD.Config;
using DelvCD.Helpers;
using DelvCD.Helpers.DataSources;
using ImGuiNET;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace DelvCD.UIElements
{
    public class Label : UIElement
    {
        [JsonIgnore] private DataSource[]? _dataSources;

        public override ElementType Type => ElementType.Label;

        [JsonConverter(typeof(LabelConverter))]
        public LabelStyleConfig LabelStyleConfig { get; set; }
        public StyleConditions<LabelStyleConfig> StyleConditions { get; set; }
        public VisibilityConfig VisibilityConfig { get; set; }

        public override bool IsAlwaysHide => VisibilityConfig.AlwaysHide;

        // Constuctor for deserialization
        public Label() : this(string.Empty) { }

        public Label(string name, string textFormat = "") : base(name)
        {
            Name = name;
            LabelStyleConfig = new LabelStyleConfig(textFormat);
            StyleConditions = new StyleConditions<LabelStyleConfig>();
            VisibilityConfig = new VisibilityConfig();
        }

        public override IEnumerable<IConfigPage> GetConfigPages()
        {
            yield return LabelStyleConfig;
            yield return StyleConditions;
            yield return VisibilityConfig;
        }

        public override void ImportPage(IConfigPage page)
        {
            switch (page)
            {
                case LabelStyleConfig newPage:
                    LabelStyleConfig = newPage;
                    break;
                case StyleConditions<LabelStyleConfig> newPage:
                    StyleConditions = newPage;
                    break;
                case VisibilityConfig newPage:
                    VisibilityConfig = newPage;
                    break;
            }
        }

        public override bool Draw(Vector2 pos, Vector2? parentSize = null, bool parentVisible = true)
        {
            if (!VisibilityConfig.IsVisible(parentVisible) && !Preview)
            {
                return false;
            }

            Vector2 size = parentSize.HasValue ? parentSize.Value : ImGui.GetMainViewport().Size;
            pos = parentSize.HasValue ? pos : Vector2.Zero;

            LabelStyleConfig style = StyleConditions.GetStyle(_dataSources) ?? LabelStyleConfig;

            string text = _dataSources == null ? 
                style.TextFormat : 
                TextTagFormatter.GetFormattedString(_dataSources, style.TextFormat, "N", style.Rounding);

            using (FontsManager.PushFont(style.FontKey))
            {
                Vector2 textSize = ImGui.CalcTextSize(text);
                Vector2 textPos = Utils.GetAnchoredPosition(pos + style.Position, -size, style.ParentAnchor);
                textPos = Utils.GetAnchoredPosition(textPos, textSize, style.TextAlign);
                DrawHelpers.DrawText(
                    ImGui.GetWindowDrawList(),
                    text,
                    textPos,
                    style.TextColor.Base,
                    style.ShowOutline,
                    style.OutlineColor.Base);
            }
            return true;
        }

        public void UpdateDataSources(DataSource[] data)
        {
            _dataSources = data;

            StyleConditions.UpdateDataSources(data);
        }
    }
}