using Dalamud.Interface;
using Dalamud.Interface.Utility;
using DelvCD.Helpers;
using DelvCD.Helpers.DataSources;
using Dalamud.Bindings.ImGui;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;

namespace DelvCD.Config
{
    public class StyleCondition<T> : IConfigurable where T : class?, IConfigPage, new()
    {
        public int TriggerDataSourceIndex = 0;
        public int Source = 0;
        public TriggerDataOp Op = TriggerDataOp.GreaterThan;
        public float Value = 0;

        public T Style { get; set; } = new T();

        public string Name
        {
            get => Style.Name;
            set { }
        }

        public override string ToString() => $"Condition [{Name}]";

        public StyleCondition() { }

        public StyleCondition(T? defaultStyle)
        {
            Style = ConfigHelpers.SerializedClone<T>(defaultStyle) ?? new T();
        }

        public IEnumerable<IConfigPage> GetConfigPages()
        {
            yield return Style;
        }

        public void ImportPage(IConfigPage page)
        {
            if (page is T t)
            {
                Style = t;
            }
        }

        public bool GetResult(DataSource data)
        {
            float value = data.GetConditionValue(Source);

            return Op switch
            {
                TriggerDataOp.Equals => value == Value,
                TriggerDataOp.NotEquals => value != Value,
                TriggerDataOp.LessThan => value < Value,
                TriggerDataOp.GreaterThan => value > Value,
                TriggerDataOp.LessThanEq => value <= Value,
                TriggerDataOp.GreaterThanEq => value >= Value,
                _ => false
            } || Singletons.Get<PluginManager>().IsConfigurableOpen(this);
        }

        public void UpdateDataSources(DataSource[] dataSources, bool needsDataSourceCheck = false)
        {
            if (Style is IconStyleConfig iconStyle)
            {
                iconStyle.UpdateDataSources(dataSources, needsDataSourceCheck);
            }
            else if (Style is BarStyleConfig barStyle)
            {
                barStyle.UpdateDataSources(dataSources);
            }
        }
    }

    public class StyleConditions<T> : IConfigPage where T : class?, IConfigPage, new()
    {
        [JsonIgnore] private float _scale => ImGuiHelpers.GlobalScale;

        [JsonIgnore] private Type[] _dataSourcesTypes = new Type[0];
        [JsonIgnore] private string[] _triggerOptions = new string[0];
        [JsonIgnore] private string[][] _sourceOptions = new string[][] { };

        [JsonIgnore] private static readonly string[] _operatorOptions = new string[] { "==", "!=", "<", ">", "<=", ">=" };
        [JsonIgnore] private static readonly string _text = $"Add Conditions below to specify alternate appearance configurations under certain conditions.";

        [JsonIgnore] private string _styleConditionValueInput = string.Empty;
        [JsonIgnore] private int _swapX = -1;
        [JsonIgnore] private int _swapY = -1;
        [JsonIgnore] private T? _defaultStyle;

        public string Name => "Conditions";
        public IConfigPage GetDefault() => new StyleConditions<T>();

        public List<StyleCondition<T>> Conditions { get; set; } = new List<StyleCondition<T>>();

        public T? GetStyle(DataSource[]? data)
        {
            if (!Conditions.Any() || data is null)
            {
                return null;
            }

            foreach (StyleCondition<T> condition in Conditions)
            {
                if (condition.TriggerDataSourceIndex < data.Length && condition.GetResult(data[condition.TriggerDataSourceIndex]))
                {
                    return condition.Style;
                }
            }

            return null;
        }

        private bool CompareDataSources(DataSource[] dataSources)
        {
            Type[] types = dataSources.Select(x => x.GetType()).ToArray();

            if (dataSources.Length != _dataSourcesTypes.Length)
            {
                _dataSourcesTypes = types;
                return false;
            }

            for (int i = 0; i < dataSources.Length; i++)
            {
                if (types[i] != _dataSourcesTypes[i])
                {
                    return false;
                }
            }

            return true;
        }

