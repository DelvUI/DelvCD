﻿using Dalamud.Interface;
using Dalamud.Interface.Utility;
using DelvCD.Helpers;
using ImGuiNET;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace DelvCD.Config
{
    public class FontConfig : IConfigPage
    {
        [JsonIgnore] private float _scale => ImGuiHelpers.GlobalScale;

        public string Name => "Fonts";

        public IConfigPage GetDefault() => new FontConfig();

        [JsonIgnore] private static string? _fontPath = FontsManager.GetUserFontPath();
        [JsonIgnore] private int _selectedFont = 0;
        [JsonIgnore] private int _selectedSize = 23;
        [JsonIgnore] private string[] _fonts = FontsManager.GetFontNamesFromPath(FontsManager.GetUserFontPath());
        [JsonIgnore] private string[] _sizes = Enumerable.Range(1, 40).Select(i => i.ToString()).ToArray();
        [JsonIgnore] private bool _chinese = false;
        [JsonIgnore] private bool _korean = false;


        public Dictionary<string, FontData> Fonts { get; init; }

        public FontConfig()
        {
            RefreshFontList();
            Fonts = new Dictionary<string, FontData>();

            foreach (string fontKey in FontsManager.DefaultFontKeys)
            {
                string[] splits = fontKey.Split("_", StringSplitOptions.RemoveEmptyEntries);
                if (splits.Length == 2 && int.TryParse(splits[1], out int size))
                {
                    FontData newFont = new FontData(splits[0], size, false, false);
                    string key = FontsManager.GetFontKey(newFont);
                    Fonts.Add(key, newFont);
                }
            }
        }

        public void DrawConfig(IConfigurable parent, Vector2 size, float padX, float padY)
        {
            if (_fonts.Length == 0)
            {
                RefreshFontList();
            }

            if (ImGui.BeginChild("##FontConfig", new Vector2(size.X, size.Y), true))
            {
                if (_fontPath is not null)
                {
                    float cursorY = ImGui.GetCursorPosY();
                    ImGui.SetCursorPosY(cursorY + 2f * _scale);
                    ImGui.Text("Copy Font Folder Path to Clipboard: ");
                    ImGui.SameLine();

                    Vector2 buttonSize = new Vector2(40 * _scale, 0);
                    ImGui.SetCursorPosY(cursorY);
                    DrawHelpers.DrawButton(string.Empty, FontAwesomeIcon.Copy, () => ImGui.SetClipboardText(_fontPath), null, buttonSize);

                    ImGui.Combo("Font", ref _selectedFont, _fonts, _fonts.Length);
                    ImGui.SameLine();
                    DrawHelpers.DrawButton(string.Empty, FontAwesomeIcon.Sync, () => RefreshFontList(), "Reload Font List", buttonSize);

                    ImGui.Combo("Size", ref _selectedSize, _sizes, _sizes.Length);
                    ImGui.SameLine();
                    ImGui.SetCursorPosX(ImGui.GetCursorPosX() + 3f * _scale);
                    DrawHelpers.DrawButton(string.Empty, FontAwesomeIcon.Plus, () => AddFont(_selectedFont, _selectedSize), "Add Font", buttonSize);

                    ImGui.Checkbox("Support Chinese/Japanese", ref _chinese);
                    ImGui.SameLine();
                    ImGui.Checkbox("Support Korean", ref _korean);

                    DrawHelpers.DrawSpacing(1);
                    ImGui.Text("Font List");

                    ImGuiTableFlags tableFlags =
                        ImGuiTableFlags.RowBg |
                        ImGuiTableFlags.Borders |
                        ImGuiTableFlags.BordersOuter |
                        ImGuiTableFlags.BordersInner |
                        ImGuiTableFlags.ScrollY |
                        ImGuiTableFlags.NoSavedSettings;

                    if (ImGui.BeginTable("##Font_Table", 5, tableFlags, new Vector2(size.X - padX * 2, size.Y - ImGui.GetCursorPosY() - padY * 2)))
                    {
                        ImGui.TableSetupColumn("Name", ImGuiTableColumnFlags.WidthStretch, 0, 0);
                        ImGui.TableSetupColumn("Size", ImGuiTableColumnFlags.WidthFixed, 40 * _scale, 1);
                        ImGui.TableSetupColumn("CN/JP", ImGuiTableColumnFlags.WidthFixed, 40 * _scale, 2);
                        ImGui.TableSetupColumn("KR", ImGuiTableColumnFlags.WidthFixed, 40 * _scale, 3);
                        ImGui.TableSetupColumn("Actions", ImGuiTableColumnFlags.WidthFixed, 45 * _scale, 4);

                        ImGui.TableSetupScrollFreeze(0, 1);
                        ImGui.TableHeadersRow();

                        for (int i = 0; i < Fonts.Keys.Count; i++)
                        {
                            ImGui.PushID(i.ToString());
                            ImGui.TableNextRow(ImGuiTableRowFlags.None, 28);

                            string key = Fonts.Keys.ElementAt(i);
                            FontData font = Fonts[key];

                            if (ImGui.TableSetColumnIndex(0))
                            {
                                ImGui.SetCursorPosY(ImGui.GetCursorPosY() + 3f * _scale);
                                ImGui.Text(key);
                            }

                            if (ImGui.TableSetColumnIndex(1))
                            {
                                ImGui.SetCursorPosY(ImGui.GetCursorPosY() + 3f * _scale);
                                ImGui.Text(font.Size.ToString());
                            }

                            if (ImGui.TableSetColumnIndex(2))
                            {
                                ImGui.SetCursorPosY(ImGui.GetCursorPosY() + 3f * _scale);
                                ImGui.Text(font.Chinese ? "Yes" : "No");
                            }

                            if (ImGui.TableSetColumnIndex(3))
                            {
                                ImGui.SetCursorPosY(ImGui.GetCursorPosY() + 3f * _scale);
                                ImGui.Text(font.Korean ? "Yes" : "No");
                            }

                            if (ImGui.TableSetColumnIndex(4))
                            {
                                if (!FontsManager.DefaultFontKeys.Contains(key))
                                {
                                    ImGui.SetCursorPosY(ImGui.GetCursorPosY() + 1f * _scale);
                                    DrawHelpers.DrawButton(string.Empty, FontAwesomeIcon.Trash, () => RemoveFont(key), "Remove Font", new Vector2(45, 0));
                                }
                            }
                        }

                        ImGui.EndTable();
                    }
                }
            }

            ImGui.EndChild();
        }

        public void RefreshFontList()
        {
            _fonts = FontsManager.GetFontNamesFromPath(FontsManager.GetUserFontPath());
        }

        public void AddFont(FontData newFont)
        {
            string key = FontsManager.GetFontKey(newFont);
            if (!Fonts.ContainsKey(key))
            {
                Fonts.Add(key, newFont);
                Singletons.Get<FontsManager>().UpdateFonts(Fonts.Values);
            }
        }

        private void AddFont(int fontIndex, int size)
        {
            FontData newFont = new FontData(_fonts[fontIndex], size + 1, _chinese, _korean);
            AddFont(newFont);
        }

        private void RemoveFont(string key)
        {
            Fonts.Remove(key);
            Singletons.Get<FontsManager>().UpdateFonts(Fonts.Values);
        }
    }
}
