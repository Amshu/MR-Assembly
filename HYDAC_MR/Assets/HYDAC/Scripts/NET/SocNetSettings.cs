using UnityEngine;
using UnityEngine.AddressableAssets;

namespace HYDAC.Scripts.NET
{
    [CreateAssetMenu(menuName = "Socks/Settings/Network", fileName = "SOC_NetSettings")]
    public class SocNetSettings: ScriptableObject
    {
        [Header("Main Settings")] [Space]
        [SerializeField] private string gameVersion = "1"; // Set to 1 by default, unless we need to make breaking changes on a project that is Live.
        public string GameVersion => gameVersion;
        
        [Header("Network Settings")] [Space]
        [Tooltip("The maximum number of players per room. When a room is full, it can't be joined by new players, and so new room will be created. MAX 20 users")]
        [SerializeField] private byte maxPlayersPerRoom = 4;
        public byte MaxPlayersPerRoom => maxPlayersPerRoom;

        [SerializeField] private bool isRoomVisible = true;
        public bool IsRoomVisible => isRoomVisible;

        [SerializeField] private bool isRoomOpen = true;
        public bool IsRoomOpen => isRoomOpen;

        [Header("Debug Settings")] [Space]
        [SerializeField] private string defaultNetRoomName;
        public string DefaultNetRoomName => defaultNetRoomName;

        [Space]
        [Tooltip("The prefab to use for representing the player")]
        [SerializeField] private AssetReference[] networkObjects;
        public AssetReference[] NetworkObjects => networkObjects;
    }
} 