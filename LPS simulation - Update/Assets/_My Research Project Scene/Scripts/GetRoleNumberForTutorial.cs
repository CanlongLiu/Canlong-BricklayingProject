using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.SceneManagement;
using HSVStudio.Tutorial;

public class GetRoleNumberForTutorial : MonoBehaviour
{


    public int RoleNumber;


    // Start is called before the first frame update
    void Start()
    {
        object Role;
        PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue(SingleplayerVRConstants.AVATAR_SELECTION_NUMBER, out Role);
        RoleNumber = (int)Role;
    }
    
    public void checkRole()
    {

    }


    // Update is called once per frame
    void Update()
    {
        
    }

    public void changeScene() 
    {

        SceneManager.LoadScene("Login");

    }
}
