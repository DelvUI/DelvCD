using Dalamud.Interface;
using DelvCD.Helpers;
using DelvCD.Helpers.DataSources;
using FFXIVClientStructs.FFXIV.Client.Game;
using ImGuiNET;
using System;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Text.Json.Serialization;

namespace DelvCD.Config
{
    public class ItemCooldownTrigger : TriggerOptions
    {
        [JsonIgnore] private float _scale => ImGuiHelpers.GlobalScale;

        [JsonIgnore] private string _triggerNameInput = string.Empty;
        [JsonIgnore] private string _cooldownValueInput = string.Empty;

        public string TriggerName = string.Empty;

        public bool Cooldown = false;
        public TriggerDataOp CooldownOp = TriggerDataOp.GreaterThan;
        public float CooldownValue;

        public override TriggerType Type => TriggerType.ItemCooldown;
        public override TriggerSource Source => TriggerSource.Player;

        [JsonIgnore] private ItemCooldownDataSource _dataSource = new();
        [JsonIgnore] public override DataSource DataSource => _dataSource;

        public override bool IsTriggered(bool preview)
        {
            if (!TriggerData.Any())
            {
                return false;
            }

            if (preview)
            {
                _dataSource.Item_Cooldown_Timer = 10;
                _dataSource.Item_Cooldown_Stacks = 1;
                _dataSource.Max_Item_Cooldown_Stacks = 1;
                _dataSource.Icon = TriggerData.FirstOrDefault()?.Icon ?? 0;

                return true;
            }

            ActionHelpers helper = Singletons.Get<ActionHelpers>();
            TriggerData actionTrigger = TriggerData.First();
            helper.GetItemRecastInfo(actionTrigger.Id, out RecastInfo recastInfo);

            _dataSource.Max_Item_Cooldown_Timer = recastInfo.RecastTime;
            _dataSource.Item_Cooldown_Timer = recastInfo.RecastTime - recastInfo.RecastTimeElapsed;
            _dataSource.Icon = actionTrigger.Icon;
            _dataSource.Id = actionTrigger.Id;

            _dataSource.Item_Cooldown_Stacks = GetQuantity(actionTrigger.Id);
            _dataSource.Max_Item_Cooldown_Stacks = _dataSource.Item_Cooldown_Stacks;

            return !Cooldown || Utils.GetResult(_dataSource.Item_Cooldown_Timer, CooldownOp, CooldownValue);
        }

        private unsafe int GetQuantity(uint itemId)
        {
            InventoryManager* manager = InventoryManager.Instance();
            InventoryType[] inventoryTypes = new InventoryType[]
            {
                InventoryType.Inventory1,
                InventoryType.Inventory2,
                InventoryType.Inventory3,
                InventoryType.Inventory4
            };

            foreach (InventoryType inventoryType in inventoryTypes)
            {
                InventoryContainer* container = manager->GetInventoryContainer(inventoryType);
                if (container is not null)
                {
                    for (int i = 0; i < container->Size; i++)
                    {
                        InventoryItem* item = container->GetInventorySlot(i);

                        if (item is not null && item->ItemID == itemId)
                        {
                            return (int)item->Quantity;
                        }
                    }
                }
            }

            return 0;
        }

        public override void DrawTriggerOptions(Vector2 size, float padX, float padY)
        {
            if (string.IsNullOrEmpty(_triggerNameInput))
            {
                _triggerNameInput = TriggerName;
            }

            if (ImGui.InputTextWithHint("Item", "Item Name or ID", ref _triggerNameInput, 32, ImGuiInputTextFlags.EnterReturnsTrue))
            {
                TriggerData.Clear();
                if (!string.IsNullOrEmpty(_triggerNameInput))
                {
                    ActionHelpers.FindItemEntries(_triggerNameInput).ForEach(t => AddTriggerData(t));
                }
            }

            DrawHelpers.DrawSpacing(1);
            ImGui.Text("Trigger Conditions");
            string[] operatorOptions = TriggerOptions.OperatorOptions;
            float optionsWidth = 100 * _scale + padX;
            float opComboWidth = 55 * _scale;
            float valueInputWidth = 45 * _scale;
            float padWidth = 0;

            DrawHelpers.DrawNestIndicator(1);
            ImGui.Checkbox("Cooldown", ref Cooldown);
            if (Cooldown)
            {
                ImGui.SameLine();
                padWidth = ImGui.CalcItemWidth() - ImGui.GetCursorPosX() - optionsWidth + padX;
                ImGui.SetCursorPosX(ImGui.GetCursorPosX() + padWidth);
                ImGui.PushItemWidth(opComboWidth);
                ImGui.Combo("##CooldownOpCombo", ref Unsafe.As<TriggerDataOp, int>(ref CooldownOp), operatorOptions, operatorOptions.Length);
                ImGui.PopItemWidth();
                ImGui.SameLine();

                if (string.IsNullOrEmpty(_cooldownValueInput))
                {
                    _cooldownValueInput = CooldownValue.ToString();
                }

                ImGui.PushItemWidth(valueInputWidth);
                if (ImGui.InputText("Seconds##CooldownValue", ref _cooldownValueInput, 10, ImGuiInputTextFlags.CharsDecimal))
                {
                    if (float.TryParse(_cooldownValueInput, out float value))
                    {
                        CooldownValue = value;
                    }

                    _cooldownValueInput = CooldownValue.ToString();
                }

                ImGui.PopItemWidth();
            }
        }

        private void ResetTrigger()
        {
            TriggerData.Clear();
            TriggerName = string.Empty;
            _triggerNameInput = string.Empty;
        }

        private void AddTriggerData(TriggerData triggerData)
        {
            TriggerName = triggerData.Name.ToString();
            _triggerNameInput = TriggerName;
            TriggerData.Add(triggerData);
            Dalamud.Logging.PluginLog.Information($"{triggerData.Name}: {triggerData.Icon}");
        }
    }
}