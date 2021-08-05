using UnityEngine;
using UnityEngine.Serialization;

namespace HYDAC.SOCS.NET
{
    [CreateAssetMenu(menuName = "NetSocs/NetSettings", fileName = "SOC_NetSettings")]
    public class SocNetSettings: ScriptableObject
    {
        [SerializeField] private string gameVersion = "1"; // Set to 1 by default, unless we need to make breaking changes on a project that is Live.
        public string GameVersion => gameVersion;
        
        [SerializeField] private string networkSceneName = "AssemblyView - HyBox";
        public string NetworkSceneName => networkSceneName;
        
        [Header("Room Options")]
        [Tooltip("The maximum number of players per room. When a room is full, it can't be joined by new players, and so new room will be created. MAX 20 users")]
        [SerializeField] private byte maxPlayersPerRoom = 4;
        public byte MaxPlayersPerRoom => maxPlayersPerRoom;

        
        [SerializeField] private bool isRoomVisible = true;
        public bool IsRoomVisible => isRoomVisible;

        
        [SerializeField] private bool isRoomOpen = true;
        public bool IsRoomOpen => isRoomOpen;


        [Space] 
        [Tooltip("The prefab to use for representing the player")]
        [SerializeField] private GameObject localPlayerPrefab;
        public GameObject LocalPlayerPrefab => localPlayerPrefab;

        [SerializeField] private GameObject focusedModuleHolderPrefab;
        public GameObject FocusedModuleHolderPrefab => focusedModuleHolderPrefab;
    }
}