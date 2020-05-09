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

    System.Timers.Timer MainTimer;


    public Dictionary<int, int> plyLastFrameInfo = new Dictionary<int, int>(); //判断玩家掉线
    public Dictionary<int, List<FrameOpt>> NowOpts = new Dictionary<int, List<FrameOpt>>();

    public bool isDiaoxian = false;

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

        NowOpts[0] = new List<FrameOpt>();
        plyLastFrameInfo[0] = 0;

        ByteBuffer initMsgBuf = GetInitMsg();
        FakeSendMsg(initMsgBuf);
        isStartGame = true;
        
        
        MainTimer.Start();



    }

    public void MainTick(object sender, ElapsedEventArgs e)
    {
        LogicFrame frame = new LogicFrame(FrameIdx++);

        foreach (var kv in plyLastFrameInfo)
        {
            if( FrameIdx - kv.Value > 50 ){

                //掉线
                isDiaoxian = true;

            }
            else
            {
                isDiaoxian = false;
            }
        }

        foreach (var kv in NowOpts)
        {
            if (kv.Value.Count == 0)
            {
                FrameOpt emptyOpt = new FrameOpt();
                emptyOpt.actorId = kv.Key;
                emptyOpt.optType = eOptType.MVOE;
                emptyOpt.optContent = "0,0";
                frame.frameOpts.Add(emptyOpt);

                if (isDiaoxian)
                {
                    //伪造回家包
                }
            }
            else
            {
                frame.frameOpts.Add(kv.Value[0]);
            }
            kv.Value.Clear();
        }

        FrameList.AddLast(new LinkedListNode<LogicFrame>(frame));
        frame.dtime = (int)TickInteval;
        
        //LinkedListNode<LogicFrame> node = FrameList.Last;
        //Debug.Log("svr frame:" + frame.frameIdx);
        
        string ret = JsonConvert.SerializeObject(frame);
        ByteBuffer byteBuffer = new ByteBuffer();
        byteBuffer.AddInt((int)eNetMsgType.FRAME);
        byteBuffer.AddString(ret);
        FakeSendMsg(byteBuffer);
    }




    private static FakeServer Instance = new FakeServer();

    public static FakeServer GetInstance()
    {
        return Instance;
    }


    public ByteBuffer GetInitMsg()
    {
        ByteBuffer byteBuffer = new ByteBuffer();
        byteBuffer.AddInt((int)eNetMsgType.SYS);
        byteBuffer.AddInt(0);

        return byteBuffer;
    }

   
    private void FakeSendMsg(ByteBuffer byteBuffer)
    {

        byte[] lenBytes = BitConverter.GetBytes(byteBuffer.bytes.Length);
        byte[] toSend = lenBytes.Concat(byteBuffer.bytes).ToArray();


        GameMain.GetInstance().netManager.srvConn.FakeRecvMsg(toSend);

        
    }

    public void FakeReceiveMsg(ByteBuffer buffer)
    {
        System.Timers.Timer timer = new System.Timers.Timer();

        timer.Interval = delay;
        timer.AutoReset = false;
        

        int start = 0;
        int type = buffer.GetInt(start, ref start);
        if (type == 0)
        {
            int frameIdx = buffer.GetInt(start, ref start);
            timer.Elapsed += (sender, e) =>
            {
                GetNewTick(frameIdx);
            };
        }
        else
        {
            string jsonStr = buffer.GetString(start, ref start);
            FrameOpt opt = JsonConvert.DeserializeObject<FrameOpt>(jsonStr);

            //Console.WriteLine("cnmcnm");

            timer.Elapsed += (sender, e) =>
            {
                GetNewOpt(opt);
            };
        }
        timer.Enabled = true;
    }

    private void GetNewOpt(FrameOpt opt)
    {
        List<FrameOpt> optList = NowOpts[0];

        lock (optList)
        {
            optList.Clear();
            optList.Add(opt);
        }
        
    }

    private void GetNewTick(int frameIdx)
    {
        plyLastFrameInfo[0] = frameIdx;
    }

}
