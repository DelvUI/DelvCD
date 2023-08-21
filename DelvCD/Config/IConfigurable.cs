using System.Collections.Generic;

namespace DelvCD.Config
{
    public interface IConfigurable
    {
        string Name { get; set; }

        IEnumerable<IConfigPage> GetConfigPages();
        void ImportPage(IConfigPage page);
    }
}
