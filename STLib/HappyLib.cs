using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using HarmonyLib;
using JetBrains.Annotations;
using MelonLoader;
using HappyLib.Core;
using HappyLib.DataTypes;
using UnityEngine;

[assembly: MelonInfo(typeof(HappyLib.ModLoader), "ST Mod Library", "1.0", "Zero & Vantablack")]
[assembly: MelonGame("ZeoWorks", "Slendytubbies 3")]

namespace HappyLib
{
    namespace DataTypes
    {
        public struct CustomProperty
        {
            public string Name;
            public object Value;
        }

        public class Player : IDisposable
        {
            /*public class Controller //я насрал, пожалуй следует убрать за собой.....
            {
                private FPScontroller m_fpsController;
                private FPSMouseLook m_fpsMouseLook_h;
                private FPSMouseLook m_fpsMouseLook_v;
                public Controller()
                {

                }
            }*/
            #region FIELDS
            private bool m_local;
            private CharacterController m_characterController;
            private FPScontroller m_fpsController;
            private FPSMouseLook m_fpsMouseLook_h;
            private FPSMouseLook m_fpsMouseLook_v;
            public GameObject m_playerObject;
            private PlayerDamage m_playerDamage;      
            private PlayerNetworkController m_playerNetworkController;            
            private WeaponManager m_weaponManager;
            private SyncCustomization m_syncCustomization;
            private DrawPlayerName m_drawPlayerName;

            #endregion

            #region PROPERTIES
            public CharacterController CharacterController
            {
                get
                {
                    m_characterController ??= m_playerObject.GetComponent<CharacterController>();
                    if (m_characterController == null)
                        MelonLogger.Error($"Couldn't find {nameof(CharacterController)}");
                    return m_characterController;
                }

                internal set => m_characterController = value;
            }
            public bool IsLocal => CharacterController != null;

            public PlayerDamage PlayerDamage
            {
                get
                {
                    m_playerDamage ??= m_playerObject.GetComponent<PlayerDamage>();
                    if (m_playerDamage == null)
                        MelonLogger.Error($"Couldn't find {nameof(PlayerDamage)}");
                    return m_playerDamage;
                }

                internal set => m_playerDamage = value;
            }

            public PlayerNetworkController PlayerNetworkController
            {
                get
                {
                    if (PhotonNetwork.isOfflineMode)
                        MelonLogger.Error("The PlayerNetworkController component is not available in offline mode");
                    m_playerNetworkController ??= m_playerNetworkController.GetComponent<PlayerNetworkController>();
                    if (m_playerNetworkController == null)
                        MelonLogger.Error($"Couldn't find {nameof(PlayerNetworkController)}");
                    return m_playerNetworkController;
                }

                internal set => m_playerNetworkController = value;
            }

            public SyncCustomization SyncCustomization
            {
                get
                {
                    m_syncCustomization ??= m_playerObject.GetComponent<SyncCustomization>();
                    if (m_syncCustomization == null)
                        MelonLogger.Error($"Couldn't find {nameof(SyncCustomization)}");
                    return m_syncCustomization;
                }

                internal set => m_syncCustomization = value;
            }

            public DrawPlayerName DrawPlayerName
            {
                get
                {
                    if (IsLocal)
                        MelonLogger.Warning("Warning! The local player does not contain the DrawPlayerName component by default.");
                    m_drawPlayerName ??= m_playerObject.GetComponent<DrawPlayerName>();
                    if (m_drawPlayerName == null)
                        MelonLogger.Error($"Couldn't find {nameof(DrawPlayerName)}");
                    return m_drawPlayerName;
                }

                internal set => m_drawPlayerName = value;
            }

            #region LOCAL ONLY

            public FPScontroller FPSController
            {
                get
                {
                    if (!IsLocal)
                        MelonLogger.Warning("Warning! The remote player does not contain the FPS components by default.");
                    m_fpsController ??= m_playerObject.GetComponent<FPScontroller>();
                    if (m_fpsController == null)
                        MelonLogger.Error($"Couldn't find {nameof(FPScontroller)}");
                    return m_fpsController;
                }

                internal set => m_fpsController = value;
            }

            #endregion

            public Component[] Components => m_playerObject != null ? Helper.GetObjectComponents(m_playerObject) : null;
            //бля, ради прикола щас попробовал юзать это, ахахах, в общем я нахуй не буду писать мануал к этой либе

            public float Hp { get; set; }
            public float MaxHp { get; set; }

            #endregion

            #region PUBLIC_METHODS

            public void Kill()
            {
                DoDamage(float.PositiveInfinity);
            }

            public void Respawn()
            {
                
            }

            public void DoDamage(float damage)
            {
                
            }
            
            
            //public void GiveWeapon(...)
            //public void RemoveWeapon(...)

            #endregion

            #region STATIC_METHODS

            //нам не надо, чтобы Player просто создавали через new, вместо этого инициализировать будем через объекты
            public static Player Find(GameObject go)
            {
                if (go == null)
                {                  
                    MelonLogger.Error("Argument is null.");
                    return null;
                }

                return new Player(go);
            }
            
