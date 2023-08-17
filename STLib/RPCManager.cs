using System;
using System.Linq;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using HarmonyLib;
using JetBrains.Annotations;
using MelonLoader;
using HappyLib.Core;
using HappyLib.DataTypes;
using UnityEngine;
using HappyLib;
using UnhollowerBaseLib;
using static PunTeams;

namespace STLib
{
    public static class RPCManager
    {
        // Эта аналоговнетная система рпц может принимать только строковые аргументы, их может быть несколько,
        // но все они будут скомканы в 1 строчку и переданы в шифрованном виде через рпц чата, W.I.P
        public static Dictionary<Component, string> rpcList;
        public static void RegisterRPC(Component type, string methodName)
        {
            rpcList.Add(type, methodName);
        }
        public static void RPC(string methodName, PhotonTargets photonTargets, string[] parameters)
        {
            string args = string.Join(";", parameters);
            Il2CppSystem.Object[] array = new Il2CppSystem.Object[2];
            array[0] = methodName;
            array[1] = args;
            RoomManager.RoomMultiplayerMenu.photonView.RPC("OHKPKNCACEK", photonTargets, array);
        }
        public static void HandleRPC(string method, string methodArgs)
        {        
            Component type = rpcList.FirstOrDefault(x => x.Value == method).Key;
            type.SendMessage(method, methodArgs);
        }

        public static bool CheckForRPC(string message, string teamName)
        {
            string method = Helper.FromBase64(message);
            foreach (string methodName in rpcList.Values)
            {
                if (methodName == method)
                {
                    HandleRPC(methodName, teamName);
                    return false;
                }
            }
            return true;
        }
    }
}
