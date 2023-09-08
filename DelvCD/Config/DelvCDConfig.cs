using DelvCD.Helpers;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace DelvCD.Config
{
    [JsonObject]
    public class DelvCDConfig : IGroup, IConfigurable, IPluginDisposable
    {
        public string Name
        {
            get => "DelvCD";
            set { }
        }

        public string Version => Plugin.Version;

        public ElementListConfig ElementList { get; set; }

        public GroupConfig GroupConfig { get; set; }

        public VisibilityConfig VisibilityConfig { get; set; }

        public FontConfig FontConfig { get; set; }

        [JsonIgnore]
        private AboutPage AboutPage { get; } = new AboutPage();

        public DelvCDConfig()
        {
            ElementList = new ElementListConfig();
            GroupConfig = new GroupConfig();
            VisibilityConfig = new VisibilityConfig();
            FontConfig = new FontConfig();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                ConfigHelpers.SaveConfig(this);
            }
        }

        public override string ToString() => Name;

        public IEnumerable<IConfigPage> GetConfigPages()
        {
            yield return ElementList;
            yield return GroupConfig;
            yield return VisibilityConfig;
            yield return FontConfig;
            yield return AboutPage;
        }

        public void ImportPage(IConfigPage page)
        {
        }
    }
}