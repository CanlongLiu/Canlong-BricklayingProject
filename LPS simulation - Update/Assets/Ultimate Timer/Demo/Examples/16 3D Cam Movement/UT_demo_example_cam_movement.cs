using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UT_demo_example_cam_movement : MonoBehaviour
{
    public GameObject sceneObj;
    public GameObject Cam_Main;
    public GameObject Cam_1;
    public GameObject Cam_2;
    private ultimate_timer UTscript_1;
    private ultimate_timer UTscript_2;

    void OnEnable()
    {
        sceneObj.SetActive(true);
        Cam_Main.SetActive(false);
        Cam_2.SetActive(false);
        Cam_1.SetActive(true);
        UTscript_1 = Cam_1.GetComponent<ultimate_timer>();
        UTscript_2 = Cam_2.GetComponent<ultimate_timer>();
        UTscript_1.StartTimer();
    }

    void OnDisable()
    {
        if (sceneObj != null)
        {
            sceneObj.SetActive(false);
        }

        UTscript_1.ResetTimer();
        UTscript_2.ResetTimer();
        if (Cam_1 != null)
        {
            Cam_1.SetActive(false);
        }

        if (Cam_2 != null)
        {
            Cam_2.SetActive(false);
        }

        if (Cam_Main != null)
        {
            Cam_Main.SetActive(true);
        }    
    }


}
