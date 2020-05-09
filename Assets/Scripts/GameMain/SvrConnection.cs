using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using UnityEngine;

public class SvrConnection
{
    const int BUFFER_SIZE = 1024 * 50;


    
    private Socket socket;
    //Buff
    private byte[] readBuff = new byte[BUFFER_SIZE];
    private int buffCount = 0;
    //沾包分包
    private Int32 msgLength = 0;
    private byte[] lenBytes = new byte[sizeof(Int32)];

    //心跳时间
    public float lastTickTime = 0;
    public float heartBeatTime = 30;


    //消息分发
    public MsgDispatcher msgDist = new MsgDispatcher();
    ///状态
    public enum Status
    {
        None,
        Connected,
    };
    public Status status = Status.None;

    public void SendLoginReq()
    {
        ByteBuffer bytes = new ByteBuffer();
        bytes.AddInt(0);
        Send(bytes);
    }

    //连接服务端
    public bool Connect(string host, int port)
    {
        try
        {
            //socket
            socket = new Socket(AddressFamily.InterNetwork,
                      SocketType.Stream, ProtocolType.Tcp);
            //Connect
            socket.Connect(host, port);
            //BeginReceive
            socket.BeginReceive(readBuff, buffCount,
                      BUFFER_SIZE - buffCount, SocketFlags.None,
                      ReceiveCb, readBuff);
            Debug.Log("连接成功");
            //状态
            status = Status.Connected;
            return true;
        }
        catch (Exception e)
        {
            Debug.Log("连接失败:" + e.Message);
            return false;
        }
    }

    //关闭连接
    public bool Close()
    {
        try
        {
            socket.Close();
            return true;
        }
        catch (Exception e)
        {
            Debug.Log("关闭失败:" + e.Message);
            return false;
        }
    }


    private string eMessage = "";

    //接收回调
    private void ReceiveCb(IAsyncResult ar)
    {
        try
        {
            int count = socket.EndReceive(ar);
            buffCount = buffCount + count;
            ProcessData();
            socket.BeginReceive(readBuff, buffCount,
                     BUFFER_SIZE - buffCount, SocketFlags.None,
                     ReceiveCb, readBuff);
        }
        catch (Exception e)
        {
            Debug.Log("ReceiveCb失败:" + e.Message);
            status = Status.None;
            eMessage = e.Message;
        }
    }

    //消息处理
    private void ProcessData()
    {
        if (buffCount < sizeof(Int32))
            return;

        

        Array.Copy(readBuff, lenBytes, sizeof(Int32));
        msgLength = BitConverter.ToInt32(lenBytes, 0);
        if (buffCount < msgLength + sizeof(Int32))
            return;

        ByteBuffer buffer = new ByteBuffer(readBuff, sizeof(Int32), msgLength);

        NetMsgBase msg = ReadAndDecode(buffer);
        msgDist.msgList.Enqueue(msg);

        int count = buffCount - msgLength - sizeof(Int32);
        //使用循环队列避免无意义的复制开销
        Array.Copy(readBuff, sizeof(Int32) + msgLength, readBuff, 0, count);
        buffCount = count;
        if (buffCount > 0)
        {
            ProcessData();
        }
    }

    

    public bool Send(ByteBuffer byteBuffer)
    {
        Debug.Log(eMessage);

        if (status != Status.Connected)
        {
            Debug.LogError("[Connection]还没链接就发送数据是不好的");
            return true;
        }

        byte[] b = byteBuffer.bytes;
        byte[] length = BitConverter.GetBytes(b.Length);

        byte[] sendbuff = length.Concat(b).ToArray();


        socket.Send(sendbuff);
        //Debug.Log("发送消息 " + protocol.GetDesc());
        return true;
    }

    public bool Send(ByteBuffer byteBuffer, string cbName, MsgDispatcher.Delegate cb)
    {
        if (status != Status.Connected)
            return false;
        msgDist.AddOnceListener(cbName, cb);
        return Send(byteBuffer);
    }

    public bool Send(ByteBuffer byteBuffer, MsgDispatcher.Delegate cb)
    {
        //string cbName = protocol.GetName();
        return Send(byteBuffer, "", cb);
    }

    private NetMsgBase ReadAndDecode(ByteBuffer byteBuffer)
    {
        int start = 0;
        int msgType = byteBuffer.GetInt(start, ref start);
        if(msgType == (int)eNetMsgType.FRAME)
        {
            string str = byteBuffer.GetString(start, ref start);
            LogicFrame frame = JsonConvert.DeserializeObject<LogicFrame>(str);
            NetFrameMsg msg = new NetFrameMsg();
            msg.MsgType = (eNetMsgType)msgType;
            msg.frame = frame;
            return msg;
        }
        else
        {
            NetSysMsg msg = new NetSysMsg();
            msg.MsgType = (eNetMsgType)msgType;
            msg.localPid = byteBuffer.GetInt(start, ref start);
            return msg;
        }

    }

    public void FakeRecvMsg(byte[] bytes)
    {
        
        Array.Copy(bytes, 0, readBuff, buffCount, bytes.Length);

        int count = bytes.Length;
        buffCount = buffCount + count;
        ProcessData();
        
    }
    public void Update()
    {
        //消息
        msgDist.Update();
        //心跳
        //if (status == Status.Connected)
        //{
        //    if (Time.time - lastTickTime > heartBeatTime)
        //    {
        //        ProtocolBase protocol = NetMgr.GetHeatBeatProtocol();
        //        Send(protocol);
        //        lastTickTime = Time.time;
        //    }
        //}
    }
}
