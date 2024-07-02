using System;
using System.Collections.Generic;
using System.Numerics;
using System.Runtime.CompilerServices;
using Dalamud.Interface;
using Dalamud.Interface.ImGuiNotification;
using Dalamud.Interface.Utility;
using DelvCD.Helpers;
using DelvCD.UIElements;
using ImGuiNET;
using Newtonsoft.Json;

namespace DelvCD.Config
{
    public class ElementListConfig : IConfigPage
    {
        [JsonIgnore] private float _scale => ImGuiHelpers.GlobalScale;
        private float MenuBarHeight => 40 * _scale;

        [JsonIgnore] private ElementType _selectedType = ElementType.Icon;
        [JsonIgnore] private string _input = string.Empty;
        [JsonIgnore] private string[] _options = new string[] { "Icon", "Bar", "Group" };
        [JsonIgnore] private int _swapX = -1;
        [JsonIgnore] private int _swapY = -1;

        public string Name => "Elements";

        public List<UIElement> UIElements { get; init; }

        public ElementListConfig()
        {
            UIElements = new List<UIElement>();
        }

        public IConfigPage GetDefault() => new ElementListConfig();

        public void DrawConfig(IConfigurable parent, Vector2 size, float padX, float padY)
        {
            DrawCreateMenu(size, padX);
            DrawUIElementTable(size.AddY(-padY), padX);
        }

        private void DrawCreateMenu(Vector2 size, float padX)
        {
            Vector2 buttonSize = new Vector2(40 * _scale, 0);
            float comboWidth = 100 * _scale;
            float textInputWidth = size.X - buttonSize.X * 2 - comboWidth - padX * 5;

            if (ImGui.BeginChild("##Buttons", new Vector2(size.X, MenuBarHeight), true))
            {
                ImGui.PushItemWidth(textInputWidth);
                ImGui.InputTextWithHint("##Input", "New Element Name", ref _input, 100);
                ImGui.PopItemWidth();

                ImGui.SameLine();
                ImGui.PushItemWidth(comboWidth);
                ImGui.Combo("##Type", ref Unsafe.As<ElementType, int>(ref _selectedType), _options, _options.Length);

                ImGui.SameLine();
                DrawHelpers.DrawButton(string.Empty, FontAwesomeIcon.Plus, () => CreateUIElement(_selectedType, _input), "Create new Element or Group", buttonSize);

                ImGui.SameLine();
                DrawHelpers.DrawButton(string.Empty, FontAwesomeIcon.Download, () => ImportUIElement(), "Import new Element or Group from Clipboard", buttonSize);
                ImGui.PopItemWidth();

                ImGui.EndChild();
            }
        }

