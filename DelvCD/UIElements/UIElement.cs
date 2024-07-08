using DelvCD.Config;
using DelvCD.Helpers;
using DelvCD.Helpers.DataSources;
using ImGuiNET;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Numerics;

namespace DelvCD.UIElements
{
    public abstract class UIElement : IConfigurable
    {
        [JsonIgnore] public bool Preview = false;
        [JsonIgnore] public bool Hovered = false;
        [JsonIgnore] public bool Dragging = false;
        [JsonIgnore] public bool SetPosition = false;

        [JsonIgnore] protected bool LastFrameWasPreview = false;
        [JsonIgnore] protected bool LastFrameWasDragging = false;
        [JsonIgnore] protected DataSource? StartData = null;
        [JsonIgnore] protected DateTime? StartTime = null;
        [JsonIgnore] protected DataSource? OldStartData = null;
        [JsonIgnore] protected DateTime? OldStartTime = null;

        [JsonIgnore] public string ID { get; }

        public string Name { get; set; }
        public string Version => Plugin.Version;

        public UIElement(string name)
        {
            Name = name;
            ID = $"DelvCD_{GetType().Name}_{Guid.NewGuid()}";
        }

        public abstract ElementType Type { get; }

        public abstract bool Draw(Vector2 pos, Vector2? parentSize = null, bool parentVisible = true, int index = -1, Vector2? offset = null);

        public abstract IEnumerable<IConfigPage> GetConfigPages();

        public abstract void ImportPage(IConfigPage page);

        public override string? ToString() => $"{Type} [{Name}]";

        public virtual void StopPreview()
        {
            Preview = false;
        }

        protected DataSource UpdatePreviewData(DataSource data)
        {
            if (StartTime.HasValue && StartData is not null)
            {
                float secondSinceStart = (float)(DateTime.UtcNow - StartTime.Value).TotalSeconds;
                float resetValue = StartData.PreviewMaxValue;
                float newValue = resetValue - secondSinceStart;

                if (newValue < 0)
                {
                    StartTime = DateTime.UtcNow;
                    newValue = resetValue;
                }

                data.PreviewValue = newValue;
                return data;
            }

            return data;
        }

        // Dont ask
        protected void UpdateDragData(Vector2 pos, Vector2 size)
        {
            Hovered = ImGui.IsMouseHoveringRect(pos, pos + size);
            Dragging = LastFrameWasDragging && ImGui.IsMouseDown(ImGuiMouseButton.Left);
            SetPosition = (Preview && !LastFrameWasPreview || !Hovered) && !Dragging;
            LastFrameWasDragging = Hovered || Dragging;
        }

        protected void UpdateStartData(DataSource data)
        {
            if (LastFrameWasPreview && !Preview)
            {
                StartData = OldStartData;
                StartTime = OldStartTime;
            }

            if (!LastFrameWasPreview && Preview)
            {
                OldStartData = StartData;
                OldStartTime = StartTime;
                StartData = null;
                StartTime = null;
            }

            if (StartData is not null &&
                data.PreviewValue > StartData.PreviewValue)
            {
                StartData = data;
                StartTime = DateTime.UtcNow;
            }

            if (StartData is null ||
                !StartTime.HasValue ||
                StartData.Id != data.Id)
            {
                StartData = data;
                StartTime = DateTime.UtcNow;
            }
        }
    }
}