        public void UpdateDataSources(DataSource[] dataSources, bool needsDataSourceCheck = false)
        {
            if (CompareDataSources(dataSources))
            {
                return;
            }

            _triggerOptions = new string[dataSources.Length];
            _sourceOptions = new string[dataSources.Length][];

            for (int i = 0; i < dataSources.Length; i++)
            {
                _triggerOptions[i] = $"Trigger {i + 1}";
                _sourceOptions[i] = dataSources[i].ConditionFieldNames.ToArray();
            }

            foreach (var condition in Conditions)
            {
                condition.UpdateDataSources(dataSources, needsDataSourceCheck);
            }
        }

        public void UpdateDefaultStyle(T style)
        {
            _defaultStyle = style;
        }

        public void DrawConfig(IConfigurable parent, Vector2 size, float padX, float padY)
        {
            ImGui.Text(_text);
            size = size.AddY(-(15 * _scale + padY));
            if (ImGui.BeginChild("##StyleConditions", new Vector2(size.X, size.Y), true))
            {
                ImGuiTableFlags tableFlags =
                    ImGuiTableFlags.RowBg |
                    ImGuiTableFlags.Borders |
                    ImGuiTableFlags.BordersOuter |
                    ImGuiTableFlags.BordersInner |
                    ImGuiTableFlags.ScrollY |
                    ImGuiTableFlags.NoSavedSettings;

                if (ImGui.BeginTable("##Conditions_Table", 6, tableFlags, new Vector2(size.X - padX * 2, size.Y - ImGui.GetCursorPosY() - padY * 2)))
                {
                    Vector2 buttonSize = new(30 * _scale, 0);
                    int buttonCount = Conditions.Count > 1 ? 4 : 2;
                    float actionsWidth = buttonSize.X * buttonCount + padX * (buttonCount - 1);
                    ImGui.TableSetupColumn("Condition", ImGuiTableColumnFlags.WidthFixed, 55 * _scale, 0);
                    ImGui.TableSetupColumn("Data Source", ImGuiTableColumnFlags.WidthFixed, 90 * _scale, 1);
                    ImGui.TableSetupColumn("Data", ImGuiTableColumnFlags.WidthStretch, 0, 2);
                    ImGui.TableSetupColumn("Operator", ImGuiTableColumnFlags.WidthFixed, 55 * _scale, 3);
                    ImGui.TableSetupColumn("Value", ImGuiTableColumnFlags.WidthStretch, 0, 4);
                    ImGui.TableSetupColumn("Actions", ImGuiTableColumnFlags.WidthFixed, actionsWidth, 5);

                    ImGui.TableSetupScrollFreeze(0, 1);
                    ImGui.TableHeadersRow();

                    for (int i = 0; i < Conditions.Count; i++)
                    {
                        ImGui.PushID($"##Conditions_Table_Row_{i}");
                        ImGui.TableNextRow(ImGuiTableRowFlags.None, 28);

                        DrawStyleConditionRow(i);
                    }

                    ImGui.PushID($"##Conditions_Table_Row_{Conditions.Count}");
                    ImGui.TableNextRow(ImGuiTableRowFlags.None, 28);
                    ImGui.TableSetColumnIndex(5);
                    DrawHelpers.DrawButton(string.Empty, FontAwesomeIcon.Plus, () => Conditions.Add(new StyleCondition<T>(_defaultStyle)), "New Condition", buttonSize);

                    ImGui.EndTable();
                }

                if (_swapX < Conditions.Count && _swapX >= 0 &&
                    _swapY < Conditions.Count && _swapY >= 0)
                {
                    var temp = Conditions[_swapX];
                    Conditions[_swapX] = Conditions[_swapY];
                    Conditions[_swapY] = temp;

                    _swapX = -1;
                    _swapY = -1;
                }
            }

            ImGui.EndChild();
        }

