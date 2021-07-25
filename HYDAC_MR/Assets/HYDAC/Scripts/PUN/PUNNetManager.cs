using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class PUNNetManager : MonoBehaviourPunCallbacks
{
    // This is a prefab representing the player. It needs to be in Assets/Resources and have

    // a PhotonView component attached to it. In addition, you will probably want to add Photon Transform

    // View components to the object's children (e.g., the meshes representing the avatar).
    private GameObject spawnedPlayerPrefab;


    // Start is called before the first frame update

    void Start()
    {
        ConnectToServer();
    }


    void ConnectToServer()
    {
        PhotonNetwork.ConnectUsingSettings();

        Debug.Log("Trying to connect to server...");
    }


    public override void OnConnectedToMaster() 
    {
        Debug.Log("Connected to server.");

        base.OnConnectedToMaster();
    }


    public override void OnJoinedRoom()
    {
        Debug.Log("Joined a room.");

        base.OnJoinedRoom();
        spawnedPlayerPrefab = PhotonNetwork.Instantiate("Network Player", transform.position, transform.rotation);
    }


    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        Debug.Log("A new player joined the room.");

        base.OnPlayerEnteredRoom(newPlayer);
    }



    public override void OnLeftRoom()
    {
        base.OnLeftRoom();

        PhotonNetwork.Destroy(spawnedPlayerPrefab);
    }

}