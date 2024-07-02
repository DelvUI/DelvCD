using System;
using System.IO;
using System.Reflection;
using Dalamud.Game;
using Dalamud.Game.ClientState.Objects;
using Dalamud.Interface.Textures.TextureWraps;
using Dalamud.Plugin;
using Dalamud.Plugin.Services;
using DelvCD.Config;
using DelvCD.Helpers;

namespace DelvCD
{
    public class Plugin : IDalamudPlugin
    {
        public const string ConfigFileName = "DelvCD.json";

        public static string Version { get; private set; } = "1.0.1.1";

        public static string ConfigFileDir { get; private set; } = "";

        public static string ConfigFilePath { get; private set; } = "";

        public static string AssemblyFileDir { get; private set; } = "";

        public static IDalamudTextureWrap? IconTexture { get; private set; } = null;

        public static string Changelog { get; private set; } = string.Empty;

        public string Name => "DelvCD";

        public Plugin(
            IBuddyList buddyList,
            IClientState clientState,
            ICommandManager commandManager,
            ICondition condition,
            IDalamudPluginInterface pluginInterface,
            IDataManager dataManager,
            IFramework framework,
            IGameGui gameGui,
            IJobGauges jobGauges,
            IObjectTable objectTable,
            IPartyList partyList,
            ISigScanner sigScanner,
            ITargetManager targetManager,
            IPluginLog logger,
            ITextureProvider textureProvider,
            ITextureSubstitutionProvider textureSubstitutionProvider
        )
        {
            Plugin.Version = Assembly.GetExecutingAssembly().GetName().Version?.ToString() ?? Plugin.Version;
            Plugin.ConfigFileDir = pluginInterface.GetPluginConfigDirectory();
            Plugin.ConfigFilePath = Path.Combine(pluginInterface.GetPluginConfigDirectory(), Plugin.ConfigFileName);
            Plugin.AssemblyFileDir = pluginInterface.AssemblyLocation.DirectoryName ?? "";

            ConfigHelpers.CheckVersion();

            // Register Dalamud APIs
            Singletons.Register(buddyList);
            Singletons.Register(clientState);
            Singletons.Register(commandManager);
            Singletons.Register(condition);
            Singletons.Register(pluginInterface);
            Singletons.Register(dataManager);
            Singletons.Register(framework);
            Singletons.Register(gameGui);
            Singletons.Register(jobGauges);
            Singletons.Register(objectTable);
            Singletons.Register(partyList);
            Singletons.Register(sigScanner);
            Singletons.Register(targetManager);
            Singletons.Register(pluginInterface.UiBuilder);
            Singletons.Register(logger);
            Singletons.Register(textureProvider);
            Singletons.Register(textureSubstitutionProvider);
            Singletons.Register(new TexturesCache());
            Singletons.Register(new ActionHelpers());
            Singletons.Register(new StatusHelpers());
            Singletons.Register(new ClipRectsHelper());

            // Load Icon
            Plugin.IconTexture = LoadIconTexture(textureProvider);

            // Load Changelog
            Plugin.Changelog = LoadChangelog();

            // Load config
            DelvCDConfig config = ConfigHelpers.LoadConfig(Plugin.ConfigFilePath);
            Singletons.Register(config);

            // Initialize Fonts
            FontsManager.CopyPluginFontsToUserPath();
            Singletons.Register(new FontsManager(pluginInterface.UiBuilder, config.FontConfig.Fonts.Values));

            // Initialize Text Tags
            TextTagFormatter.InitializeTextTags();

            // Start the plugin
            Singletons.Register(new PluginManager(clientState, commandManager, pluginInterface, config));
        }

        private static IDalamudTextureWrap? LoadIconTexture(ITextureProvider textureProvider)
        {
            if (string.IsNullOrEmpty(AssemblyFileDir))
            {
                return null;
            }

            string iconPath = Path.Combine(AssemblyFileDir, "Media", "Images", "icon_small.png");
            if (!File.Exists(iconPath))
            {
                return null;
            }

            IDalamudTextureWrap? texture = null;
            try
            {
                // texture = uiBuilder.LoadImage(iconPath);
                texture = textureProvider.GetFromFile(iconPath).GetWrapOrDefault();
                
            }
            catch (Exception ex)
            {
                Singletons.Get<IPluginLog>().Warning($"Failed to load LMeter Icon {ex.ToString()}");
            }

            return texture;
        }

        private static string LoadChangelog()
        {
            string? pluginPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

            if (string.IsNullOrEmpty(pluginPath))
            {
                return string.Empty;
            }

            string changelogPath = Path.Combine(pluginPath, "changelog.md");

            if (File.Exists(changelogPath))
            {
                try
                {
                    string changelog = File.ReadAllText(changelogPath);
                    return changelog.Replace("# ", string.Empty);
                }
                catch (Exception ex)
                {
                    Singletons.Get<IPluginLog>().Warning($"Error loading changelog: {ex.ToString()}");
                }
            }

            return string.Empty;
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
                if (Plugin.IconTexture is not null)
                {
                    Plugin.IconTexture.Dispose();
                }

                Singletons.Dispose();
            }
        }
    }
}
