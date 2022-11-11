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
    PlayMakerFSM fSMSendNotification;
    object avatarSelectionNumber;
    // Start is called before the first frame update
    void Start()
    {
        fSM = PlayMakerFSM.FindFsmOnGameObject(gameObject, "FSM");
        fSMChangeValue = PlayMakerFSM.FindFsmOnGameObject(gameObject, "ChangeValue");
        fSMSendNotification = PlayMakerFSM.FindFsmOnGameObject(gameObject, "SendNotification");
        pv = GetComponent<PhotonView>();
        PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue(MultiplayerVRConstants.AVATAR_SELECTION_NUMBER, out avatarSelectionNumber);
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
    public void AddDeliveryTime()//specify to Supplier
    {
        fSMChangeValue.SendEvent("AddDeliveryTime");
        
        if ((int)avatarSelectionNumber == 0)
        {
            fSMSendNotification.SendEvent("NotifyDeliveryDelay");
        }

    }

    [PunRPC]
    public void ChangeDesign()//specify to Manager
    {
        fSMChangeValue.SendEvent("ChangeDesign");

        if ((int)avatarSelectionNumber == 2)
        {
            fSMSendNotification.SendEvent("NotifyDesignChange");
        }
           
    }

    [PunRPC]
    public void AddProductionTime()//specify to Supplier
    {
        fSMChangeValue.SendEvent("AddProductionTime");
        if ((int)avatarSelectionNumber == 0)
        {
            fSMSendNotification.SendEvent("NotifyProductionDelay");
        }
        
    }

    [PunRPC]
    public void BagSlotChanged()//specify to Subcontractor
    {
        fSMChangeValue.SendEvent("ReduceSlotCapacity");
        if ((int)avatarSelectionNumber == 1)
        {
            fSMSendNotification.SendEvent("NotifyLowProductivity");
        }
   
    }

}
