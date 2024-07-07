using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Dalamud.Game.ClientState.Objects.Types;
using Dalamud.Plugin.Services;
using FFXIVClientStructs.FFXIV.Client.Game;
using FFXIVClientStructs.FFXIV.Common.Component.BGCollision;
using Lumina.Excel;
using Lumina.Excel.GeneratedSheets;

using LuminaAction = Lumina.Excel.GeneratedSheets.Action;

namespace DelvCD.Helpers
{
    public class ActionHelpers
    {
        private readonly unsafe ActionManager* _actionManager;

        private readonly Dictionary<uint, ushort> _actionIdToIconId;
        private static readonly Dictionary<Job, uint> _jobActionIDs = new()
        {
            [Job.GNB] = 16137, // Keen Edge
            [Job.WAR] = 31,    // Heavy Swing
            [Job.MRD] = 31,    // Heavy Swing
            [Job.DRK] = 3617,  // Hard Slash
            [Job.PLD] = 9,     // Fast Blade
            [Job.GLA] = 9,     // Fast Blade

            [Job.SCH] = 163,   // Ruin
            [Job.AST] = 3596,  // Malefic
            [Job.WHM] = 119,   // Stone
            [Job.CNJ] = 119,   // Stone
            [Job.SGE] = 24283, // Dosis

            [Job.BRD] = 97,    // Heavy Shot
            [Job.ARC] = 97,    // Heavy Shot
            [Job.DNC] = 15989, // Cascade
            [Job.MCH] = 2866,  // Split Shot

            [Job.SMN] = 163,   // Ruin
            [Job.ACN] = 163,   // Ruin
            [Job.RDM] = 7504,  // Riposte
            [Job.BLM] = 142,   // Blizzard
            [Job.THM] = 142,   // Blizzard

            [Job.SAM] = 7477,  // Hakaze
            [Job.NIN] = 2240,  // Spinning Edge
            [Job.ROG] = 2240,  // Spinning Edge
            [Job.MNK] = 53,    // Bootshine
            [Job.PGL] = 53,    // Bootshine
            [Job.DRG] = 75,    // True Thrust
            [Job.LNC] = 75,    // True Thrust
            [Job.RPR] = 24373, // Slice

            [Job.BLU] = 11385  // Water Cannon
        };

        public unsafe ActionHelpers()
        {
            _actionManager = ActionManager.Instance();
            _actionIdToIconId = [];
        }

        public ushort GetIconIdForAction(uint actionId)
        {
            if (_actionIdToIconId.ContainsKey(actionId))
            {
                return _actionIdToIconId[actionId];
            }

            ushort icon = Singletons.Get<IDataManager>().GetExcelSheet<LuminaAction>()?.GetRow(actionId)?.Icon ?? 0;
            if (icon != 0)
            {
                _actionIdToIconId.Add(actionId, icon);
            }

            return icon;
        }

        public unsafe uint GetAdjustedActionId(uint actionId)
        {
            return _actionManager->GetAdjustedActionId(actionId);
        }

        public string? GetNameForAction(uint actionId)
        {
            string name = Singletons.Get<IDataManager>().GetExcelSheet<LuminaAction>()?.GetRow(actionId)?.Name ?? string.Empty;

            return name;
        }

        public unsafe string GetAdjustedActionName(uint actionId)
        {
            var newid = GetAdjustedActionId(actionId);

            return GetNameForAction(newid) ?? string.Empty;
        }

        public unsafe void GetAdjustedRecastInfo(uint actionId, out RecastInfo recastInfo)
        {
            recastInfo = default;
            int recastGroup = _actionManager->GetRecastGroup((int)ActionType.Action, actionId);
            RecastDetail* recastDetail = _actionManager->GetRecastGroupDetail(recastGroup);
            if (recastDetail == null)
            {
                return;
            }

            recastInfo.RecastTime = recastDetail->Total;
            recastInfo.RecastTimeElapsed = recastDetail->Elapsed;
            recastInfo.MaxCharges = ActionManager.GetMaxCharges(actionId, 90);
            if (recastInfo.MaxCharges == 1)
            {
                return;
            }

            ushort currentMaxCharges = ActionManager.GetMaxCharges(actionId, 0);
            if (currentMaxCharges == recastInfo.MaxCharges)
            {
                return;
            }

            recastInfo.RecastTime = (recastInfo.RecastTime * currentMaxCharges) / recastInfo.MaxCharges;
            recastInfo.MaxCharges = currentMaxCharges;
            if (recastInfo.RecastTimeElapsed > recastInfo.RecastTime)
            {
                recastInfo.RecastTime = 0;
                recastInfo.RecastTimeElapsed = 0;
            }

            return;
        }

