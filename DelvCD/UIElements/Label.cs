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
        [JsonIgnore] private DataSource[]? _data;
        [JsonIgnore] private int _dataIndex;

        public override ElementType Type => ElementType.Label;

        [JsonConverter(typeof(LabelConverter))]
        public LabelStyleConfig LabelStyleConfig { get; set; }
        public StyleConditions<LabelStyleConfig> StyleConditions { get; set; }
        public VisibilityConfig VisibilityConfig { get; set; }

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

        public override void Draw(Vector2 pos, Vector2? parentSize = null, bool parentVisible = true)
        {
            if (!VisibilityConfig.IsVisible(parentVisible) && !Preview)
            {
                return;
            }

            Vector2 size = parentSize.HasValue ? parentSize.Value : ImGui.GetMainViewport().Size;
            pos = parentSize.HasValue ? pos : Vector2.Zero;

            LabelStyleConfig style = StyleConditions.GetStyle(_data) ?? LabelStyleConfig;

            string text = _data is not null && _dataIndex < _data.Length && _data[_dataIndex] is not null
                ? _data[_dataIndex].GetFormattedString(style.TextFormat, "N", style.Rounding)
                : style.TextFormat;

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
        }

        public void SetData(DataSource[] data, int index)
        {
            _data = data;
            _dataIndex = index;

            StyleConditions.UpdateDataSources(data);
        }
    }
}