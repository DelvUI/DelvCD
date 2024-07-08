using DelvCD.Config;
using DelvCD.Helpers;
using DelvCD.Helpers.DataSources;
using ImGuiNET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text.Json.Serialization;

namespace DelvCD.UIElements
{
    public class Bar : UIElement
    {
        public override ElementType Type => ElementType.Bar;

        public BarStyleConfig BarStyleConfig { get; set; }
        public LabelListConfig LabelListConfig { get; set; }
        public VisibilityConfig VisibilityConfig { get; set; }

        [JsonIgnore] private TriggerDataOp[] _thresholdOperators = new TriggerDataOp[] { TriggerDataOp.LessThan, TriggerDataOp.GreaterThan, TriggerDataOp.LessThanEq, TriggerDataOp.GreaterThanEq };

        [JsonIgnore] private TriggerConfig _triggerConfig = null!;
        public TriggerConfig TriggerConfig
        {
            get => _triggerConfig;
            set
            {
                if (_triggerConfig != null)
                {
                    _triggerConfig.TriggerOptionsUpdateEvent -= OnTriggerOptionsChanged;
                }

                _triggerConfig = value;
                _triggerConfig.TriggerOptionsUpdateEvent += OnTriggerOptionsChanged;

                OnTriggerOptionsChanged(TriggerConfig);
            }
        }


        [JsonIgnore] private StyleConditions<BarStyleConfig> _styleConditions = null!;
        public StyleConditions<BarStyleConfig> StyleConditions
        {
            get => _styleConditions;
            set
            {
                _styleConditions = value;

                if (TriggerConfig != null)
                {
                    OnTriggerOptionsChanged(TriggerConfig);
                }
            }
        }

        // Constructor for deserialization
        public Bar() : this(string.Empty) { }

        public Bar(string name) : base(name)
        {
            BarStyleConfig = new BarStyleConfig();
            LabelListConfig = new LabelListConfig();
            TriggerConfig = new TriggerConfig();
            StyleConditions = new StyleConditions<BarStyleConfig>();
            VisibilityConfig = new VisibilityConfig();
        }

        private void OnTriggerOptionsChanged(TriggerConfig sender)
        {
            DataSource[] dataSources = sender.TriggerOptions.Select(x => x.DataSource).ToArray();

            if (BarStyleConfig != null)
            {
                BarStyleConfig.UpdateDataSources(dataSources);
            }

            if (StyleConditions != null)
            {
                StyleConditions.UpdateDataSources(dataSources);
            }
        }

        public override IEnumerable<IConfigPage> GetConfigPages()
        {
            yield return BarStyleConfig;
            yield return LabelListConfig;
            yield return TriggerConfig;

            // ugly hack
            StyleConditions.UpdateDefaultStyle(BarStyleConfig);

            yield return StyleConditions;
            yield return VisibilityConfig;
        }

        public override void ImportPage(IConfigPage page)
        {
            switch (page)
            {
                case BarStyleConfig newPage:
                    newPage.UpdateDataSources(TriggerConfig.TriggerOptions.Select(x => x.DataSource).ToArray());
                    BarStyleConfig = newPage;
                    break;
                case LabelListConfig newPage:
                    LabelListConfig = newPage;
                    break;
                case TriggerConfig newPage:
                    TriggerConfig = newPage;
                    break;
                case StyleConditions<BarStyleConfig> newPage:
                    newPage.UpdateDefaultStyle(BarStyleConfig);
                    newPage.UpdateDataSources(TriggerConfig.TriggerOptions.Select(x => x.DataSource).ToArray());
                    StyleConditions = newPage;
                    break;
                case VisibilityConfig newPage:
                    VisibilityConfig = newPage;
                    break;
            }
        }

