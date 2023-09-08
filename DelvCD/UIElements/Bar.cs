using DelvCD.Config;
using DelvCD.Helpers;
using System;
using System.Collections.Generic;
using System.Numerics;

namespace DelvCD.UIElements
{
    public class Bar : UIElement
    {
        public override ElementType Type => ElementType.Bar;

        public BarStyleConfig BarStyleConfig { get; set; }
        public TriggerConfig TriggerConfig { get; set; }
        public StyleConditions<BarStyleConfig> StyleConditions { get; set; }
        public VisibilityConfig VisibilityConfig { get; set; }

        // Constuctor for deserialization
        public Bar() : this(string.Empty) { }

        public Bar(string name) : base(name)
        {
            Name = name;
            BarStyleConfig = new BarStyleConfig();
            TriggerConfig = new TriggerConfig();
            StyleConditions = new StyleConditions<BarStyleConfig>();
            VisibilityConfig = new VisibilityConfig();
        }

        public override void ImportPage(IConfigPage page)
        {
            switch (page)
            {
                case BarStyleConfig newPage:
                    BarStyleConfig = newPage;
                    break;
                case TriggerConfig newPage:
                    TriggerConfig = newPage;
                    break;
                case StyleConditions<BarStyleConfig> newPage:
                    StyleConditions = newPage;
                    break;
                case VisibilityConfig newPage:
                    VisibilityConfig = newPage;
                    break;
            }
        }

        public override IEnumerable<IConfigPage> GetConfigPages()
        {
            yield return BarStyleConfig;
            // yield return this.TriggerConfig;
            // yield return this.VisibilityConfig;
        }

        public override void Draw(Vector2 pos, Vector2? parentSize = null, bool parentVisible = true)
        {

        }
    }
}