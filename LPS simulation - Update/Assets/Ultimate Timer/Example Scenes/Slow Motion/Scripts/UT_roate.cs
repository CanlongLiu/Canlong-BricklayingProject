using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UT_roate : MonoBehaviour
{
    public bool showRadius = true;
    public float radiusSize = 20f;
    public Color radiusColor = new Color32(255, 255, 102, 255);

    public float RotateX_Speed;
    public float RotateY_Speed = 1f;
    public float RotateZ_Speed;
    private float X_Speed;
    private float Y_Speed;
    private float Z_Speed;
    public bool rotX;
    public bool rotY = true;
    public bool rotZ;

    public bool useWorld = true;

    private void OnDrawGizmos()
    {
        if (showRadius)
        {
            Gizmos.color = radiusColor;
            Gizmos.DrawWireSphere(transform.position, radiusSize);
        }

    }


    void Update()
    {
        if (rotX)
        {
            X_Speed = RotateX_Speed * Time.deltaTime;
        }
        else
        {
            X_Speed = 0f;
        }

        if (rotY)
        {
            Y_Speed = RotateY_Speed * Time.deltaTime;
        }
        else
        {
            Y_Speed = 0f;
        }

        if (rotZ)
        {
            Z_Speed = RotateZ_Speed * Time.deltaTime;
        }
        else
        {
            Z_Speed = 0f;
        }

        if (useWorld)
        {
            transform.Rotate(X_Speed, Y_Speed, Z_Speed, Space.World);
        }
        else
        {
            transform.Rotate(X_Speed, Y_Speed, Z_Speed, Space.Self);
        }

    }
}
