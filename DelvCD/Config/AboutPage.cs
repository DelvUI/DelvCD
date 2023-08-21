using DelvCD.Helpers;
using ImGuiNET;
using System.Numerics;

namespace DelvCD.Config
{
    public class AboutPage : IConfigPage
    {
        public string Name => "Changelog";

        public IConfigPage GetDefault() => new AboutPage();

        public void DrawConfig(IConfigurable parent, Vector2 size, float padX, float padY)
        {
            if (ImGui.BeginChild("##AboutPage", new Vector2(size.X, size.Y), true))
            {
                Vector2 headerSize = Vector2.Zero;
                if (Plugin.IconTexture is not null)
                {
                    Vector2 iconSize = new Vector2(Plugin.IconTexture.Width, Plugin.IconTexture.Height);
                    string versionText = $"DelvCD v{Plugin.Version}";
                    Vector2 textSize = ImGui.CalcTextSize(versionText);
                    headerSize = new Vector2(size.X, iconSize.Y + textSize.Y);

                    if (ImGui.BeginChild("##Icon", headerSize, false))
                    {
                        ImDrawListPtr drawList = ImGui.GetWindowDrawList();
                        Vector2 pos = ImGui.GetWindowPos().AddX(size.X / 2 - iconSize.X / 2);
                        drawList.AddImage(Plugin.IconTexture.ImGuiHandle, pos, pos + iconSize);
                        Vector2 textPos = ImGui.GetWindowPos().AddX(size.X / 2 - textSize.X / 2).AddY(iconSize.Y);
                        drawList.AddText(textPos, 0xFFFFFFFF, versionText);
                        ImGui.End();
                    }
                }

                ImGui.SetCursorPosY(ImGui.GetCursorPosY() + headerSize.Y);
                DrawHelpers.DrawSpacing(1);
                ImGui.Text("Changelog");
                Vector2 changeLogSize = new Vector2(size.X - padX * 2, size.Y - ImGui.GetCursorPosY() - padY - 30);

                if (ImGui.BeginChild("##Changelog", changeLogSize, true))
                {
                    ImGui.Text(Plugin.Changelog);
                    ImGui.EndChild();
                }
            }

            ImGui.EndChild();
        }
    }
}
