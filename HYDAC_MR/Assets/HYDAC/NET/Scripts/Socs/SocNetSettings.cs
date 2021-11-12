using UnityEngine;
using HYDAC.Scripts.ADD;

namespace HYDAC.Scripts.NET
{
    [CreateAssetMenu(menuName = "Socks/Settings/Network", fileName = "SOC_NetSettings")]
    public class SocNetSettings: ScriptableObject
    {
        [Header("Main Settings")] [Space]
        [SerializeField] private string netAssetsLabel = "NetAssets";
        public string NetAssetsLabel => netAssetsLabel;

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

        [SerializeField] private GameObject roomManagerPrefab;
        public GameObject RoomManagerPrefab => roomManagerPrefab;

        [Space]
        [Tooltip("The player prefab which needs to be added to Photon pool")]
        [SerializeField] private PUNPoolObject playerNetHeadPrefab;
        public PUNPoolObject PlayerNetHeadPrefab => playerNetHeadPrefab;

        [SerializeField] private PUNPoolObject playerNetHandsPrefab;
        public PUNPoolObject PlayerNetHandsPrefab => playerNetHandsPrefab;

        [Space]
        [Tooltip("The prefabs that are networked which needs to be added to Photon pool")]
        [SerializeField] private PUNPoolObject[] intialNetObjects;
        public PUNPoolObject[] IntialNetObjects
        {
            get => intialNetObjects;
            set
            {
                intialNetObjects = value;
            }
        }
        

        [Space]
        [Header("Debug Settings")]
        [SerializeField] private string defaultNetRoomName;
        public string DefaultNetRoomName => defaultNetRoomName;
    }
} 