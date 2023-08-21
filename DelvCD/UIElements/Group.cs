using DelvCD.Config;
using DelvCD.Helpers;
using System.Collections.Generic;
using System.Numerics;

namespace DelvCD.UIElements
{
    public class Group : UIElement, IGroup
    {
        public override ElementType Type => ElementType.Group;

        public ElementListConfig ElementList { get; set; }

        public GroupConfig GroupConfig { get; set; }

        public VisibilityConfig VisibilityConfig { get; set; }

        // Constructor for deserialization
        public Group() : this(string.Empty) { }

        public Group(string name) : base(name)
        {
            this.ElementList = new ElementListConfig();
            this.GroupConfig = new GroupConfig();
            this.VisibilityConfig = new VisibilityConfig();
        }

        public override IEnumerable<IConfigPage> GetConfigPages()
        {
            yield return this.ElementList;
            yield return this.GroupConfig;
            yield return this.VisibilityConfig;
        }

        public override void ImportPage(IConfigPage page)
        {
            switch (page)
            {
                case ElementListConfig newPage:
                    this.ElementList = newPage;
                    break;
                case GroupConfig newPage:
                    this.GroupConfig = newPage;
                    break;
                case VisibilityConfig newPage:
                    this.VisibilityConfig = newPage;
                    break;
            }
        }

        public override void StopPreview()
        {
            base.StopPreview();

            foreach (UIElement element in this.ElementList.UIElements)
            {
                element.StopPreview();
            }
        }

        public override void Draw(Vector2 pos, Vector2? parentSize = null, bool parentVisible = true)
        {
            bool visible = this.VisibilityConfig.IsVisible(parentVisible);
            foreach (UIElement element in this.ElementList.UIElements)
            {
                if (!this.Preview && this.LastFrameWasPreview)
                {
                    element.Preview = false;
                }
                else
                {
                    element.Preview |= this.Preview;
                }

                if (visible || Singletons.Get<PluginManager>().IsConfigOpen())
                {
                    element.Draw(pos + this.GroupConfig.Position, null, visible);
                }
            }

            this.LastFrameWasPreview = this.Preview;
        }

        public void ResizeIcons(Vector2 size, bool recurse, bool conditions)
        {
            foreach (UIElement item in this.ElementList.UIElements)
            {
                if (item is Icon icon)
                {
                    icon.Resize(size, conditions);
                }
                else if (recurse && item is Group group)
                {
                    group.ResizeIcons(size, recurse, conditions);
                }
            }
        }

        public void ScaleResolution(Vector2 scaleFactor, bool positionOnly)
        {
            this.GroupConfig.Position *= scaleFactor;
            foreach (UIElement item in this.ElementList.UIElements)
            {
                if (item is Icon icon)
                {
                    icon.ScaleResolution(scaleFactor, positionOnly);
                }
                else if (item is Group group)
                {
                    group.ScaleResolution(scaleFactor, positionOnly);
                }
            }
        }
    }
}