        public unsafe void GetItemRecastInfo(uint actionId, out RecastInfo recastInfo)
        {
            recastInfo = default;
            recastInfo.RecastTime = _actionManager->GetRecastTime(ActionType.Item, actionId);
            recastInfo.RecastTimeElapsed = _actionManager->GetRecastTimeElapsed(ActionType.Item, actionId);
            recastInfo.MaxCharges = 1;
            return;
        }

        public unsafe bool CanUseAction(uint actionId, ActionType type = ActionType.Action, ulong targetId = 0xE000_0000)
        {
            return _actionManager->GetActionStatus(type, actionId, targetId, false, true) == 0;
        }

        public unsafe bool GetActionInRange(uint actionId, IGameObject? player, IGameObject? target)
        {
            if (player is null || target is null)
            {
                return false;
            }

            uint result = ActionManager.GetActionInRangeOrLoS(
                actionId,
                (FFXIVClientStructs.FFXIV.Client.Game.Object.GameObject*)player.Address,
                (FFXIVClientStructs.FFXIV.Client.Game.Object.GameObject*)target.Address);

            return result != 566; // 0 == in range, 565 == in range but not facing target, 566 == out of range, 562 == not in LoS
        }

        public unsafe bool IsTargetInLos(IGameObject? player, IGameObject? target, uint actionId)
        {
            if (target is null || player is null)
            {
                return false;
            }

            uint result = ActionManager.GetActionInRangeOrLoS(
                actionId,
                (FFXIVClientStructs.FFXIV.Client.Game.Object.GameObject*)player.Address,
                (FFXIVClientStructs.FFXIV.Client.Game.Object.GameObject*)target.Address);

            Vector3 delta = target.Position - player.Position;
            Vector3 origin = new(player.Position.X, player.Position.Y + 2f, player.Position.Z);
            return !BGCollisionModule.RaycastMaterialFilter(origin, Vector3.Normalize(delta), out RaycastHit hitInfo, delta.Length()) && result != 562;
        }

        public unsafe uint GetLastUsedActionId()
        {
            return _actionManager->Combo.Action;
        }

        public static List<TriggerData> FindItemEntries(string input)
        {
            ExcelSheet<Item>? sheet = Singletons.Get<IDataManager>().GetExcelSheet<Item>();

            if (!string.IsNullOrEmpty(input) && sheet is not null)
            {
                List<TriggerData> itemList = new List<TriggerData>();

                // Add by id
                if (uint.TryParse(input, out uint value))
                {
                    if (value > 0)
                    {
                        Item? item = sheet.GetRow(value);
                        if (item is not null)
                        {
                            itemList.Add(new TriggerData(item.Name, item.RowId, item.Icon, 0));
                        }
                    }
                }

                // Add by name
                if (itemList.Count == 0)
                {
                    itemList.AddRange(
                        sheet.Where(item => input.ToLower().Equals(item.Name.ToString().ToLower()))
                            .Select(item => new TriggerData(item.Name, item.RowId, item.Icon, 0)));
                }

                return itemList;
            }

            return new List<TriggerData>();
        }

        public static List<TriggerData> FindActionEntries(string input)
        {
            List<TriggerData> actionList = new List<TriggerData>();

            if (!string.IsNullOrEmpty(input))
            {
                actionList.AddRange(FindEntriesFromActionSheet(input));
                actionList.AddRange(FindEntriesFromActionIndirectionSheet(input));
                actionList.AddRange(FindEntriesFromGeneralActionSheet(input));
            }

            return actionList;
        }

        public static List<TriggerData> FindEntriesFromActionSheet(string input)
        {
            List<TriggerData> actionList = new List<TriggerData>();
            ExcelSheet<LuminaAction>? actionSheet = Singletons.Get<IDataManager>().GetExcelSheet<LuminaAction>();

            if (actionSheet is null)
            {
                return actionList;
            }

            // Add by id
            if (uint.TryParse(input, out uint value))
            {
                if (value > 0)
                {
                    LuminaAction? action = actionSheet.GetRow(value);
                    if (action is not null && (action.IsPlayerAction || action.IsRoleAction))
                    {
                        actionList.Add(new TriggerData(action.Name, action.RowId, action.Icon, action.MaxCharges, GetComboIds(action), action.IsPvP ? CombatType.PvP : CombatType.PvE));
                    }
                }
            }

            // Add by name
            if (!actionList.Any())
            {
                foreach (LuminaAction action in actionSheet)
                {
                    if (input.ToLower().Equals(action.Name.ToString().ToLower()) && (action.IsPlayerAction || action.IsRoleAction))
                    {
                        actionList.Add(new TriggerData(action.Name, action.RowId, action.Icon, action.MaxCharges, GetComboIds(action), action.IsPvP ? CombatType.PvP : CombatType.PvE));
                    }
                }
            }

            return actionList;
        }

