using UnityEngine;
using UnityEngine.AddressableAssets;

namespace HYDAC.Scripts.SOCS
{
    [CreateAssetMenu(menuName = "Socks/Net/Settings", fileName = "SOC_NetSettings")]
    public class SocMainSettings: ScriptableObject
    {
        [Header("Main Settings")] [Space]
        [SerializeField] private string gameVersion = "1"; // Set to 1 by default, unless we need to make breaking changes on a project that is Live.
        public string GameVersion => gameVersion;
        
        [SerializeField] private AssetReference menuSceneRef;
        public AssetReference MenuSceneRef => menuSceneRef;

        [SerializeField] private AssetReference networkSceneRef;
        public AssetReference NetworkSceneRef => networkSceneRef;
        
        
        [Header("Network Settings")] [Space]
        [Tooltip("The maximum number of players per room. When a room is full, it can't be joined by new players, and so new room will be created. MAX 20 users")]
        [SerializeField] private byte maxPlayersPerRoom = 4;
        public byte MaxPlayersPerRoom => maxPlayersPerRoom;

        [SerializeField] private bool isRoomVisible = true;
        public bool IsRoomVisible => isRoomVisible;

        [SerializeField] private bool isRoomOpen = true;
        public bool IsRoomOpen => isRoomOpen;


        [Space] 
        [Tooltip("The prefab to use for representing the player")]
        [SerializeField] private AssetReference localPlayerPrefab;
        public AssetReference LocalPlayerPrefab => localPlayerPrefab;

        [Header("Debug Settings")] [Space]
        [SerializeField] private string defaultNetRoomName;
        public string DefaultNetRoomName => defaultNetRoomName;
    }
} 