using DelvCD.Config;
using DelvCD.Helpers;
using DelvCD.Helpers.DataSources;
using ImGuiNET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text.Json.Serialization;
using System.Threading;
using Dalamud.Logging;
using Dalamud.Plugin.Services;

namespace DelvCD.UIElements
{
    public class Circle : UIElement
    {
        public override ElementType Type => ElementType.Circle;

        public CircleStyleConfig CircleStyleConfig { get; set; }
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


        [JsonIgnore] private StyleConditions<CircleStyleConfig> _styleConditions = null!;
        public StyleConditions<CircleStyleConfig> StyleConditions
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
        public Circle() : this(string.Empty) { }

        public Circle(string name) : base(name)
        {
            CircleStyleConfig = new CircleStyleConfig();
            LabelListConfig = new LabelListConfig();
            TriggerConfig = new TriggerConfig();
            StyleConditions = new StyleConditions<CircleStyleConfig>();
            VisibilityConfig = new VisibilityConfig();
        }

        private void OnTriggerOptionsChanged(TriggerConfig sender)
        {
            DataSource[] dataSources = sender.TriggerOptions.Select(x => x.DataSource).ToArray();

            if (CircleStyleConfig != null)
            {
                CircleStyleConfig.UpdateDataSources(dataSources);
            }

            if (StyleConditions != null)
            {
                StyleConditions.UpdateDataSources(dataSources);
            }
        }

        public override IEnumerable<IConfigPage> GetConfigPages()
        {
            yield return CircleStyleConfig;
            yield return LabelListConfig;
            yield return TriggerConfig;

            // ugly hack
            StyleConditions.UpdateDefaultStyle(CircleStyleConfig);

            yield return StyleConditions;
            yield return VisibilityConfig;
        }

        public override void ImportPage(IConfigPage page)
        {
            switch (page)
            {
                case CircleStyleConfig newPage:
                    newPage.UpdateDataSources(TriggerConfig.TriggerOptions.Select(x => x.DataSource).ToArray());
                    CircleStyleConfig = newPage;
                    break;
                case LabelListConfig newPage:
                    LabelListConfig = newPage;
                    break;
                case TriggerConfig newPage:
                    TriggerConfig = newPage;
                    break;
                case StyleConditions<CircleStyleConfig> newPage:
                    newPage.UpdateDefaultStyle(CircleStyleConfig);
                    newPage.UpdateDataSources(TriggerConfig.TriggerOptions.Select(x => x.DataSource).ToArray());
                    StyleConditions = newPage;
                    break;
                case VisibilityConfig newPage:
                    VisibilityConfig = newPage;
                    break;
            }
        }

