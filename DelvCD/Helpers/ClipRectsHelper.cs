using FFXIVClientStructs.FFXIV.Client.UI;
using FFXIVClientStructs.FFXIV.Component.GUI;
using ImGuiNET;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Runtime.InteropServices;

namespace DelvCD.Helpers
{
    public class ClipRectsHelper
    {
        // these are ordered by priority, if 2 game windows are on top of a DelvUI element
        // the one that comes first in this list is the one that will be clipped around
        internal static List<string> AddonNames = new List<string>()
        {
            "ContextMenu",
            "ItemDetail", // tooltip
            "ActionDetail", // tooltip
            "AreaMap",
            "JournalAccept",
            "Talk",
            "Teleport",
            "ActionMenu",
            "Character",
            "CharacterInspect",
            "CharacterTitle",
            "Tryon",
            "ArmouryBoard",
            "RecommendList",
            "GearSetList",
            "MiragePrismMiragePlate",
            "ItemSearch",
            "RetainerList",
            "Bank",
            "RetainerSellList",
            "RetainerSell",
            "SelectString",
            "Shop",
            "ShopExchangeCurrency",
            "ShopExchangeItem",
            "CollectablesShop",
            "MateriaAttach",
            "Repair",
            "Inventory",
            "InventoryLarge",
            "InventoryExpansion",
            "InventoryEvent",
            "InventoryBuddy",
            "Buddy",
            "BuddyEquipList",
            "BuddyInspect",
            "Currency",
            "Macro",
            "PcSearchDetail",
            "Social",
            "SocialDetailA",
            "SocialDetailB",
            "LookingForGroupSearch",
            "LookingForGroupCondition",
            "LookingForGroupDetail",
            "LookingForGroup",
            "ReadyCheck",
            "Marker",
            "FieldMarker",
            "CountdownSettingDialog",
            "CircleFinder",
            "CircleList",
            "CircleNameInputString",
            "Emote",
            "FreeCompany",
            "FreeCompanyProfile",
            "HousingSubmenu",
            "HousingSignBoard",
            "HousingMenu",
            "CharaCard",
            "CharaCardDesignSetting",
            "CharaCardProfileSetting",
            "CharaCardPermissionSetting",
            "BannerList",
            "BannerEditor",
            "SelectString",
            "Description",
            "McGuffin",
            "AkatsukiNote",
            "DescriptionYTC",
            "MYCWarResultNoteBook",
            "CrossWorldLinkshell",
            "ContactList",
            "CircleBookInputString",
            "CircleBookQuestion",
            "CircleBookGroupSetting",
            "MultipleHelpWindow",
            "CircleFinderSetting",
            "CircleBook",
            "CircleBookWriteMessage",
            "ColorantColoring",
            "MonsterNote",
            "RecipeNote",
            "GatheringNote",
            "ContentsNote",
            "SpearFishing",
            "Orchestrion",
            "MountNoteBook",
            "MinionNoteBook",
            "AetherCurrent",
            "MountSpeed",
            "FateProgress",
            "SystemMenu",
            "ConfigCharacter",
            "ConfigSystem",
            "ConfigKeybind",
            "AOZNotebook",
            "AOZActiveSetInputString",
            "PvpProfile",
            "GoldSaucerInfo",
            "Achievement",
            "RecommendList",
            "JournalDetail",
            "Journal",
            "ContentsFinder",
            "ContentsFinderSetting",
            "ContentsFinderMenu",
            "ContentsInfo",
            "Dawn",
            "DawnStory",
            "DawnStoryMemberSelect",
            "BeginnersMansionProblem",
            "BeginnersMansionProblemCompList",
            "SupportDesk",
            "HowToList",
            "HudLayout",
            "LinkShell",
            "ChatConfig",
            "ColorPicker",
            "PlayGuide",
            "SelectYesno"
        };

        private List<ClipRect> _clipRects = new List<ClipRect>();

        public unsafe void Update()
        {
            _clipRects.Clear();

            AtkStage* stage = AtkStage.GetSingleton();
            if (stage == null) { return; }

            RaptureAtkUnitManager* manager = stage->RaptureAtkUnitManager;
            if (manager == null) { return; }

            AtkUnitList* loadedUnitsList = &manager->AtkUnitManager.AllLoadedUnitsList;
            if (loadedUnitsList == null) { return; }

            AtkUnitBase** addonList = &loadedUnitsList->AtkUnitEntries;
            if (addonList == null) { return; }

            for (var i = 0; i < loadedUnitsList->Count; i++)
            {
                try
                {
                    AtkUnitBase* addon = addonList[i];
                    if (addon == null || !addon->IsVisible || addon->WindowNode == null || addon->Scale == 0)
                    {
                        continue;
                    }

                    string? name = Marshal.PtrToStringAnsi(new IntPtr(addon->Name));
                    if (name == null || !AddonNames.Contains(name))
                    {
                        continue;
                    }

                    var margin = 5 * addon->Scale;
                    var bottomMargin = 13 * addon->Scale;

                    var clipRect = new ClipRect(
                        new Vector2(addon->X + margin, addon->Y + margin),
                        new Vector2(
                            addon->X + addon->WindowNode->AtkResNode.Width * addon->Scale - margin,
                            addon->Y + addon->WindowNode->AtkResNode.Height * addon->Scale - bottomMargin
                        )
                    );

                    // just in case this causes weird issues / crashes (doubt it though...)
                    if (clipRect.Max.X < clipRect.Min.X || clipRect.Max.Y < clipRect.Min.Y)
                    {
                        continue;
                    }

                    _clipRects.Add(clipRect);
                }
                catch { }
            }
        }

        public ClipRect? GetClipRectForArea(Vector2 pos, Vector2 size)
        {
            var area = new ClipRect(pos, pos + size);
            foreach (ClipRect clipRect in _clipRects)
            {
                if (clipRect.IntersectsWith(area))
                {
                    return clipRect;
                }
            }

            return null;
        }

        public bool IsPointClipped(Vector2 point)
        {
            foreach (ClipRect clipRect in _clipRects)
            {
                if (clipRect.Contains(point))
                {
                    return true;
                }
            }

            return false;
        }
    }

    public struct ClipRect
    {
        public readonly Vector2 Min;
        public readonly Vector2 Max;

        public ClipRect(Vector2 min, Vector2 max)
        {
            var screenSize = ImGui.GetMainViewport().Size;

            Min = Clamp(min, Vector2.Zero, screenSize);
            Max = Clamp(max, Vector2.Zero, screenSize);
        }

        public bool Contains(Vector2 p)
        {
            return p.X <= Max.X && p.X >= Min.X && p.Y <= Max.Y && p.Y >= Min.Y;
        }

        public bool IntersectsWith(ClipRect other)
        {
            return other.Max.X >= Min.X && other.Min.X <= Max.X &&
                other.Max.Y >= Min.Y && other.Min.Y <= Max.Y;
        }

        private static Vector2 Clamp(Vector2 vector, Vector2 min, Vector2 max)
        {
            return new Vector2(Math.Max(min.X, Math.Min(max.X, vector.X)), Math.Max(min.Y, Math.Min(max.Y, vector.Y)));
        }
    }
}