        private void DrawStyleConditionRow(int i)
        {
            StyleCondition<T> condition = Conditions[i];

            if (ImGui.TableSetColumnIndex(0))
            {
                if (i == 0)
                {
                    ImGui.SetCursorPosY(ImGui.GetCursorPosY() + 3f * _scale);
                    ImGui.Text("IF");
                }
                else
                {
                    ImGui.SetCursorPosY(ImGui.GetCursorPosY() + 3f * _scale);
                    ImGui.Text("ELSE IF");
                }
            }

            if (ImGui.TableSetColumnIndex(1))
            {
                if (_triggerOptions.Length > 0)
                {
                    condition.TriggerDataSourceIndex = Math.Clamp(condition.TriggerDataSourceIndex, 0, _triggerOptions.Length - 1);

                    ImGui.SetCursorPosY(ImGui.GetCursorPosY() + 1f * _scale);
                    ImGui.PushItemWidth(ImGui.GetColumnWidth());
                    ImGui.Combo("##TriggerCombo", ref condition.TriggerDataSourceIndex, _triggerOptions, _triggerOptions.Length);
                    ImGui.PopItemWidth();
                }
            }

            if (ImGui.TableSetColumnIndex(2))
            {
                if (condition.TriggerDataSourceIndex < _sourceOptions.Length &&
                    _sourceOptions[condition.TriggerDataSourceIndex].Length > 0)
                {
                    condition.Source = Math.Clamp(condition.Source, 0, _sourceOptions[condition.TriggerDataSourceIndex].Length - 1);

                    ImGui.SetCursorPosY(ImGui.GetCursorPosY() + 1f * _scale);
                    ImGui.PushItemWidth(ImGui.GetColumnWidth());
                    ImGui.Combo("##SourceCombo", ref condition.Source, _sourceOptions[condition.TriggerDataSourceIndex], _sourceOptions[condition.TriggerDataSourceIndex].Length);
                    ImGui.PopItemWidth();
                }
            }

            if (ImGui.TableSetColumnIndex(3))
            {
                ImGui.SetCursorPosY(ImGui.GetCursorPosY() + 1f * _scale);
                ImGui.PushItemWidth(ImGui.GetColumnWidth());
                ImGui.Combo("##OpCombo", ref Unsafe.As<TriggerDataOp, int>(ref condition.Op), _operatorOptions, _operatorOptions.Length);
                ImGui.PopItemWidth();
            }

            if (ImGui.TableSetColumnIndex(4))
            {
                ImGui.SetCursorPosY(ImGui.GetCursorPosY() + 1f);
                ImGui.PushItemWidth(ImGui.GetColumnWidth());

                _styleConditionValueInput = condition.Value.ToString();
                ImGui.InputText("##InputFloat", ref _styleConditionValueInput, 10);
                if (float.TryParse(_styleConditionValueInput, out float value))
                {
                    condition.Value = value;
                }

                ImGui.PopItemWidth();
            }

            if (ImGui.TableSetColumnIndex(5))
            {
                ImGui.SetCursorPosY(ImGui.GetCursorPosY() + 1f * _scale);
                Vector2 buttonSize = new(30 * _scale, 0);
                DrawHelpers.DrawButton(string.Empty, FontAwesomeIcon.Pen, () => Singletons.Get<PluginManager>().Edit(condition), "Edit Style", buttonSize);

                if (Conditions.Count > 1)
                {
                    ImGui.SameLine();
                    DrawHelpers.DrawButton(string.Empty, FontAwesomeIcon.ArrowUp, () => Swap(i, i - 1), "Move Up", buttonSize);

                    ImGui.SameLine();
                    DrawHelpers.DrawButton(string.Empty, FontAwesomeIcon.ArrowDown, () => Swap(i, i + 1), "Move Down", buttonSize);
                }

                ImGui.SameLine();
                DrawHelpers.DrawButton(string.Empty, FontAwesomeIcon.Trash, () => Conditions.Remove(condition), "Remove Condition", buttonSize);
            }
        }

        private void Swap(int x, int y)
        {
            _swapX = x;
            _swapY = y;
        }
    }
}