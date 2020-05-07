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
            GameMain.GetInstance().netManager.TestConnect();
            GameMain.GetInstance().netManager.SendLoginReq();
            //FakeServer.GetInstance().StartGame();
        });
    }

	// Update is called once per frame
	void Update () {
		
	}
}
