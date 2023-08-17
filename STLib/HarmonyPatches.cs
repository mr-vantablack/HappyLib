using HappyLib.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;
using HarmonyLib;
using JetBrains.Annotations;
using MelonLoader;
using HappyLib.Core;
using HappyLib.DataTypes;
using UnityEngine;

namespace Patches
{
    [HarmonyPatch(typeof(RoomMultiplayerMenu), "Start")]
    internal static class OnLoadingCompletedPatch
    {
        [HarmonyPostfix]
        internal static void OnRoomInit()
        {
            Events.OnLoadingCompleted();
        }
    }
    [HarmonyPatch(typeof(MultiplayerChat), "HLIDELGJEON")]
    internal static class OnChatMessagePatch
    {
        [HarmonyPrefix]
        internal static void OnChatMessage(ref string NAEFFPHCJKL, ref string NOFLIGCKLDF, ref string PBPBALMOMEM, MultiplayerChat __instance)
        {
            Events.OnChatMessage(NAEFFPHCJKL, NOFLIGCKLDF, PBPBALMOMEM, __instance);
        }
    }
    [HarmonyPatch(typeof(WhoKilledWho), "networkAddMessage")]
    internal static class OnNetworkMessagePatch
    {
        [HarmonyPrefix]
        internal static void OnNetworkMessage(ref string killer, ref string killed, ref string middleText, ref string teamName, WhoKilledWho __instance)
        {
            Events.OnNetworkMessage(killer, killed, middleText, teamName, __instance);
        }
    }
}