            public static Player Find(PhotonView view)
            {
                if (view == null)
                {
                    MelonLogger.Error("Argument is null.");
                    return null;
                }
                return new Player(view);
            }

            //предстоить протестить
            public static Player Find(string playerName)
            {
                GameObject player = GameObject.Find(playerName);
                if (player == null)
                {
                    MelonLogger.Error("Argument is null.");
                    return null;
                }
                return new Player(player);
            }

            public static Player Find(PhotonPlayer player)
            {
                if (player == null)
                {
                    MelonLogger.Error("Argument is null.");
                    return null;
                }
                return Find(player.NickName);
            }

            public void Dispose()
            {
                throw new NotImplementedException();
            }

            #endregion

            #region CTORS

            private Player(GameObject go)
            {
                //великолепный способ, просто ахуенный, как я понял, надежный блять как швейцарские часы
                bool isPlayer = go.GetComponent<PlayerDamage>() != null;

                if (!isPlayer)
                {
                    MelonLogger.Error($"Provided GameObject {go.name} is not a Player instance.");
                    return;
                }
                m_playerObject = go;
            }

            private Player(PhotonView view)
            {
                bool isPlayer = view.gameObject.GetComponent<PlayerDamage>() != null;

                if (!isPlayer)
                {
                    MelonLogger.Error($"Provided GameObject {view.name} is not a Player instance.");
                    return;
                }
                m_playerObject = view.gameObject;
            }
            //я гей, но эту строчку никто не увидит

            /*private Player(PhotonPlayer player) пока не стал делать, потому что поиск по имени плохо оптимизирован
            {
                
            }*/

            #endregion

        }
    }
    
    namespace Core
    {
        public static class RoomManager
        {
            #region FIELDS

            private static RoomMultiplayerMenu m_rmm;
            private static MultiplayerChat m_multiplayerChat;
            private static WhoKilledWho m_whoKilledWho;
            private static SurvivalMechanics m_survivalMechanics;
            private static ClassicMechanics m_classicMechanics;
            private static AudioSource m_roomAudioSource;
            private static Volume m_console;

            private static Player m_localPlayer;
            
            #endregion

            #region PROPERTIES

           // public static Component[] RoomComponents { get; } в данный момент юзлесс
            public static RoomMultiplayerMenu RoomMultiplayerMenu
            {
                get
                {
                    m_rmm ??= GameObject.FindObjectOfType<RoomMultiplayerMenu>();
                    if(m_rmm == null)
                        MelonLogger.Error($"Couldn't find {nameof(RoomMultiplayerMenu)}");
                    return m_rmm;
                }

                internal set => m_rmm = value;
            }
            
            public static MultiplayerChat MultiplayerChat
            {
                get
                {
                    m_multiplayerChat ??= GameObject.FindObjectOfType<MultiplayerChat>();
                    if(m_multiplayerChat == null)
                        MelonLogger.Error($"Couldn't find {nameof(MultiplayerChat)}");
                    return m_multiplayerChat;
                }

                internal set => m_multiplayerChat = value;
            }

            public static WhoKilledWho WhoKilledWho
            {
                get
                {
                    m_whoKilledWho ??= GameObject.FindObjectOfType<WhoKilledWho>();
                    if (m_whoKilledWho is null)
                        MelonLogger.Error($"Couldn't find {nameof(WhoKilledWho)}");
                    return m_whoKilledWho;
                }

                internal set => m_whoKilledWho = value;
            }

            public static Player LocalPlayer
            {
                get
                {
                    if (m_localPlayer == null)
                        GetLocalPlayer();
                    if (m_localPlayer == null)
                        MelonLogger.Error($"Couldn't find {nameof(LocalPlayer)}");
                    return m_localPlayer;
                }
            }

            public static SurvivalMechanics SurvivalMechanics
            {
                get
                {
                    m_survivalMechanics ??= GameObject.FindObjectOfType<SurvivalMechanics>();
                    if (m_survivalMechanics is null)
                        MelonLogger.Error($"Couldn't find {nameof(SurvivalMechanics)}");
                    return m_survivalMechanics;
                }

                internal set => m_survivalMechanics = value;
            }

            public static ClassicMechanics ClassicMechanics
            {
                get
                {
                    m_classicMechanics ??= GameObject.FindObjectOfType<ClassicMechanics>();
                    if (m_classicMechanics is null)
                        MelonLogger.Error($"Couldn't find {nameof(ClassicMechanics)}");
                    return m_classicMechanics;
                }

                internal set => m_classicMechanics = value;
            }

            public static Volume SandboxConsole
            {
                get
                {
                    if (GameMode != "SBX")
                        MelonLogger.Warning($"The current gamemode ({GameMode}) does not include a sandbox console by default.");
                    m_console ??= GameObject.FindObjectOfType<Volume>();
                    if (m_console is null)
                        MelonLogger.Error($"Couldn't find {nameof(Volume)}");
                    return m_console;
                }

                internal set => m_console = value;
            }

            public static string GameMode => RoomMultiplayerMenu.KCIGKBNBPNN;
            public static Player AllPlayers { get; }

            public static Player AlivePlayers { get; }

