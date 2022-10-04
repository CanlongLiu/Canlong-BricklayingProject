using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Distance : MonoBehaviour
{
    [SerializeField]
    public GameObject object1;
    public GameObject object2;
    public Text distanceText;



    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        float distance = Vector3.Distance(object1.transform.position, object2.transform.position);
        distanceText.text = distance.ToString() + " meters";
    }
}