        public static List<TriggerData> FindEntriesFromActionIndirectionSheet(string input)
        {
            List<TriggerData> actionList = new List<TriggerData>();
            ExcelSheet<ActionIndirection>? actionIndirectionSheet = Singletons.Get<IDataManager>().GetExcelSheet<ActionIndirection>();

            if (actionIndirectionSheet is null)
            {
                return actionList;
            }

            // Add by id
            if (uint.TryParse(input, out uint value))
            {
                foreach (ActionIndirection iAction in actionIndirectionSheet)
                {
                    LuminaAction? action = iAction.Name.Value;
                    if (action is not null && action.RowId == value)
                    {
                        actionList.Add(new TriggerData(action.Name, action.RowId, action.Icon, action.MaxCharges, GetComboIds(action), action.IsPvP ? CombatType.PvP : CombatType.PvE));
                        break;
                    }
                }
            }

            // Add by name
            if (!actionList.Any())
            {
                foreach (ActionIndirection indirectAction in actionIndirectionSheet)
                {
                    LuminaAction? action = indirectAction.Name.Value;
                    if (action is not null && input.ToLower().Equals(action.Name.ToString().ToLower()))
                    {
                        actionList.Add(new TriggerData(action.Name, action.RowId, action.Icon, action.MaxCharges, GetComboIds(action), action.IsPvP ? CombatType.PvP : CombatType.PvE));
                    }
                }
            }

            return actionList;
        }

        public static List<TriggerData> FindEntriesFromGeneralActionSheet(string input)
        {
            List<TriggerData> actionList = new List<TriggerData>();
            ExcelSheet<GeneralAction>? generalSheet = Singletons.Get<IDataManager>().GetExcelSheet<GeneralAction>();

            if (generalSheet is null)
            {
                return actionList;
            }

            // Add by name (Add by id doesn't really work, these sheets are a mess)
            if (!actionList.Any())
            {
                foreach (GeneralAction generalAction in generalSheet)
                {
                    LuminaAction? action = generalAction.Action.Value;
                    if (action is not null && input.ToLower().Equals(generalAction.Name.ToString().ToLower()))
                    {
                        actionList.Add(new TriggerData(generalAction.Name, action.RowId, (ushort)generalAction.Icon, action.MaxCharges));
                    }
                }
            }

            return actionList;
        }

        private static Dictionary<uint, uint[]> _specialCases = new()
        {
            [88] = [87, 36955], // chaos thrust
            [25772] = [87, 36955], // chaotic spring
            [84] = [78, 36954], // full thrust
            [25771] = [78, 36954], // heavens thrust
            [3556] = [88, 25772], // wheeling thrust
            [36952] = [3554, 3556], // drakesbane
            [3554] = [84, 25771] // fang and claw
        };

        public static uint[] GetComboIds(LuminaAction? action)
        {
            if (action is null)
            {
                return Array.Empty<uint>();
            }

            // can't figure out lumina data, seems incorrect
            // hardcode away...
            if (_specialCases.TryGetValue(action.RowId, out uint[]? ids) && ids != null)
            {
                return ids;
            }

            return GetComboIds(action.ActionCombo.Value?.RowId ?? 0);
        }

        public static uint[] GetComboIds(uint baseComboId)
        {
            if (baseComboId == 0)
            {
                return Array.Empty<uint>();
            }

            List<uint> comboIds = new List<uint>() { baseComboId };
            ExcelSheet<ActionIndirection>? actionIndirectionSheet = Singletons.Get<IDataManager>().GetExcelSheet<ActionIndirection>();

            if (actionIndirectionSheet is null)
            {
                return comboIds.ToArray();
            }

            foreach (ActionIndirection indirectAction in actionIndirectionSheet)
            {
                LuminaAction? upgradedAction = indirectAction.Name.Value;
                LuminaAction? prevAction = indirectAction.PreviousComboAction.Value;
                if (upgradedAction is not null && prevAction is not null && baseComboId == prevAction.RowId)
                {
                    comboIds.Add(upgradedAction.RowId);
                }
            }

            return comboIds.ToArray();
        }

        public static unsafe void GetGCDInfo(out RecastInfo recastInfo, ActionType actionType = ActionType.Action)
        {
            if (!_jobActionIDs.TryGetValue(CharacterState.GetCharacterJob(), out uint actionId))
            {
                recastInfo = new(0, 0, 0);
                return;
            }

            var helper = Singletons.Get<ActionHelpers>();
            helper.GetAdjustedRecastInfo(helper.GetAdjustedActionId(actionId), out recastInfo);
        }
    }
}
