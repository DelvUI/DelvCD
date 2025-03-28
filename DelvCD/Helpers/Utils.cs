﻿using System;
using System.Diagnostics;
using System.Numerics;
using System.Runtime.InteropServices;
using Dalamud.Game.ClientState.Objects;
using Dalamud.Game.ClientState.Objects.Enums;
using Dalamud.Game.ClientState.Objects.Types;
using Dalamud.Plugin.Services;

using CharacterStruct = FFXIVClientStructs.FFXIV.Client.Game.Character.Character;

namespace DelvCD.Helpers
{
    public static class Utils
    {
        public static Vector2 GetAnchoredPosition(Vector2 position, Vector2 size, DrawAnchor anchor)
        {
            return anchor switch
            {
                DrawAnchor.Center => position - size / 2f,
                DrawAnchor.Left => position + new Vector2(0, -size.Y / 2f),
                DrawAnchor.Right => position + new Vector2(-size.X, -size.Y / 2f),
                DrawAnchor.Top => position + new Vector2(-size.X / 2f, 0),
                DrawAnchor.TopLeft => position,
                DrawAnchor.TopRight => position + new Vector2(-size.X, 0),
                DrawAnchor.Bottom => position + new Vector2(-size.X / 2f, -size.Y),
                DrawAnchor.BottomLeft => position + new Vector2(0, -size.Y),
                DrawAnchor.BottomRight => position + new Vector2(-size.X, -size.Y),
                _ => position
            };
        }

        public static IGameObject? FindTarget()
        {
            ITargetManager targetManager = Singletons.Get<ITargetManager>();
            return targetManager.SoftTarget ?? targetManager.Target;
        }

        public static IGameObject? FindTargetOfTarget()
        {
            IGameObject? target = FindTarget();
            if (target == null)
            {
                return null;
            }

            IGameObject? player = Singletons.Get<IClientState>().LocalPlayer;
            if (target.TargetObjectId == 0 && player is not null && player.TargetObjectId == 0)
            {
                return player;
            }

            // only the first 200 elements in the array are relevant due to the order in which SE packs data into the array
            // we do a step of 2 because its always an actor followed by its companion
            IObjectTable objectTable = Singletons.Get<IObjectTable>();
            for (int i = 0; i < 200; i += 2)
            {
                IGameObject? actor = objectTable[i];
                if (actor?.GameObjectId == target.TargetObjectId)
                {
                    return actor;
                }
            }

            return null;
        }

        /// <summary>
        /// Checks if a character is considered hostile relative to the player.
        /// </summary>
        /// <param name="character">The <c>Character</c> object to check.</param>
        /// <returns>Returns true if <c>Character</c> provided is hostile, otherwise false.</returns>
        public static unsafe bool IsHostile(ICharacter character)
        {
            CharacterStruct* chara = (CharacterStruct*)character.Address;

            return character != null
                && ((character.SubKind == (byte)BattleNpcSubKind.Enemy || (int)character.SubKind == (byte)BattleNpcSubKind.BattleNpcPart)
                && chara->CharacterData.Battalion > 0); // Since its not super clear, CharacterData.Battalion used for determining friend/enemy state
        }

        public static bool GetResult(
            float dataValue,
            TriggerDataOp op,
            float value)
        {
            return op switch
            {
                TriggerDataOp.Equals => dataValue == value,
                TriggerDataOp.NotEquals => dataValue != value,
                TriggerDataOp.LessThan => dataValue < value,
                TriggerDataOp.GreaterThan => dataValue > value,
                TriggerDataOp.LessThanEq => dataValue <= value,
                TriggerDataOp.GreaterThanEq => dataValue >= value,
                _ => false
            };
        }

        public static string GetTagsTooltip()
        {
            return $"Append the characters ':k' to a numeric tag to kilo-format it.\n" +
                    "Append the characters ':t' to a numeric tag to time-format it.\n" +
                    "Append a '.' and a number to limit the number of characters,\n" +
                    "or the number of decimals when used with numeric values.\n\nExamples:\n" +
                    "[value]          =>    1,234\n" +
                    "[value:k]      =>           1k\n" +
                    "[value:t]       =>     20:34\n" +
                    "[value:k.1]  =>       1.2k\n\n" +
                    "[name]                   =>    Firstname Lastname\n" +
                    "[name:upper]       =>    FIRSTNAME LASTNAME\n" +
                    "[name:lower]       =>    firstname lastname\n" +
                    "[name_first.5]    =>    First\n" +
                    "[name_last.1]     =>    L";
        }

        public static void OpenUrl(string url)
        {
            try
            {
                Process.Start(url);
            }
            catch
            {
                try
                {
                    // hack because of this: https://github.com/dotnet/corefx/issues/10361
                    if (RuntimeInformation.IsOSPlatform(osPlatform: OSPlatform.Windows))
                    {
                        url = url.Replace("&", "^&");
                        Process.Start(new ProcessStartInfo("cmd", $"/c start {url}") { CreateNoWindow = true });
                    }
                    else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                    {
                        Process.Start("xdg-open", url);
                    }
                }
                catch (Exception e)
                {
                    Singletons.Get<IPluginLog>().Error("Error trying to open url: " + e.Message);
                }
            }
        }
    }
}
