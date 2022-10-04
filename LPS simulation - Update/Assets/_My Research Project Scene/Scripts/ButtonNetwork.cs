using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using HutongGames.PlayMaker;

public class ButtonNetwork : MonoBehaviourPun
{

    PhotonView pv;
    PlayMakerFSM GoFsm;

    [SerializeField]
    private string proudceItem;


    // Start is called before the first frame update
    void Start()
    {
        GoFsm = GetComponent<PlayMakerFSM>();
        pv = GetComponent<PhotonView>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

   
  
    public void TellOtherPlayer ()
    {
        pv.RPC("RecieveButtonDown", RpcTarget.AllBuffered);
    }


    [PunRPC]
    public void RecieveButtonDown()
    {
        GoFsm.SendEvent(proudceItem);
    }

}
