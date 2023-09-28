using Dalamud.Game.Command;
using Dalamud.Interface.Utility;
using Dalamud.Interface.Windowing;
using Dalamud.Plugin;
using Dalamud.Plugin.Services;
using DelvCD.Config;
using DelvCD.Helpers;
using DelvCD.UIElements;
using DelvCD.Windows;
using ImGuiNET;
using System;
using System.Numerics;

namespace DelvCD
{
    public class PluginManager : IPluginDisposable
    {
        private IClientState ClientState { get; init; }

        private DalamudPluginInterface PluginInterface { get; init; }

        private ICommandManager CommandManager { get; init; }

        private WindowSystem WindowSystem { get; init; }

        private ConfigWindow ConfigRoot { get; init; }

        private DelvCDConfig Config { get; init; }

        private readonly Vector2 _configSize = new Vector2(600, 650);

        private readonly ImGuiWindowFlags _mainWindowFlags =
            ImGuiWindowFlags.NoTitleBar |
            ImGuiWindowFlags.NoScrollbar |
            ImGuiWindowFlags.AlwaysAutoResize |
            ImGuiWindowFlags.NoBackground |
            ImGuiWindowFlags.NoInputs |
            ImGuiWindowFlags.NoBringToFrontOnFocus;

        public PluginManager(
            IClientState clientState,
            ICommandManager commandManager,
            DalamudPluginInterface pluginInterface,
            DelvCDConfig config)
        {
            ClientState = clientState;
            CommandManager = commandManager;
            PluginInterface = pluginInterface;
            Config = config;

            ConfigRoot = new ConfigWindow("ConfigRoot", ImGui.GetMainViewport().Size / 2, _configSize);
            WindowSystem = new WindowSystem("DelvCD");
            WindowSystem.AddWindow(ConfigRoot);

            CommandManager.AddHandler(
                "/delvcd",
                new CommandInfo(PluginCommand)
                {
                    HelpMessage = "Opens the DelvCD configuration window.",
                    ShowInHelp = true
                }
            );

            CommandManager.AddHandler(
                "/dcd",
                new CommandInfo(PluginCommand)
                {
                    HelpMessage = "Opens the DelvCD configuration window.",
                    ShowInHelp = true
                }
            );

            ClientState.Logout += OnLogout;
            PluginInterface.UiBuilder.OpenConfigUi += OpenConfigUi;
            PluginInterface.UiBuilder.Draw += Draw;
        }

        private void Draw()
        {
            if (!CharacterState.ShouldBeVisible())
            {
                return;
            }

            WindowSystem.Draw();

            Vector2 viewPortSize = ImGui.GetMainViewport().Size;
            ImGuiHelpers.ForceNextWindowMainViewport();
            ImGui.SetNextWindowPos(Vector2.Zero);
            ImGui.SetNextWindowSize(viewPortSize);
            ImGui.PushStyleVar(ImGuiStyleVar.WindowRounding, 0);
            ImGui.PushStyleVar(ImGuiStyleVar.WindowPadding, new Vector2(0, 0));
            ImGui.PushStyleVar(ImGuiStyleVar.WindowBorderSize, 0);

            try
            {
                if (ImGui.Begin("DelvCD_Root", _mainWindowFlags))
                {
                    if (Config.VisibilityConfig.IsVisible(true))
                    {
                        Singletons.Get<StatusHelpers>().GenerateStatusMap();
                        Singletons.Get<ClipRectsHelper>().Update();
                        foreach (UIElement element in Config.ElementList.UIElements)
                        {
                            element.Draw((viewPortSize / 2) + Config.GroupConfig.Position);
                        }
                    }
                }
            }
            finally
            {
                ImGui.End();
                ImGui.PopStyleVar(3);
            }
        }

        public void Edit(IConfigurable config)
        {
            ConfigRoot.PushConfig(config);
        }

        public bool IsConfigOpen()
        {
            return ConfigRoot.IsOpen;
        }

        public bool IsConfigurableOpen(IConfigurable configurable)
        {
            return ConfigRoot.IsConfigurableOpen(configurable);
        }

        public bool ShouldClip()
        {
            return Config.VisibilityConfig.Clip;
        }

        private void OpenConfigUi()
        {
            if (!ConfigRoot.IsOpen)
            {
                ConfigRoot.PushConfig(Config);
            }
        }

        private void OnLogout()
        {
            ConfigHelpers.SaveConfig();
        }

        private void PluginCommand(string command, string arguments)
        {
            if (ConfigRoot.IsOpen)
            {
                ConfigRoot.IsOpen = false;
            }
            else
            {
                ConfigRoot.PushConfig(Config);
            }
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
                // Don't modify order
                PluginInterface.UiBuilder.Draw -= Draw;
                PluginInterface.UiBuilder.OpenConfigUi -= OpenConfigUi;
                ClientState.Logout -= OnLogout;
                CommandManager.RemoveHandler("/delvcd");
                CommandManager.RemoveHandler("/dcd");
                WindowSystem.RemoveAllWindows();
            }
        }
    }
}
