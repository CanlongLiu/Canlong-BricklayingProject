using GVRI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Photon.Pun;

public class Container : MonoBehaviour
{
    // contains all the slots you are interested in. In this case set before start
    public List<Slot> slots;
    public string SiteTitle;
    public TextMeshProUGUI Site;
    public string BillboardTitle;
    public TextMeshProUGUI Billboard;
    public string ZoneCape;
    object avatarSelectionNumber;
    int avatarNumber;
    public bool LPSMode = false;





    // Start is called before the first frame update
    void Start()
    {
        foreach(Slot s in slots)
        {
            // subscribe to slots, so that OnChange is being called whenever one of them changes
            s.CoreSlot.subscribers.Add(OnChange);
        }
        PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue(MultiplayerVRConstants.AVATAR_SELECTION_NUMBER, out avatarSelectionNumber);
        avatarNumber = (int)avatarSelectionNumber;

    }

    private void OnChange(CoreSlot slot_that_changed)
    {
        // collect counts for each item info
        Dictionary<ItemInfo, int> counts = new Dictionary<ItemInfo, int>();
        foreach (Slot s in slots)
        {
            CoreSlot cs = s.CoreSlot;
            ItemInfo ii = cs.ItemInfo;
            int ic = cs.ItemCount;

            if (cs.ItemInfo == null) continue; // ignore if empty

            int current_count;
            if (counts.TryGetValue(ii, out current_count))
            {
                // there already is a current count for the item info
                // update it by adding the item count of the slot to it
                counts[ii] = current_count + ic;
            }
            else
            {
                // this is the first iteminfo of that type
                // add it to the dictionary
                counts.Add(ii, ic);
            }

        }
        // construct an output string to log to the console
        string ouput_string = "";
        int total = 0;
        foreach (KeyValuePair<ItemInfo, int> pair in counts)
        {
            ouput_string += pair.Key.name + ": " + pair.Value + "; ";
            total += pair.Value;
        }
        Debug.Log(ouput_string + "Total item count: " + total + "; }");
        if (avatarNumber == 2 || LPSMode)
        {
            Site.text = SiteTitle + " : " + ouput_string + "Total bricks: " + total + " / " + ZoneCape;
        }
        Billboard.text = BillboardTitle + " : " + ouput_string + "Total bricks: " + total + " / " + ZoneCape;
    }

}
