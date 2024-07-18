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
            ElementList = new ElementListConfig();
            GroupConfig = new GroupConfig();
            VisibilityConfig = new VisibilityConfig();
        }

        public override IEnumerable<IConfigPage> GetConfigPages()
        {
            yield return ElementList;
            yield return GroupConfig;
            yield return VisibilityConfig;
        }

        public override void ImportPage(IConfigPage page)
        {
            switch (page)
            {
                case ElementListConfig newPage:
                    ElementList = newPage;
                    break;
                case GroupConfig newPage:
                    GroupConfig = newPage;
                    break;
                case VisibilityConfig newPage:
                    VisibilityConfig = newPage;
                    break;
            }
        }

        public override void StopPreview()
        {
            base.StopPreview();

            foreach (UIElement element in ElementList.UIElements)
            {
                element.StopPreview();
            }
        }

        public override bool Draw(Vector2 pos, Vector2? parentSize = null, bool parentVisible = true)
        {
            bool visible = VisibilityConfig.IsVisible(parentVisible);
            int dynamicIdx = 0;
            int staticIdx = 0;
            GrowthDirections growthDirections = LayoutHelper.GrowthDirectionsFromIndex(GroupConfig.DynamicGrowthDir);
            foreach (UIElement element in ElementList.UIElements)
            {

                if (!Preview && LastFrameWasPreview)
                {
                    element.Preview = false;
                }
                else
                {
                    element.Preview |= Preview;
                }

                if (visible || Singletons.Get<PluginManager>().IsConfigOpen())
                {
                    Vector2 eleOffset = GroupConfig.IsDynamic ? GroupConfig.DynamicOffset : Vector2.Zero;
                    Vector2 localPos = GroupConfig.Position + (GroupConfig.IsDynamic ? LayoutHelper.CalculateElementPosition(growthDirections, GroupConfig.IsDynamic ? GroupConfig.DynamicMaxPerRow : 1, GroupConfig.RedrawDynamicElements ? dynamicIdx : staticIdx, pos, eleOffset) : pos);
                    if (element.Draw(localPos, null, visible))
                    {
                        dynamicIdx++;
                    }
                    staticIdx++;
                }
            }

            LastFrameWasPreview = Preview;
            return dynamicIdx > 0;
        }

        public void ResizeIcons(Vector2 size, bool recurse, bool conditions)
        {
            foreach (UIElement item in ElementList.UIElements)
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
            GroupConfig.Position *= scaleFactor;
            foreach (UIElement item in ElementList.UIElements)
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