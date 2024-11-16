using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Dalamud.Plugin.Services;
using FFXIVClientStructs.Attributes;
using FFXIVClientStructs.FFXIV.Client.Game;
using FFXIVClientStructs.FFXIV.Client.UI;
using FFXIVClientStructs.FFXIV.Client.UI.Misc;

namespace DelvCD.Helpers;

public unsafe class KeybindHelper
{
    // Referenced https://github.com/Caraxi/SimpleTweaksPlugin/blob/main/Tweaks/UiAdjustment/ControlHintMirroring.cs for a lot of this code.
    
    #region Singleton
    
    public KeybindHelper()
    {
        Singletons.Get<IClientState>().ClassJobChanged += OnJobChanged;
    }

    public static void Initialize() { Instance = new KeybindHelper(); }

    public static KeybindHelper Instance { get; private set; } = null!;

    ~KeybindHelper() { Dispose(false); }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected void Dispose(bool disposing)
    {
        if (!disposing)
        {
            return;
        }

        Instance = null!;
    }
    
    #endregion
    
    private readonly Dictionary<uint, string> _keybindActionHints = new();
    private readonly Dictionary<uint, string> _keybindItemHints = new();
    private readonly string?[] _actionBars = ["_ActionBar09", "_ActionBar", "_ActionBar01", "_ActionBar02", "_ActionBar03", "_ActionBar04", "_ActionBar05", "_ActionBar06", "_ActionBar07", "_ActionBar08"];
    
    private void ActionBarUpdateRequested(AddonActionBarX* addon) {
        var barId = addon->AddonActionBarBase.RaptureHotbarId;
        if (barId == 9)
        {
            _keybindActionHints.Clear(); // Relies on the game updating action bars in reverse order
            _keybindItemHints.Clear(); // Relies on the game updating action bars in reverse order
        }

        for (var slotIndex = (byte)(addon->AddonActionBarBase.SlotCount - 1); slotIndex < addon->AddonActionBarBase.SlotCount; slotIndex--) {
            var slot = RaptureHotbarModule.Instance()->GetSlotById(barId, slotIndex);
            if (slot->CommandType == RaptureHotbarModule.HotbarSlotType.Empty)
            {
                continue;
            }

            var str = slot->KeybindHintString;
            if (string.IsNullOrWhiteSpace(str)) {
                continue;
            }

            if (slot->CommandType == RaptureHotbarModule.HotbarSlotType.Item)
            {
                // For some reason the first 2 digits for items (well or some items) is 10?
                var itemId = StripFirstTwoDigits(slot->CommandId);
                _keybindItemHints[itemId] = str;
                continue;
            }
            
            var actionId = ActionManager.Instance()->GetAdjustedActionId(slot->CommandId);
            _keybindActionHints[actionId] = str;
        }
    }
    
    public void UpdateKeybindHints() {
        var gameGui = Singletons.Get<IGameGui>();

        foreach (var addonName in _actionBars) {
            string? nameToUse = addonName;

            AddonActionBarX* addon = !string.IsNullOrEmpty(nameToUse) 
                ? (AddonActionBarX*)gameGui.GetAddonByName(nameToUse, 1) 
                : null;

            if (addon != null) {
                ActionBarUpdateRequested(addon);
            }
        }
    }

    public string GetKeybindHint(uint id, KeybindType type)
    {
        var dictionary = type switch
        {
            KeybindType.Item => _keybindItemHints,
            KeybindType.Action => _keybindActionHints,
            _ => _keybindActionHints
        };

        return dictionary.TryGetValue(id, out var keybindHint) ? keybindHint : string.Empty;
    }

    public string GetKeybindHintFormatted(uint id, KeybindType type)
    {
        string keybindHint = GetKeybindHint(id, type);
        keybindHint = keybindHint.ToUpper();
        keybindHint = keybindHint.Replace("§", "s");      // Shift
        keybindHint = keybindHint.Replace("\u00a2", "c"); // Control
        keybindHint = keybindHint.Replace("ª", "a");      // Alt
        keybindHint = keybindHint.Replace("º", "n");      // Numpad
        return keybindHint;
    }
    
    private void OnJobChanged(uint _)
    {
        // Update the action bar when the job changes
        UpdateKeybindHints();
    }
    
    public uint StripFirstTwoDigits(uint number)
    {
        var numberStr = number.ToString();
        return numberStr.Length > 2 && uint.TryParse(numberStr.Substring(2), out var result) ? result : 0;
    }

    public enum KeybindType
    {
        Action,
        Item,
    }
}
