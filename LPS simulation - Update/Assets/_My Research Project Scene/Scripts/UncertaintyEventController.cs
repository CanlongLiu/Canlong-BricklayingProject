using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using HutongGames.PlayMaker;


// This script hold all uncertainty events.
public class UncertaintyEventController : MonoBehaviour
{


    PhotonView pv;
    PlayMakerFSM fSM;
    PlayMakerFSM fSMChangeValue;
    // Start is called before the first frame update
    void Start()
    {
        fSM = PlayMakerFSM.FindFsmOnGameObject(gameObject, "FSM");
        fSMChangeValue = PlayMakerFSM.FindFsmOnGameObject(gameObject, "ChangeValue");
    }

    // Update is called once per frame
    void Update()
    {
        
    }



    public void TriggerUncertaintyEvent()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            fSM.SendEvent("Trigger Uncertainty");
        }
    }

    public void DeliveryTimeIncrease()
    {
        pv.RPC("AddDeliveryTime", RpcTarget.AllBuffered);
    }

    public void DesignChange()
    {
        pv.RPC("ChangeDesign", RpcTarget.AllBuffered);
    }

    public void ProductionTimeIncrease()
    {
        pv.RPC("AddProductionTime", RpcTarget.AllBuffered);
    }
    public void BagSlotLimitation()
    {
        pv.RPC("BagSlotChanged", RpcTarget.AllBuffered);
    }

    [PunRPC]
    public void AddDeliveryTime()
    {
        fSMChangeValue.SendEvent("AddDeliveryTime");
    }

    [PunRPC]
    public void ChangeDesign()
    {
        fSMChangeValue.SendEvent("ChangeDesign");
    }

    [PunRPC]
    public void AddProductionTime()
    {
        fSMChangeValue.SendEvent("AddProductionTime");
    }

    [PunRPC]
    public void BagSlotChanged()
    {
        fSMChangeValue.SendEvent("BagSlotChanged");
    }

}
