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

        FakeGetFrame();
    }

    public void TestConnect()
    {
        string host = "127.0.0.1";
        int port = 1234;
        srvConn.Connect(host, port);
    }

    //心跳
    public static ByteBuffer GetHeatBeatProtocol()
    {
        return new ByteBuffer();
    }

    private bool isInstantiated = false;
    private int fakeFrameIdx = 1;
    private float fakeSendTimer = 0;
    
    public byte[] GetInitMsg()
    {
        ByteBuffer byteBuffer = new ByteBuffer();
        byteBuffer.AddInt((int)eNetMsgType.SYS);
        byte[] lenBytes = BitConverter.GetBytes(byteBuffer.bytes.Length);
        byte[] bytes = lenBytes.Concat(byteBuffer.bytes).ToArray();
        return bytes;
    }
    public void FakeGetFrame()
    {
        if (!isInstantiated)
        {
            byte[] initMsgBytes = GetInitMsg();

            
            srvConn.FakeRecvMsg(initMsgBytes);

            isInstantiated = true;
        }

        fakeSendTimer += Time.deltaTime;
        if(fakeSendTimer < 0.1f)
        {
            return;
        }
        fakeSendTimer -= 0.1f;

        LogicFrame frame = new LogicFrame(fakeFrameIdx++);

        for(int i = 0; i < nowOpts.Count; i++)
        {
            frame.frameOpts.Add(nowOpts[i]);
        }
        nowOpts.Clear();
        //{
        //    FrameOpt opt = new FrameOpt();
        //    opt.optType = eOptType.MVOE;
        //    opt.actorId = 0;
        //    frame.frameOpts.Add(opt);
        //}
        
        string ret = JsonConvert.SerializeObject(frame);
        ByteBuffer byteBuffer = new ByteBuffer();
        byteBuffer.AddInt((int)eNetMsgType.FRAME);
        byteBuffer.AddString(ret);
        byte[] lenBytes = BitConverter.GetBytes(byteBuffer.bytes.Length);
        byte[] bytes = lenBytes.Concat(byteBuffer.bytes).ToArray();
        srvConn.FakeRecvMsg(bytes);
    }




    List<FrameOpt> nowOpts = new List<FrameOpt>();
    public void FakeSendOpts(List<FrameOpt> opts)
    {
        nowOpts.AddRange(opts);
    }

}
