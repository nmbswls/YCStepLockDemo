using UnityEngine;
using System;
using System.Collections;

public class GameMain : MonoBehaviour
{
	private static GameMain mInstance;

    public readonly NetManager netManager = new NetManager();
    public readonly LogicManager logicManager = new LogicManager();

    public ViewManager viewManager = new ViewManager();
    public PlayerController plyCtrl = new PlayerController();

    public InputModule inputModule = new InputModule();
	private void Awake()
	{
		Init();
        GameObject.DontDestroyOnLoad(this);
	}

	public void RunCoroutine(IEnumerator c){
		StartCoroutine (c);
	}

   

    public void Start(){
        Application.targetFrameRate = 60;
        plyCtrl.Init();
    }

	public void Update(){
        netManager.Update();
        logicManager.Update();
        inputModule.Update();
        plyCtrl.Update();
    }


	public void Init()
	{



		
	}

	public void Release()
	{
		mInstance = null;
	}


	public static GameMain GetInstance()
	{
		if (mInstance == null)
		{
			Type type = typeof(GameMain);
			GameMain gameMain = (GameMain)FindObjectOfType(type);
			mInstance = gameMain;
		}
		return mInstance;
	}



	public static bool HasInstance()
	{
		return mInstance != null;
	}


	

    public long GetTime(bool bflag = true)
    {
        TimeSpan ts = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0);
        long ret;
        if (bflag)
            ret = Convert.ToInt64(ts.TotalSeconds);
        else
            ret = Convert.ToInt64(ts.TotalMilliseconds);
        return ret;
    }
}
