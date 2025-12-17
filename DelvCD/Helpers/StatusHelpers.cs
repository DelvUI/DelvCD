using System;
using System.Collections.Generic;
using System.Linq;
using Dalamud.Game.ClientState.Objects;
using Dalamud.Game.ClientState.Objects.SubKinds;
using Dalamud.Game.ClientState.Objects.Types;
using Dalamud.Plugin.Services;
using Lumina.Excel;

using DalamudStatus = Dalamud.Game.ClientState.Statuses.Status;
using LuminaStatus = Lumina.Excel.Sheets.Status;

namespace DelvCD.Helpers
{
    public class StatusHelpers
    {
        private readonly TriggerSource[] _sourceKeys = Enum.GetValues<TriggerSource>();
        private readonly Dictionary<TriggerSource, Dictionary<uint, List<DalamudStatus>>> _statusMap;

        public StatusHelpers()
        {
            _statusMap = new Dictionary<TriggerSource, Dictionary<uint, List<DalamudStatus>>>(4);
            foreach (var source in _sourceKeys)
            {
                _statusMap.Add(source, new Dictionary<uint, List<DalamudStatus>>(30));
            }
        }

        public List<DalamudStatus> GetStatusList(TriggerSource source, uint statusId)
        {
            var dict = _statusMap[source];
            if (dict.TryGetValue(statusId, out var result))
            {
                return result;
            }

            return new();
        }

        public void GenerateStatusMap()
        {
            foreach (var dict in _statusMap.Values)
            {
                dict.Clear();
            }

            IPlayerCharacter? player = Singletons.Get<IObjectTable>().LocalPlayer;
            if (player is null)
            {
                return;
            }

            foreach (var source in _sourceKeys)
            {
                IGameObject? actor = source switch
                {
                    TriggerSource.Player => player,
                    TriggerSource.Target => Utils.FindTarget(),
                    TriggerSource.TargetOfTarget => Utils.FindTargetOfTarget(),
                    TriggerSource.FocusTarget => Singletons.Get<ITargetManager>().FocusTarget,
                    _ => null
                };

                if (actor is not IBattleChara chara)
                {
                    continue;
                }

                if (_statusMap.ContainsKey(source))
                {
                    foreach (var status in chara.StatusList)
                    {
                        if (!_statusMap[source].ContainsKey(status.StatusId))
                        {
                            _statusMap[source].Add(status.StatusId, new List<DalamudStatus>());
                        }

                        _statusMap[source][status.StatusId].Add(status);
                    }
                }
            }
        }

        public static List<TriggerData> FindStatusEntries(string input)
        {
            ExcelSheet<LuminaStatus>? sheet = Singletons.Get<IDataManager>().GetExcelSheet<LuminaStatus>();
            List<TriggerData> statusList = new List<TriggerData>();

            if (string.IsNullOrEmpty(input) || sheet is null) { return statusList; }

            // Add by id
            if (uint.TryParse(input, out uint value))
            {
                if (value > 0)
                {
                    LuminaStatus? status = sheet.GetRow(value);
                    if (status is not null && status.HasValue)
                    {
                        statusList.Add(new TriggerData(status.Value.Name.ToString(), status.Value.RowId, status.Value.Icon, status.Value.MaxStacks));
                    }
                }
            }

            // Add by name
            if (statusList.Count == 0)
            {
                statusList.AddRange(
                    sheet.Where(status => input.ToLower().Equals(status.Name.ToString().ToLower()))
                        .Select(status => new TriggerData(status.Name.ToString(), status.RowId, status.Icon, status.MaxStacks)));
            }

            return statusList;
        }
    }
}
