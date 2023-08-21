﻿using ImGuiNET;
using System.Numerics;

namespace DelvCD.Config
{
    public class BarStyleConfig : IConfigPage
    {
        public string Name => "Bar";

        public IConfigPage GetDefault() => new BarStyleConfig();

        public void DrawConfig(IConfigurable parent, Vector2 size, float padX, float padY)
        {
            if (ImGui.BeginChild("##BarStyleConfig", new Vector2(size.X, size.Y), true))
            {
                ImGui.Text("Coming Soon");

                ImGui.EndChild();
            }
        }
    }
}