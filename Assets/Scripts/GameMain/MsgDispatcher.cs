using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using UnityEngine;

public class NetMsgBase{
    public eNetMsgType MsgType;
}

public class NetFrameMsg : NetMsgBase
{
    public LogicFrame frame;
}

public enum eNetMsgType
{
    SYS = 0,
    FRAME,
    CHAT
}

public class MsgDispatcher
{
    public int maxMsgPerFrame = 15;

    public ConcurrentQueue<NetMsgBase> msgList = new ConcurrentQueue<NetMsgBase>();
    //委托类型
    public delegate void Delegate(NetMsgBase msg);
    //事件监听表
    private Dictionary<string, Delegate> eventDict = new Dictionary<string, Delegate>();
    private Dictionary<string, Delegate> onceDict = new Dictionary<string, Delegate>();

    //Update
    public void Update()
    {
        for (int i = 0; i < maxMsgPerFrame; i++)
        {
            if (msgList.Count > 0)
            {
                NetMsgBase msg;
                bool ret = msgList.TryDequeue(out msg);
                if (ret)
                {
                    DispatchMsgEvent(msg);
                }
            }
            else
            {
                break;
            }
        }
    }

    //消息分发
    public void DispatchMsgEvent(NetMsgBase msg)
    {
        if(msg.MsgType == eNetMsgType.FRAME)
        {
            NetFrameMsg realMSg = (NetFrameMsg)msg;
            if(realMSg == null)
            {
                Debug.Log("协议 error");
                return;
            }
            GameMain.GetInstance().logicManager.AddNewFrame(realMSg.frame);
        }
        else if(msg.MsgType == eNetMsgType.SYS)
        {
            GameMain.GetInstance().logicManager.Init();
        }
        //string name = protocol.GetName();
        //Debug.Log("分发处理消息 " + name);
        //if (eventDict.ContainsKey(name))
        //{
        //    eventDict[name](protocol);
        //}
        //if (onceDict.ContainsKey(name))
        //{
        //    onceDict[name](protocol);
        //    onceDict[name] = null;
        //    onceDict.Remove(name);
        //}


    }



    //添加事件监听 
    public void AddListener(string name, Delegate cb)
    {
        if (eventDict.ContainsKey(name))
            eventDict[name] += cb;
        else
            eventDict[name] = cb;
    }

    //添加单次监听事件
    public void AddOnceListener(string name, Delegate cb)
    {
        if (onceDict.ContainsKey(name))
            onceDict[name] += cb;
        else
            onceDict[name] = cb;
    }

    //删除监听事件
    public void DelListener(string name, Delegate cb)
    {
        if (eventDict.ContainsKey(name))
        {
            eventDict[name] -= cb;
            if (eventDict[name] == null)
                eventDict.Remove(name);
        }
    }

    //删除单次监听事件
    public void DelOnceListener(string name, Delegate cb)
    {
        if (onceDict.ContainsKey(name))
        {
            onceDict[name] -= cb;
            if (onceDict[name] == null)
                onceDict.Remove(name);
        }
    }
}
