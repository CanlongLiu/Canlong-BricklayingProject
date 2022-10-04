using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class GameMaster : MonoBehaviour
{
    public ultimate_timer ultimate_timer_script1;
    public ultimate_timer ultimate_timer_script2;
    public float increaseTimeInput;
    PhotonView pv;

    // Start is called before the first frame update
    void Start()
    {
        
        pv = GetComponent<PhotonView>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void StartGame()
    {
     
        pv.RPC("StartGameNetwork", RpcTarget.AllBuffered);
    }

    public void PauseGame()
    {
        ultimate_timer_script1.PauseTimer();
        ultimate_timer_script2.PauseTimer();
        pv.RPC("PauseGameNetwork", RpcTarget.AllBuffered);
    }

    public void ResumeGame()
    {
        
        pv.RPC("ResumeGameNetwork", RpcTarget.AllBuffered);
    }


    public void IncreasTime()
    {
        //Debug.Log("Time Increased by: " + increaseTimeInput.text);
        float newIncrease = increaseTimeInput;
        pv.RPC("IncreasTimeNetwork", RpcTarget.AllBuffered, newIncrease);
    }


    [PunRPC]
    public void StartGameNetwork()
    {
        ultimate_timer_script1.StartTimer();
        ultimate_timer_script2.StartTimer();
    }
    [PunRPC]
    public void PauseGameNetwork()
    {
        ultimate_timer_script1.PauseTimer();
        ultimate_timer_script2.PauseTimer();
    }

    [PunRPC]
    public void ResumeGameNetwork()
    {
        ultimate_timer_script1.ResumeTimer();
        ultimate_timer_script2.ResumeTimer();
    }

    [PunRPC]
    public void IncreasTimeNetwork(float a)
    {
        ultimate_timer_script1.IncreaseTimer(a);
        ultimate_timer_script2.IncreaseTimer(a);
    }

}
