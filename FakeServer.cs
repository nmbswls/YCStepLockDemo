using Newtonsoft.Json;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Timers;

public class FakeServer {


    private bool isStartGame = false;
    private int FrameIdx = 1;
    private float ServerRate = 15;
    private float TickInteval = 0;

    public static float delay = 50;

    public LinkedList<LogicFrame> FrameList = new LinkedList<LogicFrame>();
    public List<FrameOpt> NowOpts = new List<FrameOpt>();

    System.Timers.Timer MainTimer;

    private FakeServer()
    {
        TickInteval = 1000.0f / ServerRate ;

        MainTimer = new System.Timers.Timer();
        MainTimer.Interval = TickInteval;
        MainTimer.AutoReset = true;
        MainTimer.Elapsed += new ElapsedEventHandler(MainTick);



    }



    public void StartGame()
    {
        if (isStartGame)
        {
            return;
        }
        byte[] initMsgBytes = GetInitMsg();
        SendMsg(initMsgBytes);
        isStartGame = true;
        MainTimer.Start();

    }

    public void MainTick(object sender, ElapsedEventArgs e)
    {
        LogicFrame frame = new LogicFrame(FrameIdx++);

        for (int i = 0; i < NowOpts.Count; i++)
        {
            frame.frameOpts.Add(NowOpts[i]);
        }
        NowOpts.Clear();
        FrameList.AddLast(new LinkedListNode<LogicFrame>(frame));
        frame.dtime = (int)TickInteval;
        //LinkedListNode<LogicFrame> node = FrameList.Last;
        //Debug.Log("svr frame:" + frame.frameIdx);

        string ret = JsonConvert.SerializeObject(frame);
        ByteBuffer byteBuffer = new ByteBuffer();
        byteBuffer.AddInt((int)eNetMsgType.FRAME);
        byteBuffer.AddString(ret);
        byte[] lenBytes = BitConverter.GetBytes(byteBuffer.bytes.Length);
        byte[] bytes = lenBytes.Concat(byteBuffer.bytes).ToArray();

        SendMsg(bytes);
    }




    private static FakeServer Instance = new FakeServer();

    public static FakeServer GetInstance()
    {
        return Instance;
    }


    public byte[] GetInitMsg()
    {
        ByteBuffer byteBuffer = new ByteBuffer();
        byteBuffer.AddInt((int)eNetMsgType.SYS);
        byte[] lenBytes = BitConverter.GetBytes(byteBuffer.bytes.Length);
        byte[] bytes = lenBytes.Concat(byteBuffer.bytes).ToArray();
        return bytes;
    }

    public void SendMsg(byte[] toSend)
    {
        FakeSendMsg(toSend);
    }

    private void FakeSendMsg(byte[] toSend)
    {

        GameMain.GetInstance().netManager.srvConn.FakeRecvMsg(toSend);
    }

    public void FakeSendOpts(List<FrameOpt> opts)
    {
        System.Timers.Timer timer = new System.Timers.Timer();

        timer.Interval = delay;
        timer.AutoReset = false;
        timer.Enabled = true;
        timer.Elapsed += (sender, e) =>
        {

            //Debug.Log("tick");
            NowOpts.AddRange(opts);
        };
    }

}
