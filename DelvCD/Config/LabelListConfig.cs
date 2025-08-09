using System.Collections.Generic;
using System.Numerics;
using Dalamud.Interface;
using Dalamud.Interface.ImGuiNotification;
using Dalamud.Interface.Utility;
using DelvCD.Helpers;
using DelvCD.UIElements;
using Dalamud.Bindings.ImGui;
using Newtonsoft.Json;

namespace DelvCD.Config
{
    public class LabelListConfig : IConfigPage
    {
        [JsonIgnore] private float _scale => ImGuiHelpers.GlobalScale;

        [JsonIgnore] public string Name => "Labels";
        [JsonIgnore] private string _labelInput = string.Empty;

        public List<Label> Labels { get; init; }

        public LabelListConfig()
        {
            Labels = new List<Label>();
        }

        public LabelListConfig(params Label[] labels)
        {
            Labels = new List<Label>(labels);
        }

        public IConfigPage GetDefault()
        {
            Label valueLabel = new Label("Value", "[value:t]");
            valueLabel.LabelStyleConfig.FontKey = FontsManager.DefaultBigFontKey;
            valueLabel.LabelStyleConfig.FontID = FontsManager.GetFontIndex(FontsManager.DefaultBigFontKey);
            valueLabel.StyleConditions.Conditions.Add(new StyleCondition<LabelStyleConfig>()
            {
                Source = 0,
                Op = TriggerDataOp.Equals,
                Value = 0
            });

            Label stacksLabel = new Label("Stacks", "[stacks]");
            stacksLabel.LabelStyleConfig.FontKey = FontsManager.DefaultMediumFontKey;
            stacksLabel.LabelStyleConfig.FontID = FontsManager.GetFontIndex(FontsManager.DefaultMediumFontKey);
            stacksLabel.LabelStyleConfig.Position = new Vector2(-1, 0);
            stacksLabel.LabelStyleConfig.ParentAnchor = DrawAnchor.BottomRight;
            stacksLabel.LabelStyleConfig.TextAlign = DrawAnchor.BottomRight;
            stacksLabel.LabelStyleConfig.TextColor = new ConfigColor(0, 0, 0, 1);
            stacksLabel.LabelStyleConfig.OutlineColor = new ConfigColor(1, 1, 1, 1);
            stacksLabel.StyleConditions.Conditions.Add(new StyleCondition<LabelStyleConfig>()
            {
                Source = 2,
                Op = TriggerDataOp.LessThanEq,
                Value = 1
            });

            return new LabelListConfig(valueLabel, stacksLabel);
        }

        public void DrawConfig(IConfigurable parent, Vector2 size, float padX, float padY)
        {
            DrawLabelTable(size, padX);
        }

        private void DrawLabelTable(Vector2 size, float padX)
        {
            ImGuiTableFlags tableFlags =
                ImGuiTableFlags.RowBg |
                ImGuiTableFlags.Borders |
                ImGuiTableFlags.BordersOuter |
                ImGuiTableFlags.BordersInner |
                ImGuiTableFlags.ScrollY |
                ImGuiTableFlags.NoSavedSettings;

            if (ImGui.BeginTable("##Label_Table", 2, tableFlags, size))
            {
                Vector2 buttonSize = new Vector2(30 * _scale, 0);
                float actionsWidth = buttonSize.X * 3 + padX * 2;

                ImGui.TableSetupColumn("Label Name", ImGuiTableColumnFlags.WidthStretch, 0, 0);
                ImGui.TableSetupColumn("Actions", ImGuiTableColumnFlags.WidthFixed, actionsWidth, 1);

                ImGui.TableSetupScrollFreeze(0, 1);
                ImGui.TableHeadersRow();

                int i = 0;
                for (; i < Labels.Count; i++)
                {
                    ImGui.PushID($"##Label_Table_Row_{i}");
                    ImGui.TableNextRow(ImGuiTableRowFlags.None, 28);

                    Label label = Labels[i];

                    if (ImGui.TableSetColumnIndex(0))
                    {
                        ImGui.SetCursorPosY(ImGui.GetCursorPosY() + 3f * _scale);
                        ImGui.Text(label.Name);
                    }

                    if (ImGui.TableSetColumnIndex(1))
                    {
                        ImGui.SetCursorPosY(ImGui.GetCursorPosY() + 1f * _scale);
                        DrawHelpers.DrawButton(string.Empty, FontAwesomeIcon.Pen, () => EditLabel(label), "Edit", buttonSize);

                        ImGui.SameLine();
                        DrawHelpers.DrawButton(string.Empty, FontAwesomeIcon.Upload, () => ExportLabel(label), "Export", buttonSize);

                        ImGui.SameLine();
                        DrawHelpers.DrawButton(string.Empty, FontAwesomeIcon.Trash, () => DeleteLabel(label), "Delete", buttonSize);
                    }
                }

                ImGui.PushID($"##Label_Table_Row_{i + 1}");
                ImGui.TableNextRow(ImGuiTableRowFlags.None, 28);
                if (ImGui.TableSetColumnIndex(0))
                {
                    ImGui.SetCursorPosY(ImGui.GetCursorPosY() + 1f * _scale);
                    ImGui.PushItemWidth(ImGui.GetColumnWidth());
                    ImGui.InputTextWithHint("##LabelInput", "New Label Name", ref _labelInput, 10000);
                    ImGui.PopItemWidth();
                }

                if (ImGui.TableSetColumnIndex(1))
                {
                    ImGui.SetCursorPosY(ImGui.GetCursorPosY() + 1f);
                    DrawHelpers.DrawButton(string.Empty, FontAwesomeIcon.Plus, () => AddLabel(_labelInput), "Create Label", buttonSize);

                    ImGui.SameLine();
                    DrawHelpers.DrawButton(string.Empty, FontAwesomeIcon.Download, () => ImportLabel(), "Import Label", buttonSize);
                }

                ImGui.EndTable();
            }
        }

        private void AddLabel(string name)
        {
            if (!string.IsNullOrEmpty(name))
            {
                Labels.Add(new Label(name));
            }

            _labelInput = string.Empty;
        }

        private void ImportLabel()
        {
            string importString = string.Empty;
            try
            {
                importString = ImGui.GetClipboardText();
            }
            catch
            {
                DrawHelpers.DrawNotification("Failed to read from clipboard!", NotificationType.Error);
                return;
            }

            UIElement? newElement = ConfigHelpers.GetFromImportString<UIElement>(importString);

            if (newElement is Label label)
            {
                Labels.Add(label);
            }
            else
            {
                DrawHelpers.DrawNotification("Failed to Import Element!", NotificationType.Error);
            }

            _labelInput = string.Empty;
        }

        private void EditLabel(Label label)
        {
            Singletons.Get<PluginManager>().Edit(label);
        }

        private void ExportLabel(Label label)
        {
            ConfigHelpers.ExportToClipboard<Label>(label);
        }

        private void DeleteLabel(Label label)
        {
            Labels.Remove(label);
        }
    }
}
