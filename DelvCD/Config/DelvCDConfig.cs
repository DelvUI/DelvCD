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
            this.ElementList = new ElementListConfig();
            this.GroupConfig = new GroupConfig();
            this.VisibilityConfig = new VisibilityConfig();
            this.FontConfig = new FontConfig();
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                ConfigHelpers.SaveConfig(this);
            }
        }

        public override string ToString() => this.Name;

        public IEnumerable<IConfigPage> GetConfigPages()
        {
            yield return this.ElementList;
            yield return this.GroupConfig;
            yield return this.VisibilityConfig;
            yield return this.FontConfig;
            yield return this.AboutPage;
        }

        public void ImportPage(IConfigPage page)
        {
        }
    }
}