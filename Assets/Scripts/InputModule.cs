using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputModule
{

    public bool isW;
    public bool isA;
    public bool isS;
    public bool isD;

    
    // Update is called once per frame
    public void Update()
    {
        isW = false;
        isA = false;
        isS = false;
        isD = false;
        if (Input.GetKey(KeyCode.W))
        {
            isW = true;
        }
        if (Input.GetKey(KeyCode.A))
        {
            isA = true;
        }
        if (Input.GetKey(KeyCode.S))
        {
            isS = true;
        }
        if (Input.GetKey(KeyCode.D))
        {
            isD = true;
        }

        //if (Input.GetKeyUp(KeyCode.W))
        //{
        //    isW = false;
        //}
        //if (Input.GetKeyUp(KeyCode.A))
        //{
        //    isA = false;
        //}
        //if (Input.GetKeyUp(KeyCode.S))
        //{
        //    isS = false;
        //}
        //if (Input.GetKeyUp(KeyCode.D))
        //{
        //    isD = false;
        //}


    }
}
