using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Aim : MonoBehaviour
{ 
    public GameObject aimPoint;

    public float AimAreaSize;

    void Update()
    {
        if(Time.timeScale > 0)
        {
            Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            transform.position = ignoreZ(mousePosition);
            if(Input.GetMouseButtonDown(0))
            {
                GameObject AP = Instantiate(aimPoint,transform);
                AP.transform.parent = null;
            }
        }

    }


    public static Vector3 ignoreZ(Vector3 vector)
    {
        vector.z = 0;
        return vector;
    }

}