        public override void Draw(Vector2 pos, Vector2? parentSize = null, bool parentVisible = true)
        {
            if (!TriggerConfig.TriggerOptions.Any())
            {
                return;
            }

            bool visible = VisibilityConfig.IsVisible(parentVisible);
            if (!visible && !Preview)
            {
                return;
            }

            bool triggered = TriggerConfig.IsTriggered(Preview, out int triggeredIndex);
            DataSource data = TriggerConfig.TriggerOptions[triggeredIndex].DataSource;
            DataSource[] datas = TriggerConfig.TriggerOptions.Select(x => x.DataSource).ToArray();
            CircleStyleConfig style = StyleConditions.GetStyle(datas) ?? CircleStyleConfig;

            Vector2 localPos = pos + style.Position;
            Vector2 size = new Vector2(style.Radius * 2);

            if (Singletons.Get<PluginManager>().ShouldClip())
            {
                ClipRect? clipRect = Singletons.Get<ClipRectsHelper>().GetClipRectForArea(localPos, size);
                if (clipRect.HasValue)
                {
                    return;
                }
            }

            LastFrameWasPreview = Preview;

            if (!triggered && !Preview)
            {
                StartData = null;
                StartTime = null;
                return;
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
                
                // controls how smooth the arc looks
                const int segments = 100;
                float startAngle = (float)(-Math.PI / 2f + style.StartAngle * (Math.PI / 180f));
                float endAngle = (float)(-Math.PI / 2f + style.EndAngle * (Math.PI / 180f));

                List<CircleData> circles;
                circles = new List<CircleData>() { CalculateCircle(startAngle, endAngle, progressValue, progressMaxValue, style.Direction) };

                foreach (CircleData circle in circles)
                {
                    // fill
                    ConfigColor fillColor = circle.FillColor ?? style.FillColor;

                    // Draw background arc first
                    if (circle.EndAngle != startAngle)
                    {
                        drawList.PathArcTo(localPos, style.Radius, circle.StartAngle, endAngle, segments);
                        drawList.PathStroke(ImGui.ColorConvertFloat4ToU32(style.BackgroundColor.Vector), ImDrawFlags.None, style.Thickness);
                    }

                    // Draw fill arc on top
                    drawList.PathArcTo(localPos, style.Radius, circle.StartAngle, circle.EndAngle, segments);
                    drawList.PathStroke(ImGui.ColorConvertFloat4ToU32(fillColor.Vector), ImDrawFlags.None, style.Thickness);
                    
                    
                    if (style.Glow)
                    {
                        DrawHelpers.DrawGlowCircle(localPos, style.Radius, style.Thickness, style.GlowPadding, style.GlowSegments, style.GlowSpeed, style.GlowColor, style.GlowColor2, drawList);
                    }
                    
                }
                if (style.ShowBorder)
                {
                    drawList.PathArcTo(localPos, style.Radius - style.Thickness / 2f, startAngle, endAngle, segments);
                    drawList.PathStroke(ImGui.ColorConvertFloat4ToU32(style.BorderColor.Vector), ImDrawFlags.None, style.BorderThickness);

                    drawList.PathArcTo(localPos, style.Radius + style.Thickness / 2f, startAngle, endAngle, segments);
                    drawList.PathStroke(ImGui.ColorConvertFloat4ToU32(style.BorderColor.Vector), ImDrawFlags.None, style.BorderThickness);
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
        }
        
        private CircleData CalculateCircle(float startAngle, float endAngle, float progress, float max, CircleDirection direction)
        {
            CircleData circle = new();

            float fillPercent = max == 0 ? 1f : Math.Clamp(progress / max, 0f, 1f);
            float angleRange = endAngle - startAngle;

            if (direction == CircleDirection.AntiClockwise)
            {
                // If anticlockwise, the angle range needs to be reversed
                angleRange = startAngle - endAngle;
            }

            float fillAngle = angleRange * fillPercent;

            // Adjusting for direction
            float relativeAngle;
            if (direction == CircleDirection.Clockwise)
            {
                relativeAngle = startAngle + fillAngle;
            }
            else // Anticlockwise
            {
                relativeAngle = startAngle - fillAngle;
            }

            circle.StartAngle = startAngle;
            circle.EndAngle = relativeAngle;

            return circle;
        }

        public void Resize(Vector2 size, bool conditions)
        {
           // CircleStyleConfig.Size = size;

            if (conditions)
            {
                foreach (var condition in StyleConditions.Conditions)
                {
                    //condition.Style.Size = size;
                }
            }
        }

        public void ScaleResolution(Vector2 scaleFactor, bool positionOnly)
        {
            CircleStyleConfig.Position *= scaleFactor;

            if (!positionOnly)
            {
                //CircleStyleConfig.Size *= scaleFactor;
            }

            foreach (var condition in StyleConditions.Conditions)
            {
                condition.Style.Position *= scaleFactor;

                if (!positionOnly)
                {
                    //condition.Style.Size *= scaleFactor;
                }
            }
        }

        public static Circle GetDefaultUIElementCircle(string name)
        {
            Circle newCircle = new Circle(name);
            newCircle.ImportPage(newCircle.LabelListConfig.GetDefault());
            return newCircle;
        }
    }

    internal struct CircleData
    {
        public float StartAngle;
        public float EndAngle;
        public Vector2 FillSize;

        public ConfigColor? FillColor;
    }
}