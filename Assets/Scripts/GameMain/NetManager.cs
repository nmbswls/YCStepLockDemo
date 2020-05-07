using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class NetManager
{
    public SvrConnection srvConn = new SvrConnection();
    public void Update()
    {
        srvConn.Update();

        //FakeGetFrame();
    }

    public void TestConnect()
    {
        string host = "127.0.0.1";
        int port = 1234;
        srvConn.Connect(host, port);
    }

    public void SendLoginReq()
    {
        srvConn.SendLoginReq();
    }

    //心跳
    public static ByteBuffer GetHeatBeatProtocol()
    {
        return new ByteBuffer();
    }

    

}
