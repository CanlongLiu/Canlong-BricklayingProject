using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GVRI;
using HutongGames.PlayMaker;

public class SlotChanged : MonoBehaviour
{
    public Slot slot;
    public PlayMakerFSM fSM;
    public bool BagSlot;
    

    //tell the FSM that the slot has changed.
    public void CountChanged(CoreSlot s)
    {
        if (BagSlot)
        {
            fSM.SendEvent("BagSlotChanged");
        }
        else
        {
            fSM.SendEvent("SlotChanged");

        }
    }



    // Start is called before the first frame update
    void Start()
    {
        if (!slot) slot = gameObject.GetComponent<Slot>();
        if (!slot) Debug.LogWarning("No InventorySlot could be found to display the Count");
        else
        {
            slot.CoreSlot.subscribers.Add(CountChanged);
            CountChanged(slot.CoreSlot); //first update
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