            public static Player ConnectedPlayers { get; }
            public static bool Joined => RoomMultiplayerMenu.CNLHJAICIBH != null;

            #endregion

            #region METHODS

            public static Player GetLocalPlayer()
            {
               // GameObject.Instantiate(GameObject.Find("CreditButton")).transform.SetParent(GameObject.Find("PCButtons").transform, false);
                // if (m_localPlayer)
                GameObject player = GameObject.FindWithTag("Player");
                if (player == null)
                {
                    MelonLogger.Error("Argument is null.");
                    return null;
                }
                return Player.Find(player);
            }

            #endregion
            static RoomManager()
            {

            }           
        }

        public static class Events
        {
            public delegate void LoadingCompleted();
            public static event LoadingCompleted LoadingCompletedEvent;

            public delegate void ChatMessage(string senderName, string message, string teamName, MultiplayerChat instance);
            public static event ChatMessage ChatMessageEvent;

            public delegate void NetworkMessage(string killer, string killed, string middleText, string teamName, WhoKilledWho instance);
            public static event NetworkMessage NetworkMessageEvent;

            //не работает upd работает upd2 БЛЯТЬ СНОВА НЕ РАБОТАЕТ ЧО ЗА ХУЙНЯ ЗЕРО UPD3 все, пофиксил, я еблан
            internal static void OnLoadingCompleted()
            {
                MelonLogger.Msg("Invoking OnLoadingCompleted");
                LoadingCompletedEvent?.Invoke();
            }
            internal static void OnChatMessage(string senderName, string message, string teamName, MultiplayerChat instance)
            {
                MelonLogger.Msg("Invoking OnChatMessage");
                ChatMessageEvent?.Invoke(senderName, message, teamName, instance);
            }
            internal static void OnNetworkMessage(string killer, string killed, string middleText, string teamName, WhoKilledWho instance)
            {
                MelonLogger.Msg("Invoking OnNetworkMessage");
                NetworkMessageEvent?.Invoke(killer, killed, middleText, teamName, instance);
            }
        }

        public static class AudioManager
        {
            //предлагаю сделать систему, которая позволила бы проигрывать разные звуки, путём регистрации их в этом списке
            //предлагаю нахуяриться димедролом ведь у меня снова нихуя не получилось, я потратил 2 часа на ахуенную аудио систему
            //и оно блять просто вылетает нахуй без ошибок, вернул все как было, эта ебаная магия меня уже заебала

            #region FIELDS

            private static Transform m_sourcesRoot; //ради удобства храним сурсы здесь
            
            private static readonly List<AudioSource> m_audioSources = new List<AudioSource>();

            #endregion

            #region PROPERTIES

            public static List<AudioSource> RegisteredAudioSources => m_audioSources;

            #endregion

            public static void SetRootPersistance()
            {
                if (m_sourcesRoot == null)
                    MelonLogger.Error("There's no Sources Root to set persistance.");
                GameObject.DontDestroyOnLoad(m_sourcesRoot);    
            }
            
            public static AudioSource RegisterAudioSource(string name)
            {
                if (m_sourcesRoot == null)
                    m_sourcesRoot = new GameObject("[AUDIO SOURCES ROOT]").transform;

                GameObject sourceObject = new GameObject($"[SOURCE] {name}");
                sourceObject.transform.SetParent(m_sourcesRoot);

                var audioSource = sourceObject.AddComponent<AudioSource>();
                m_audioSources.Add(audioSource);
                
                return audioSource;
            }

            public static void UnregisterAudioSource(AudioSource source)
            {
                if (m_sourcesRoot == null || m_audioSources == null || m_audioSources.Count < 1)
                    MelonLogger.Error("There's no audiosources to unregister.");

                m_audioSources.Remove(source);
            }

            public static void UnregisterAll()
            {
                GameObject.Destroy(m_sourcesRoot);
                m_audioSources.Clear();
            }
        }
    }
  
    //служебный класс, чтобы получить все компоненты из любого объекта
    public static class Helper
    {
        public static Component[] GetObjectComponents(GameObject gameObject)
        {
            return gameObject.GetComponents<Component>();
        }
        public static string ToBase64(string plainText)
        {
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
            return System.Convert.ToBase64String(plainTextBytes);
        }
        public static string FromBase64(string base64EncodedData)
        {
            var base64EncodedBytes = System.Convert.FromBase64String(base64EncodedData);
            return System.Text.Encoding.UTF8.GetString(base64EncodedBytes);
        }
        private static string GetFunctionName(Action method)
        {
            return method.Method.Name;
        }
    }

    public struct LibInfo
    {
        public string Name;
        public string Version;
        public string GameVersion;
        public string Author;
        public string Description;
    }
    
    public static class HappyLib
    {
        public static LibInfo LibInfo;

        static HappyLib()
        {
            LibInfo = new LibInfo()
            {
                Name = "HappyLib",
                Version = "v1.0",
                Author = "Zero & Vantablack",
                Description = "Simple library for S3 modding",
                GameVersion = "S3 v2.48"
            };
        }
    }

    internal sealed class ModLoader : MelonMod
    {

    }
    
}