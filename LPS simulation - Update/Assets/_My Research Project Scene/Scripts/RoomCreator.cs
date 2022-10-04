using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
public class RoomCreator : MonoBehaviourPunCallbacks
{
    private string mapType;


    // Start is called before the first frame update
    void Start()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
        PhotonNetwork.UseRpcMonoBehaviourCache = true; //For better performance
        PhotonNetwork.OfflineMode = false; //true would "fake" an online connection
    

        if (!PhotonNetwork.IsConnectedAndReady)
        {
            PhotonNetwork.ConnectUsingSettings();
        }
        else
        {
            PhotonNetwork.JoinLobby();
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    #region UI Callback Methods
    public void JoinRandomRoom()
    {
        PhotonNetwork.JoinRandomRoom();
    }

    public void OnEnterButtonCicked_Tutorial()
    {
        mapType = MultiplayerVRConstants.MAP_TYPE_VALUE_TUTORIAL;
        ExitGames.Client.Photon.Hashtable expectedCustomRoomProperties = new ExitGames.Client.Photon.Hashtable() { { MultiplayerVRConstants.MAP_TYPE_KEY, mapType } };
        PhotonNetwork.JoinRandomRoom(expectedCustomRoomProperties, 0);
    }

    public void OnEnterButtonCicked_Experiment()
    {
        mapType = MultiplayerVRConstants.MAP_TYPE_VALUE_EXPERIMENT;
        ExitGames.Client.Photon.Hashtable expectedCustomRoomProperties = new ExitGames.Client.Photon.Hashtable() { { MultiplayerVRConstants.MAP_TYPE_KEY, mapType } };
        PhotonNetwork.JoinRandomRoom(expectedCustomRoomProperties, 0);
    }

    public void OnEnterButtonCicked_ExperimentTraditional()
    {
        mapType = MultiplayerVRConstants.MAP_TYPE_VALUE_EXPERIMENTTRADITIONAL;
        ExitGames.Client.Photon.Hashtable expectedCustomRoomProperties = new ExitGames.Client.Photon.Hashtable() { { MultiplayerVRConstants.MAP_TYPE_KEY, mapType } };
        PhotonNetwork.JoinRandomRoom(expectedCustomRoomProperties, 0);
    }



    #endregion

    #region Photon Callback Methods
    public override void OnCreatedRoom()
    {
        Debug.Log("A room is created with the name: " + PhotonNetwork.CurrentRoom.Name);
    }
    public override void OnConnectedToMaster()
    {
        Debug.Log("Connected to servers again.");
        PhotonNetwork.JoinLobby();
    }
    public override void OnJoinedRoom()
    {
        Debug.Log("The Local player: " + PhotonNetwork.NickName + " joined to " + PhotonNetwork.CurrentRoom.Name + " Player count " + PhotonNetwork.CurrentRoom.PlayerCount);
        if (PhotonNetwork.CurrentRoom.CustomProperties.ContainsKey(MultiplayerVRConstants.MAP_TYPE_KEY))
        {
            object mapType;
            if (PhotonNetwork.CurrentRoom.CustomProperties.TryGetValue(MultiplayerVRConstants.MAP_TYPE_KEY, out mapType))
            {
                Debug.Log("Joined room with the map: " + (string)mapType);
                
                if ((string)mapType == MultiplayerVRConstants.MAP_TYPE_VALUE_TUTORIAL)
                {
                    //Load the Tutorial scene
                    PhotonNetwork.LoadLevel("Tutorial");

                }


                else if ((string)mapType == MultiplayerVRConstants.MAP_TYPE_VALUE_EXPERIMENT)
                {
                    //Load the Experiment scene
                    PhotonNetwork.LoadLevel("Experiment");


                }
                else if ((string)mapType == MultiplayerVRConstants.MAP_TYPE_VALUE_EXPERIMENTTRADITIONAL)
                {
                    //Load the Experiment scene
                    PhotonNetwork.LoadLevel("Experiment Traditional");


                }

        }   }   
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        Debug.Log(newPlayer.NickName + " joined to: " + "Player count: " + PhotonNetwork.CurrentRoom.PlayerCount);
    }
    


    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.Log(message);
        CreateAndJoinRoom();
    }
    public override void OnJoinedLobby()
    {
        Debug.Log("Joined the Lobby.");
    }

    #endregion


    #region Private Methods
    private void CreateAndJoinRoom()
    {

        string randomRoomName = "Room_" + mapType + Random.Range(0,10000);
        RoomOptions roomOptions = new RoomOptions();
        roomOptions.MaxPlayers = 20;

        string[] roomPropsInLobby = { MultiplayerVRConstants.MAP_TYPE_KEY };
        //We have 2 different maps
        //1. Tutorial = "tutorial"
        //2. Experiment = "experiment"
        //3. Traditional = "traditional"

        ExitGames.Client.Photon.Hashtable customRoomProperties = new ExitGames.Client.Photon.Hashtable() { { MultiplayerVRConstants.MAP_TYPE_KEY, mapType } };

        roomOptions.CustomRoomPropertiesForLobby = roomPropsInLobby;
        roomOptions.CustomRoomProperties = customRoomProperties;




        PhotonNetwork.CreateRoom(randomRoomName, roomOptions);

    }


    #endregion
}
