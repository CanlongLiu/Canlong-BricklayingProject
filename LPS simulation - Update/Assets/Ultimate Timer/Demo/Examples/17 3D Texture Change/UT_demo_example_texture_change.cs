using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UT_demo_example_texture_change : MonoBehaviour
{
    public GameObject sceneObj;
    public GameObject main_cam;
    public GameObject scene_cam;


    void OnEnable()
    {
        sceneObj.SetActive(true);
        main_cam.SetActive(false);
        scene_cam.SetActive(true);
    }

    void OnDisable()
    {
        if (sceneObj != null)
        {
            sceneObj.SetActive(false);
        }
        

        if (scene_cam != null)
        {
            scene_cam.SetActive(false);
        }

        if (main_cam != null)
        {
            main_cam.SetActive(true);
        }    
    }


}
