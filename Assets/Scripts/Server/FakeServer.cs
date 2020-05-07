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
        FakeSendMsg(initMsgBytes);
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

        FakeSendMsg(bytes);
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
        byteBuffer.AddInt(0);
        byte[] lenBytes = BitConverter.GetBytes(byteBuffer.bytes.Length);
        byte[] bytes = lenBytes.Concat(byteBuffer.bytes).ToArray();
        return bytes;
    }

   
    private void FakeSendMsg(byte[] toSend)
    {

        GameMain.GetInstance().netManager.srvConn.FakeRecvMsg(toSend);
    }

    public void FakeSendOpt(ByteBuffer buffer)
    {
        int start = 0;
        string jsonStr = buffer.GetString(start, ref start);
        FrameOpt opt = JsonConvert.DeserializeObject<FrameOpt>(jsonStr);

        int delay = UnityEngine.Random.Range(50,90);
        
        System.Timers.Timer timer = new System.Timers.Timer();

        timer.Interval = delay;
        timer.AutoReset = false;
        timer.Enabled = true;
        timer.Elapsed += (sender, e) =>
        {
            NowOpts.Add(opt);
        };
    }

}