        public override bool Draw(Vector2 pos, Vector2? parentSize = null, bool parentVisible = true, int index = -1, Vector2? offset = null)
        {
            if (!TriggerConfig.TriggerOptions.Any())
            {
                return false;
            }

            bool visible = VisibilityConfig.IsVisible(parentVisible);
            if (!visible && !Preview)
            {
                return false;
            }

            bool triggered = TriggerConfig.IsTriggered(Preview, out int triggeredIndex);
            DataSource data = TriggerConfig.TriggerOptions[triggeredIndex].DataSource;
            DataSource[] datas = TriggerConfig.TriggerOptions.Select(x => x.DataSource).ToArray();
            BarStyleConfig style = StyleConditions.GetStyle(datas) ?? BarStyleConfig;

            Vector2 localPos = pos + style.Position;
            if (index >= 0 && offset != null)
            {
                localPos += index * (Vector2)offset;
            }
            Vector2 size = style.Size;

            if (Singletons.Get<PluginManager>().ShouldClip())
            {
                ClipRect? clipRect = Singletons.Get<ClipRectsHelper>().GetClipRectForArea(localPos, size);
                if (clipRect.HasValue)
                {
                    return false;
                }
            }

            LastFrameWasPreview = Preview;

            if (!triggered && !Preview)
            {
                StartData = null;
                StartTime = null;
                return false;
            }

            UpdateStartData(data);
            UpdateDragData(localPos, size);

            float realValue = datas[style.ProgressDataSourceIndex].GetProgressValue(style.ProgressDataSourceFieldIndex);
            float progressMaxValue = datas[style.ProgressDataSourceIndex].GetMaxValue(style.ProgressDataSourceFieldIndex);

            float progressValue = Math.Min(realValue, progressMaxValue);

            if (style.InvertValues && progressValue != 0)
            {
                progressValue = progressMaxValue - progressValue;
            }

            DrawHelpers.DrawInWindow($"##{ID}", localPos, size, Preview, SetPosition, (drawList) =>
            {
                if (Preview)
                {
                    data = UpdatePreviewData(data);
                    if (LastFrameWasDragging)
                    {
                        localPos = ImGui.GetWindowPos();
                        style.Position = localPos - pos;
                    }
                }

                List<BarData> bars;
                if (!style.Chunked)
                {
                    bars = new List<BarData>() { CalculateBar(size, progressValue, progressMaxValue, style.Direction) };
                }
                else
                {
                    int chunkCount = style.ChunkedStacksFromTrigger ? (int)progressMaxValue : style.ChunkCount;
                    bars = CalculateChunkedBars(size, progressValue, progressMaxValue, chunkCount, style.ChunkPadding, style.Direction, style.IncompleteChunkColor);
                }

                foreach (BarData bar in bars)
                {
                    if (style.ChunkStylesIndex == 0)
                    {
                        // background
                        drawList.AddRectFilled(
                            localPos + bar.BackgroundPosition,
                            localPos + bar.BackgroundPosition + bar.BackgroundSize,
                            ImGui.ColorConvertFloat4ToU32(style.BackgroundColor.Vector)
                        );

                        // fill
                        ConfigColor fillColor = bar.FillColor ?? style.FillColor;
                        drawList.AddRectFilled(
                            localPos + bar.FillPosition,
                            localPos + bar.FillPosition + bar.FillSize,
                            ImGui.ColorConvertFloat4ToU32(fillColor.Vector)
                        );
                        
                        if (style.ShowBorder)
                        {
                            for (int i = 0; i < style.BorderThickness; i++)
                            {
                                Vector2 offset = new Vector2(i, i);
                                drawList.AddRect(
                                    localPos + bar.BackgroundPosition + offset,
                                    localPos + bar.BackgroundPosition + bar.BackgroundSize - offset,
                                    ImGui.ColorConvertFloat4ToU32(style.BorderColor.Vector));
                            }
                        }

                        if (style.Glow)
                        {
                            DrawHelpers.DrawGlow(localPos, size, style.GlowThickness, style.GlowSegments, style.GlowSpeed, style.GlowColor, style.GlowColor2, drawList);
                        }
                    }

                    if (style.ChunkStylesIndex == 1 || style.ChunkStylesIndex == 2) // Circles || Polygons
                    {
                        var ngonsides = style.NgonSides;
                        if (style.ChunkStylesIndex == 1) { ngonsides = 0; } // Circles

                        // background
                        drawList.AddCircleFilled(
                            localPos + bar.BackgroundPosition + new Vector2(style.Radius / 2, style.Radius / 2),
                            style.Radius/2,
                            ImGui.ColorConvertFloat4ToU32(style.BackgroundColor.Vector),
                            ngonsides
                        );

                        // fill
                        ConfigColor fillColor = bar.FillColor ?? style.FillColor;
                        drawList.AddCircleFilled(
                            localPos + bar.BackgroundPosition + new Vector2(style.Radius / 2, style.Radius / 2),
                            style.Radius/2,
                            ImGui.ColorConvertFloat4ToU32(fillColor.Vector),
                            ngonsides
                        );

                        if (style.ShowBorder)
                        {
                            for (int i = 0; i < style.BorderThickness; i++)
                            {
                                float offset = i;
                                drawList.AddCircle(
                                    localPos + bar.BackgroundPosition + new Vector2(style.Radius / 2, style.Radius / 2),
                                    (style.Radius / 2) - offset,
                                    ImGui.ColorConvertFloat4ToU32(style.BorderColor.Vector),
                                    ngonsides
                                    );
                            }
                        }

                        if (style.Glow) // First draft, opted for "flashing" border effect since drawing the glow effect on non-rectangles seems messy
                        {
                            DrawHelpers.DrawGlowNGon(
                                localPos + bar.BackgroundPosition + new Vector2(style.Radius / 2, style.Radius / 2),
                                style.Radius,
                                style.GlowThickness,
                                ngonsides,
                                style.GlowSegments,
                                style.GlowSpeed,
                                style.GlowColor,
                                style.GlowColor2,
                                drawList
                                );
                        }
                    }
                }
            });

            foreach (Label label in LabelListConfig.Labels)
            {
                if (!Preview && LastFrameWasPreview)
                {
                    label.Preview = false;
                }
                else
                {
                    label.Preview |= Preview;
                }

                if (triggered || label.Preview)
                {
                    label.UpdateDataSources(datas);
                    label.Draw(localPos, size, visible);
                }
            }
            return true;
        }

