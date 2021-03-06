﻿using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainPanel : MonoBehaviour
{


    public Button StartBtn;

	// Use this for initialization
	void Start () {
        BindView();
	}


    void BindView()
    {
        StartBtn = transform.Find("Button").GetComponent<Button>();
        StartBtn.onClick.AddListener(delegate()
        {
            if (NetManager.USE_FAKE_SERVER)
            {
                FakeServer.GetInstance().StartGame();
            }else{
                GameMain.GetInstance().netManager.TestConnect();
                GameMain.GetInstance().netManager.SendLoginReq();
            }
        });


        
    }

	// Update is called once per frame
	void Update () {
        if (Input.GetKeyDown(KeyCode.G))
        {
            //Debug.Log("get key down ");
            //ByteBuffer byteBuffer = new ByteBuffer();
            //FrameOpt opt = new FrameOpt();
            
            ////string jsonStr = JsonConvert.SerializeObject(opt);
            //byteBuffer.AddString("asd");
            
            //GameMain.GetInstance().netManager.srvConn.Send(byteBuffer);
            //Debug.Log("get key down finish");
            
            //GameMain.GetInstance().netManager.SendLoginReq();
        }
	}
}
