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
    public class Icon : UIElement
    {
        public override ElementType Type => ElementType.Icon;

        public IconStyleConfig IconStyleConfig { get; set; }
        public LabelListConfig LabelListConfig { get; set; }
        public VisibilityConfig VisibilityConfig { get; set; }

        public override bool IsAlwaysHide => VisibilityConfig.AlwaysHide;


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


        [JsonIgnore] private StyleConditions<IconStyleConfig> _styleConditions = null!;
        public StyleConditions<IconStyleConfig> StyleConditions
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
        public Icon() : this(string.Empty) { }

        public Icon(string name) : base(name)
        {
            IconStyleConfig = new IconStyleConfig();
            LabelListConfig = new LabelListConfig();
            TriggerConfig = new TriggerConfig();
            StyleConditions = new StyleConditions<IconStyleConfig>();
            VisibilityConfig = new VisibilityConfig();
        }

        private void OnTriggerOptionsChanged(TriggerConfig sender)
        {
            DataSource[] dataSources = sender.TriggerOptions.Select(x => x.DataSource).ToArray();

            if (IconStyleConfig != null)
            {
                IconStyleConfig.UpdateDataSources(dataSources);
            }

            if (StyleConditions != null)
            {
                StyleConditions.UpdateDataSources(dataSources);
            }
        }

        public override IEnumerable<IConfigPage> GetConfigPages()
        {
            yield return IconStyleConfig;
            yield return LabelListConfig;
            yield return TriggerConfig;

            // ugly hack
            StyleConditions.UpdateDefaultStyle(IconStyleConfig);

            yield return StyleConditions;
            yield return VisibilityConfig;
        }

        public override void ImportPage(IConfigPage page)
        {
            switch (page)
            {
                case IconStyleConfig newPage:
                    newPage.UpdateDataSources(TriggerConfig.TriggerOptions.Select(x => x.DataSource).ToArray());
                    IconStyleConfig = newPage;
                    break;
                case LabelListConfig newPage:
                    LabelListConfig = newPage;
                    break;
                case TriggerConfig newPage:
                    TriggerConfig = newPage;
                    break;
                case StyleConditions<IconStyleConfig> newPage:
                    newPage.UpdateDefaultStyle(IconStyleConfig);
                    newPage.UpdateDataSources(TriggerConfig.TriggerOptions.Select(x => x.DataSource).ToArray());
                    StyleConditions = newPage;
                    break;
                case VisibilityConfig newPage:
                    VisibilityConfig = newPage;
                    break;
            }
        }

        public override bool Draw(Vector2 pos, Vector2? parentSize = null, bool parentVisible = true)
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
            IconStyleConfig style = StyleConditions.GetStyle(datas) ?? IconStyleConfig;

            Vector2 localPos = pos + style.Position;
            Vector2 size = style.Size;

            if (Singletons.Get<PluginManager>().ShouldClip())
            {
                ClipRect? clipRect = Singletons.Get<ClipRectsHelper>().GetClipRectForArea(localPos, size);
                if (clipRect.HasValue)
                {
                    return false;
                }
            }

            if (!triggered && !Preview)
            {
                StartData = null;
                StartTime = null;
                return false;
            }

            UpdateStartData(data);
            UpdateDragData(localPos, size);

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

                if (style.IconOption == 2)
                {
                    return;
                }

                bool desaturate = style.DesaturateIcon;
                float alpha = style.Opacity;

                if (style.IconOption == 3)
                {
                    drawList.AddRectFilled(localPos, localPos + size, style.IconColor.Base);
                }
                else
                {
                    uint icon = style.IconOption switch
                    {
                        0 => data.Icon,
                        1 => style.CustomIcon,
                        _ => 0
                    };

                    if (icon > 0)
                    {
                        DrawHelpers.DrawIcon(icon, localPos, size, style.CropIcon, 0, desaturate, alpha, drawList);
                    }
                }

                if (style.ShowProgressSwipe && datas.Length > style.ProgressDataSourceIndex)
                {
                    float progressValue = datas[style.ProgressDataSourceIndex].GetProgressValue(style.ProgressDataSourceFieldIndex);
                    float progressMaxValue = datas[style.ProgressDataSourceIndex].GetMaxValue(style.ProgressDataSourceFieldIndex);

                    if (style.InvertValues)
                    {
                        progressValue = progressMaxValue - progressValue;
                    }

                    if (style.GcdSwipe && (progressValue == 0 || progressMaxValue == 0 || style.GcdSwipeOnly))
                    {
                        ActionHelpers.GetGCDInfo(out var recastInfo);
                        DrawProgressSwipe(style, localPos, size, recastInfo.RecastTime - recastInfo.RecastTimeElapsed, recastInfo.RecastTime, alpha, drawList);
                    }
                    else
                    {
                        DrawProgressSwipe(style, localPos, size, progressValue, progressMaxValue, alpha, drawList);
                    }
                }

                if (style.ShowBorder)
                {
                    for (int i = 0; i < style.BorderThickness; i++)
                    {
                        Vector2 offset = new Vector2(i, i);
                        Vector4 color = style.BorderColor.Vector.AddTransparency(alpha);
                        drawList.AddRect(localPos + offset, localPos + size - offset, ImGui.ColorConvertFloat4ToU32(color));
                    }
                }

                if (style.Glow)
                {
                    DrawHelpers.DrawGlow(localPos, size, style.GlowThickness, style.GlowSegments, style.GlowSpeed, style.GlowColor, style.GlowColor2, drawList);
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

            LastFrameWasPreview = Preview;
            return true;
        }

        private static void DrawProgressSwipe(
            IconStyleConfig style,
            Vector2 pos,
            Vector2 size,
            float triggeredValue,
            float startValue,
            float alpha,
            ImDrawListPtr drawList)
        {
            if (startValue <= 0 || triggeredValue == 0 || startValue < triggeredValue)
            {
                return;
            }
            
            float completion = triggeredValue / startValue;
            uint progressAlpha = (uint)(style.ProgressSwipeOpacity * 255 * alpha) << 24;
            
            if (completion == 1)
            {
                drawList.AddRectFilled(pos, pos + size, progressAlpha);
                return;
            }
            
            float segInvert = style.InvertSwipe ? 1 - completion : completion;
            int segments = (int)Math.Ceiling(segInvert*4);
            Vector2 center = pos + size / 2;
            uint swipeLineColor = ImGui.ColorConvertFloat4ToU32(style.ProgressLineColor.Vector.AddTransparency(alpha));
            
            // Define vertices for top, left, bottom, and right points relative to the center.
            Vector2[] vertices =
            [
                center with {Y = center.Y - size.Y}, // Top
                center with {X = center.X - size.X}, // Left
                center with {Y = center.Y + size.Y}, // Bottom
                center with {X = center.X + size.X} // Right
            ];

            if (style.InvertSwipe)
            {
                vertices = vertices.Select((v, i) => new { v, i })
                                   .OrderBy(x => new[] { 0, 3, 2, 1 }.ToList().IndexOf(x.i))
                                   .Select(x => x.v)
                                   .ToArray();
            }
            
            ImGui.PushClipRect(pos, pos + size, false);
            for (int i = 0; i < segments; i++)
            {
                Vector2 v2 = vertices[i % 4];
                Vector2 v3 = vertices[(i + 1) % 4];
                
                if (i == segments - 1)
                {   // If drawing the last segment, adjust the second vertex based on the cooldown.
                    float angle = 2 * MathF.PI * (1 - completion);
                    float cos = MathF.Cos(angle);
                    float sin = MathF.Sin(angle);
                    
                    v3 = center + Vector2.Multiply(new Vector2(sin,-cos), size);
                    
                }

                if (style.InvertSwipe) //account for winding visual issues
                    (v2, v3) = (v3, v2);
                drawList.AddTriangleFilled(center, v3, v2, progressAlpha);
                
                if (style.ShowSwipeLines && i == segments - 1)
                {   // Draw swipe lines if enabled & on last segment, must be here due to draw order.
                    var inv = style.InvertSwipe ? v2 : v3;
                    drawList.AddLine(center, inv, swipeLineColor, style.ProgressLineThickness);
                    drawList.AddLine(center, vertices[0], swipeLineColor, style.ProgressLineThickness);
                    drawList.AddCircleFilled(center, style.ProgressLineThickness / 2, swipeLineColor);
                }
            }
            ImGui.PopClipRect();
        }
        
        public void Resize(Vector2 size, bool conditions)
        {
            IconStyleConfig.Size = size;

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
            IconStyleConfig.Position *= scaleFactor;

            if (!positionOnly)
            {
                IconStyleConfig.Size *= scaleFactor;
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

        public static Icon GetDefaultUIElementIcon(string name)
        {
            Icon newIcon = new Icon(name);
            newIcon.ImportPage(newIcon.LabelListConfig.GetDefault());
            return newIcon;
        }
    }
}