        private BarData CalculateBar(Vector2 size, float progress, float max, BarDirection direction)
        {
            return CalculateBar(Vector2.Zero, size, progress, max, direction);
        }

        private BarData CalculateBar(Vector2 pos, Vector2 size, float progress, float max, BarDirection direction)
        {
            BarData bar = new();
            bar.BackgroundPosition = pos;
            bar.BackgroundSize = size;

            float fillPercent = max == 0 ? 1f : Math.Clamp(progress / max, 0f, 1f);

            bar.FillSize = direction == BarDirection.Left || direction == BarDirection.Right ?
                new(size.X * fillPercent, size.Y) :
                new(size.X, size.Y * fillPercent);

            bar.FillPosition = direction switch
            {
                BarDirection.Right => pos,
                BarDirection.Left => new(pos.X + size.X - bar.FillSize.X, pos.Y),
                BarDirection.Up => new Vector2(pos.X, pos.Y + size.Y - bar.FillSize.Y),
                BarDirection.Down => pos,
                _ => Vector2.Zero,
            };

            return bar;
        }

        private List<BarData> CalculateChunkedBars(Vector2 size, float progress, float max, int count, int padding, BarDirection direction, ConfigColor incompleteColor)
        {
            if (count == 1)
            {
                return new List<BarData>() { CalculateBar(size, progress, 1, direction) };
            }

            bool horizontal = direction == BarDirection.Right || direction == BarDirection.Left;
            bool reversed = direction == BarDirection.Left || direction == BarDirection.Up;

            int paddingCount = count - 1;
            int chunkLength = horizontal ?
                (int)((size.X - (paddingCount * padding)) / count) :
                (int)((size.Y - (paddingCount * padding)) / count);

            Vector2 chunkSize = horizontal ?
                new(chunkLength, size.Y) :
                new(size.X, chunkLength);

            float chunkProgressSize = max / count;

            Vector2 pos = Vector2.Zero;
            List<BarData> bars = new(count);

            for (int i = 0; i < count; i++)
            {
                int index = reversed ? count - i - 1 : i;
                float chunkProgress = Math.Min(progress - (chunkProgressSize * index), chunkProgressSize);
                BarData bar = CalculateBar(pos, chunkSize, chunkProgress, chunkProgressSize, direction);

                pos = horizontal ?
                    new Vector2(pos.X + padding + chunkLength, pos.Y) :
                    new Vector2(pos.X, pos.Y + padding + chunkLength);

                if (chunkProgress < chunkProgressSize)
                {
                    bar.FillColor = incompleteColor;
                }

                bars.Add(bar);
            }

            return bars;
        }

        public void Resize(Vector2 size, bool conditions)
        {
            BarStyleConfig.Size = size;

            if (conditions)
            {
                foreach (var condition in StyleConditions.Conditions)
                {
                    condition.Style.Size = size;
                }
            }
        }

        public void ScaleResolution(Vector2 scaleFactor, bool positionOnly)
        {
            BarStyleConfig.Position *= scaleFactor;

            if (!positionOnly)
            {
                BarStyleConfig.Size *= scaleFactor;
            }

            foreach (var condition in StyleConditions.Conditions)
            {
                condition.Style.Position *= scaleFactor;

                if (!positionOnly)
                {
                    condition.Style.Size *= scaleFactor;
                }
            }
        }

        public static Bar GetDefaultUIElementBar(string name)
        {
            Bar newBar = new Bar(name);
            newBar.ImportPage(newBar.LabelListConfig.GetDefault());
            return newBar;
        }
    }

    internal struct BarData
    {
        public Vector2 BackgroundPosition;
        public Vector2 BackgroundSize;

        public Vector2 FillPosition;
        public Vector2 FillSize;

        public ConfigColor? FillColor;
    }
}