        private void DrawUIElementTable(Vector2 size, float padX)
        {
            ImGuiTableFlags flags =
                ImGuiTableFlags.RowBg |
                ImGuiTableFlags.Borders |
                ImGuiTableFlags.BordersOuter |
                ImGuiTableFlags.BordersInner |
                ImGuiTableFlags.ScrollY |
                ImGuiTableFlags.NoSavedSettings;

            if (ImGui.BeginTable("##UIElements_Table", 4, flags, new Vector2(size.X, size.Y - MenuBarHeight)))
            {
                Vector2 buttonSize = new Vector2(30 * _scale, 0);
                int buttonCount = UIElements.Count > 1 ? 5 : 3;
                float actionsWidth = buttonSize.X * buttonCount + padX * (buttonCount - 1);
                float previewWidth = buttonSize.X;
                float typeWidth = 75 * _scale;

                ImGui.TableSetupColumn("Name", ImGuiTableColumnFlags.WidthStretch, 0, 0);
                ImGui.TableSetupColumn("Type", ImGuiTableColumnFlags.WidthFixed, typeWidth, 1);
                ImGui.TableSetupColumn("Pre.", ImGuiTableColumnFlags.WidthFixed, previewWidth, 2);
                ImGui.TableSetupColumn("Actions", ImGuiTableColumnFlags.WidthFixed, actionsWidth, 3);

                ImGui.TableSetupScrollFreeze(0, 1);
                ImGui.TableHeadersRow();

                for (int i = 0; i < UIElements.Count; i++)
                {
                    UIElement element = UIElements[i];

                    if (!string.IsNullOrEmpty(_input) &&
                        !element.Name.Contains(_input, StringComparison.OrdinalIgnoreCase))
                    {
                        continue;
                    }

                    ImGui.PushID(i.ToString());
                    ImGui.TableNextRow(ImGuiTableRowFlags.None, 28);

                    if (ImGui.TableSetColumnIndex(0))
                    {
                        ImGui.SetCursorPosY(ImGui.GetCursorPosY() + 3f * _scale);
                        ImGui.Text(element.Name);
                    }

                    if (ImGui.TableSetColumnIndex(1))
                    {
                        ImGui.SetCursorPosY(ImGui.GetCursorPosY() + 3f * _scale);
                        ImGui.Text(element.Type.ToString());
                    }

                    if (ImGui.TableSetColumnIndex(2))
                    {
                        ImGui.SetCursorPosY(ImGui.GetCursorPosY() + 1f * _scale);
                        ImGui.Checkbox("##Preview", ref element.Preview);
                        if (ImGui.IsItemHovered())
                        {
                            ImGui.SetTooltip("Preview");
                        }
                    }

                    if (ImGui.TableSetColumnIndex(3))
                    {
                        ImGui.SetCursorPosY(ImGui.GetCursorPosY() + 1f * _scale);
                        DrawHelpers.DrawButton(string.Empty, FontAwesomeIcon.Pen, () => EditUIElement(element), "Edit", buttonSize);

                        if (UIElements.Count > 1)
                        {
                            ImGui.SameLine();
                            DrawHelpers.DrawButton(string.Empty, FontAwesomeIcon.ArrowUp, () => Swap(i, i - 1), "Move Up", buttonSize);

                            ImGui.SameLine();
                            DrawHelpers.DrawButton(string.Empty, FontAwesomeIcon.ArrowDown, () => Swap(i, i + 1), "Move Down", buttonSize);
                        }

                        ImGui.SameLine();
                        DrawHelpers.DrawButton(string.Empty, FontAwesomeIcon.Upload, () => ExportUIElement(element), "Export", buttonSize);

                        ImGui.SameLine();
                        DrawHelpers.DrawButton(string.Empty, FontAwesomeIcon.Trash, () => DeleteUIElement(element), "Delete", buttonSize);
                    }
                }

                ImGui.EndTable();
            }

            if (_swapX < UIElements.Count && _swapX >= 0 &&
                _swapY < UIElements.Count && _swapY >= 0)
            {
                UIElement temp = UIElements[_swapX];
                UIElements[_swapX] = UIElements[_swapY];
                UIElements[_swapY] = temp;

                _swapX = -1;
                _swapY = -1;
            }
        }

        private void Swap(int x, int y)
        {
            _swapX = x;
            _swapY = y;
        }

        private void CreateUIElement(ElementType type, string name)
        {
            if (!string.IsNullOrEmpty(name))
            {
                UIElement? newElement = type switch
                {
                    ElementType.Group => new Group(name),
                    ElementType.Icon => Icon.GetDefaultUIElementIcon(name),
                    ElementType.Bar => Bar.GetDefaultUIElementBar(name),
                    _ => null
                };

                if (newElement is not null)
                {
                    UIElements.Add(newElement);
                }
            }

            _input = string.Empty;
        }

        private void EditUIElement(UIElement element)
        {
            Singletons.Get<PluginManager>().Edit(element);
        }

        private void DeleteUIElement(UIElement element)
        {
            UIElements.Remove(element);
        }

        private void ImportUIElement()
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
            if (newElement is not null)
            {
                UIElements.Add(newElement);
            }
            else
            {
                DrawHelpers.DrawNotification("Failed to Import Element!", NotificationType.Error);
            }

            _input = string.Empty;
        }

        private void ExportUIElement(UIElement element)
        {
            ConfigHelpers.ExportToClipboard<UIElement>(element);
        }
    }
}
