using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LogicFrame
{
    public int frameIdx = 0;
    public List<FrameOpt> frameOpts = new List<FrameOpt>();
    public int dtime = 100;
    public LogicFrame()
    {

    }

    public LogicFrame(int frameIdx)
    {
        this.frameIdx = frameIdx;
    }
}

public enum eOptType
{
    MVOE = 0,
    ATTACK = 1,
    SKILL = 2,
}
public class FrameOpt
{
    public int actorId = 0;
    public eOptType optType;
    public string optContent;

    public string ToString()
    {
        return optType.ToString() + " " + optContent;
    }
}

public class LogicManager
{
    public int logicInstIdx = 0;

    public int frameIdx = 0;
    public int randomSeed = 0;

    //public int LogicTickRate = 10;
    public int LocalTickRate = 60;

    public float LogicLastTickTime = 0;
    public float LogicCurTickTime = 0;



    public int LastDtime;
    public BattleManager battleMgr = new BattleManager();

    public Dictionary<int, LogicActor> LogicActorMap = new Dictionary<int, LogicActor>();
    public List<LogicActor> LogicActorList = new List<LogicActor>();
    public Queue<LogicFrame> frames = new Queue<LogicFrame>();

    private float logicTimer = 0;
    private static float LogicTickInterval = 0;

    public static int MAX_LOGIC_FRAME_PER_TICK = 3; 


    public void Init(int localPid)
    {
        //get id
        battleMgr.Init(localPid);

    }

    public void Update()
    {
        logicTimer += Time.deltaTime;
        int logicCount = 0;
        //三倍速在视图层处理 逻辑层只管会不会卡死
        while (logicTimer >= LogicTickInterval && logicCount < MAX_LOGIC_FRAME_PER_TICK)
        {
            if(frames.Count > 0)
            {
                LogicFrame nowFrame = frames.Dequeue();
                LogicUpdate(nowFrame);
                LogicLastTickTime = LogicCurTickTime;
                LogicCurTickTime = Time.time;
                logicTimer -= LogicTickInterval;
                logicCount += 1;
            }
            else
            {
                break;
            }

        }
    }



    private void LogicUpdate(LogicFrame frame)
    {
        //handle frame
        if(frame.frameIdx != this.frameIdx + 1)
        {
            Debug.Log(this.frameIdx + "  " + frame.frameIdx);
            Debug.Log("zhenshu错误 丢弃");
            return;
        }
        
        this.frameIdx = frame.frameIdx;
        this.LastDtime = frame.dtime;

        //Debug.Log("opt shu:" + frame.frameOpts.Count);
        //handle input
        for(int i = 0; i < frame.frameOpts.Count; i++)
        {
            FrameOpt opt = frame.frameOpts[i];
            Handle(opt);
        }


        battleMgr.Update(frame.dtime);



        //执行所有update
        for (int i=0; i< LogicActorList.Count; i++)
        {
            LogicActorList[i].Update(frame.dtime);
        }

        //发送本机的命令
        //SendLocalOpt();

        SendAck();

    }


    public void SendAck()
    {
        ByteBuffer byteBuffer = new ByteBuffer();
        byteBuffer.AddInt(0);
        byteBuffer.AddInt(frameIdx);

        GameMain.GetInstance().netManager.Send(byteBuffer);
    }

    public void SendLocalOpt(FrameOpt opt)
    {

        ByteBuffer byteBuffer = new ByteBuffer();
        byteBuffer.AddInt(1);
        string jsonStr = JsonConvert.SerializeObject(opt);
        byteBuffer.AddString(jsonStr);

        GameMain.GetInstance().netManager.Send(byteBuffer);
    }
    public void Handle(FrameOpt opt)
    {
        if (opt == null)
        {
            Debug.Log("why null?");
        }
        //Debug.Log("handle:");
        switch (opt.optType)
        {
            case eOptType.MVOE:
                string optContent = opt.optContent;
                //Debug.Log(realOpt.Velocity);
                if(optContent != null)
                {
                    LogicActor target = GetActor(opt.actorId);
                    if(target != null)
                    {
                        string[] vString = optContent.Split(',');

                        target.Volocity = new VInt2(int.Parse(vString[0]),int.Parse(vString[1]));
                    }
                    
                }
                break;

            default:
                break;
        }
    }

    public void AddNewFrame(LogicFrame newFrame)
    {
         frames.Enqueue(newFrame);
        
    }

    public void AddActor(LogicActor logicActor)
    {
        LogicActorMap[logicActor.ActorId] = logicActor;
        LogicActorList.Add(logicActor);
    }

    public LogicActor GetActor(int id)
    {
        if (LogicActorMap.ContainsKey(id))
        {
            return LogicActorMap[id];
        }
        return null;
    }
}
