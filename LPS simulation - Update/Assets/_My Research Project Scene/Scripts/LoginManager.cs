using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using TMPro;
using VRUiKits.Utils;
using UnityEngine.SceneManagement;
public class LoginManager : MonoBehaviourPunCallbacks
{
    public UIKitInputField PlayerName_InputName;


    #region Unity Methods
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
    #endregion

    #region UI Callback Methods
    public void ConnectAnonymously()
    {
        PhotonNetwork.ConnectUsingSettings();
    }

    public void GoToLogin()
    {
        SceneManager.LoadScene("Login");
    }
  
    public void ConnectWithOffLineModeSupplier()
    {
        PhotonNetwork.NickName = PlayerName_InputName.text;
        PhotonNetwork.OfflineMode = true;
        PhotonNetwork.CreateRoom("Tutorial");
        ExitGames.Client.Photon.Hashtable playerSelectionProp = new ExitGames.Client.Photon.Hashtable() { { SingleplayerVRConstants.AVATAR_SELECTION_NUMBER, 0 } };
        PhotonNetwork.LocalPlayer.SetCustomProperties(playerSelectionProp);
        SceneManager.LoadScene("Experiment Tutorial");

    }

    public void ConnectWithOffLineModeSubcontractor()
    {
        PhotonNetwork.NickName = PlayerName_InputName.text;
        PhotonNetwork.OfflineMode = true;
        PhotonNetwork.CreateRoom("Tutorial");
        ExitGames.Client.Photon.Hashtable playerSelectionProp = new ExitGames.Client.Photon.Hashtable() { { SingleplayerVRConstants.AVATAR_SELECTION_NUMBER, 1 } };
        PhotonNetwork.LocalPlayer.SetCustomProperties(playerSelectionProp);
        SceneManager.LoadScene("Experiment Tutorial");

    }

    public void ConnectWithOffLineModeManager()
    {
        PhotonNetwork.NickName = PlayerName_InputName.text;
        PhotonNetwork.OfflineMode = true;
        PhotonNetwork.CreateRoom("Tutorial");
        ExitGames.Client.Photon.Hashtable playerSelectionProp = new ExitGames.Client.Photon.Hashtable() { { SingleplayerVRConstants.AVATAR_SELECTION_NUMBER, 2 } };
        PhotonNetwork.LocalPlayer.SetCustomProperties(playerSelectionProp);
        SceneManager.LoadScene("Experiment Tutorial");

    }

    public void ConnectToPhotonServer()
    {
        if (PlayerName_InputName != null)
        {
            PhotonNetwork.NickName = PlayerName_InputName.text;
            PhotonNetwork.ConnectUsingSettings();
        }
    }
    #endregion


    #region Photon Callback Methods
    public override void OnConnected()
    {
        Debug.Log("OnConnected is called. The server is available!");
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("Connected to Master Server with player name: " + PhotonNetwork.NickName);
        PhotonNetwork.LoadLevel("RoleSelection");
    }


    #endregion
}
