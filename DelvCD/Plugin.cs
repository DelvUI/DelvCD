using Dalamud.Data;
using Dalamud.Game;
using Dalamud.Game.ClientState;
using Dalamud.Game.ClientState.Buddy;
using Dalamud.Game.ClientState.Conditions;
using Dalamud.Game.ClientState.JobGauge;
using Dalamud.Game.ClientState.Objects;
using Dalamud.Game.ClientState.Party;
using Dalamud.Game.Command;
using Dalamud.Game.Gui;
using Dalamud.Interface;
using Dalamud.Interface.Internal;
using Dalamud.Logging;
using Dalamud.Plugin;
using Dalamud.Plugin.Services;
using DelvCD.Config;
using DelvCD.Helpers;
using ImGuiScene;
using System;
using System.IO;
using System.Reflection;
using SigScanner = Dalamud.Game.SigScanner;

namespace DelvCD
{
    public class Plugin : IDalamudPlugin
    {
        public const string ConfigFileName = "DelvCD.json";

        public static string Version { get; private set; } = "0.5.1.1";

        public static string ConfigFileDir { get; private set; } = "";

        public static string ConfigFilePath { get; private set; } = "";

        public static IDalamudTextureWrap? IconTexture { get; private set; } = null;

        public static string Changelog { get; private set; } = string.Empty;

        public string Name => "DelvCD";

        public Plugin(
            IBuddyList buddyList,
            IClientState clientState,
            ICommandManager commandManager,
            ICondition condition,
            DalamudPluginInterface pluginInterface,
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
            Singletons.Register(new ActionHelpers(sigScanner));
            Singletons.Register(new StatusHelpers());
            Singletons.Register(new ClipRectsHelper());

            // Load Icon
            Plugin.IconTexture = LoadIconTexture(pluginInterface.UiBuilder);

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

        private static IDalamudTextureWrap? LoadIconTexture(UiBuilder uiBuilder)
        {
            string? pluginPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            if (string.IsNullOrEmpty(pluginPath))
            {
                return null;
            }

            string iconPath = Path.Combine(pluginPath, "Media", "Images", "icon.png");
            if (!File.Exists(iconPath))
            {
                return null;
            }

            IDalamudTextureWrap? texture = null;
            try
            {
                texture = uiBuilder.LoadImage(iconPath);
            }
            catch (Exception ex)
            {
                Singletons.Get<IPluginLog>().Warning($"Failed to load DelvCD Icon {ex.ToString()}");
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
