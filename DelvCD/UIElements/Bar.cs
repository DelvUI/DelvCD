using Dalamud.Logging;
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
                StyleConditions.UpdateTriggerCount(dataSources.Length);
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
                    newPage.UpdateTriggerCount(0);
                    newPage.UpdateDefaultStyle(BarStyleConfig);
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
            //if (!TriggerConfig.TriggerOptions.Any())
            //{
            //    return;
            //}

            //bool visible = VisibilityConfig.IsVisible(parentVisible);
            //if (!visible && !Preview)
            //{
            //    return;
            //}

            //bool triggered = TriggerConfig.IsTriggered(Preview, out int triggeredIndex);
            //DataSource data = TriggerConfig.TriggerOptions[triggeredIndex].DataSource;
            //DataSource[] datas = TriggerConfig.TriggerOptions.Select(x => x.DataSource).ToArray();
            //BarStyleConfig style = StyleConditions.GetStyle(datas, triggeredIndex) ?? BarStyleConfig;

            //Vector2 localPos = pos + style.Position;
            //Vector2 size = style.Size;

            //if (Singletons.Get<PluginManager>().ShouldClip())
            //{
            //    ClipRect? clipRect = Singletons.Get<ClipRectsHelper>().GetClipRectForArea(localPos, size);
            //    if (clipRect.HasValue)
            //    {
            //        return;
            //    }
            //}

            //if (triggered || Preview)
            //{
            //    UpdateStartData(data);
            //    UpdateDragData(localPos, size);

            //    DrawHelpers.DrawInWindow($"##{ID}", localPos, size, Preview, SetPosition, (drawList) =>
            //    {
            //        if (Preview)
            //        {
            //            data = UpdatePreviewData(data);
            //            if (LastFrameWasDragging)
            //            {
            //                localPos = ImGui.GetWindowPos();
            //                style.Position = localPos - pos;
            //            }
            //        }

            //        if (style.IconOption == 2)
            //        {
            //            return;
            //        }

            //        bool desaturate = style.DesaturateIcon;
            //        float alpha = style.Opacity;

            //        if (style.IconOption == 3)
            //        {
            //            drawList.AddRectFilled(localPos, localPos + size, style.IconColor.Base);
            //        }
            //        else
            //        {
            //            uint icon = style.IconOption switch
            //            {
            //                0 => data.Icon,
            //                1 => style.CustomIcon,
            //                _ => 0
            //            };

            //            if (icon > 0)
            //            {
            //                DrawHelpers.DrawIcon(icon, localPos, size, style.CropIcon, 0, desaturate, alpha, drawList);
            //            }
            //        }

            //        if (style.ShowProgressSwipe && datas.Length > style.ProgressDataSourceIndex)
            //        {
            //            float progressValue = datas[style.ProgressDataSourceIndex].GetProgressValue(style.ProgressDataSourceFieldIndex);
            //            float progressMaxValue = datas[style.ProgressDataSourceIndex].GetMaxValue(style.ProgressDataSourceFieldIndex);

            //            if (style.InvertValues)
            //            {
            //                progressValue = progressMaxValue - progressValue;
            //            }

            //            if (style.GcdSwipe && (progressValue == 0 || progressMaxValue == 0 || style.GcdSwipeOnly))
            //            {
            //                ActionHelpers.GetGCDInfo(out var recastInfo);
            //                DrawProgressSwipe(style, localPos, size, recastInfo.RecastTime - recastInfo.RecastTimeElapsed, recastInfo.RecastTime, alpha, drawList);
            //            }
            //            else
            //            {
            //                DrawProgressSwipe(style, localPos, size, progressValue, progressMaxValue, alpha, drawList);
            //            }
            //        }

            //        if (style.ShowBorder)
            //        {
            //            for (int i = 0; i < style.BorderThickness; i++)
            //            {
            //                Vector2 offset = new Vector2(i, i);
            //                Vector4 color = style.BorderColor.Vector.AddTransparency(alpha);
            //                drawList.AddRect(localPos + offset, localPos + size - offset, ImGui.ColorConvertFloat4ToU32(color));
            //            }
            //        }

            //        if (style.Glow)
            //        {
            //            DrawIconGlow(localPos, size, style.GlowThickness, style.GlowSegments, style.GlowSpeed, style.GlowColor, style.GlowColor2, drawList);
            //        }
            //    });
            //}
            //else
            //{
            //    StartData = null;
            //    StartTime = null;
            //}

            //foreach (Label label in LabelListConfig.Labels)
            //{
            //    if (!Preview && LastFrameWasPreview)
            //    {
            //        label.Preview = false;
            //    }
            //    else
            //    {
            //        label.Preview |= Preview;
            //    }

            //    if (triggered || label.Preview)
            //    {
            //        label.SetData(datas, triggeredIndex);
            //        label.Draw(localPos, size, visible);
            //    }
            //}

            //LastFrameWasPreview = Preview;
        }

        private void DrawIconGlow(Vector2 pos, Vector2 size, int thickness, int segments, float speed, ConfigColor col1, ConfigColor col2, ImDrawListPtr drawList)
        {
            speed = Math.Abs(speed);
            int mod = speed == 0 ? 1 : (int)(250 / speed);
            float prog = (float)(DateTimeOffset.Now.ToUnixTimeMilliseconds() % mod) / mod;

            float offset = thickness / 2 + thickness % 2;
            Vector2 pad = new Vector2(offset);
            Vector2 c1 = new Vector2(pos.X, pos.Y);
            Vector2 c2 = new Vector2(pos.X + size.X, pos.Y);
            Vector2 c3 = new Vector2(pos.X + size.X, pos.Y + size.Y);
            Vector2 c4 = new Vector2(pos.X, pos.Y + size.Y);

            DrawHelpers.DrawSegmentedLineHorizontal(drawList, c1, size.X, thickness, prog, segments, col1, col2);
            DrawHelpers.DrawSegmentedLineVertical(drawList, c2.AddX(-thickness), thickness, size.Y, prog, segments, col1, col2);
            DrawHelpers.DrawSegmentedLineHorizontal(drawList, c3.AddY(-thickness), -size.X, thickness, prog, segments, col1, col2);
            DrawHelpers.DrawSegmentedLineVertical(drawList, c4, thickness, -size.Y, prog, segments, col1, col2);
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
}