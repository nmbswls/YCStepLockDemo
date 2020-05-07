using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using Newtonsoft.Json;

using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

//install-package Newtonsoft.Json 悉再
namespace DemoServ
{



    


    class ServNet
    {
        //监听嵌套字
        public Socket listenfd;
        //客户端链接
        public Conn[] conns;
        //最大链接数
        public int maxConn = 50;
        //单例
        public static ServNet instance;
        //主定时器
        System.Timers.Timer timer = new System.Timers.Timer(1000);
        //心跳时间
        //协议
        //消息分发

        public GameManager gameManager = new GameManager();


        public ServNet()
        {
            instance = this;
        }

        //获取链接池索引，返回负数表示获取失败
        public int NewIndex()
        {
            if (conns == null)
                return -1;
            for (int i = 0; i < conns.Length; i++)
            {
                if (conns[i] == null)
                {
                    conns[i] = new Conn();
                    return i;
                }
                else if (conns[i].isUse == false)
                {
                    return i;
                }
            }
            return -1;
        }

        //开启服务器
        public void Start(string host, int port)
        {
            //定时器
            timer.Elapsed += new System.Timers.ElapsedEventHandler(HandleMainTimer);
            timer.AutoReset = false;
            timer.Enabled = true;
            //链接池
            conns = new Conn[maxConn];
            for (int i = 0; i < maxConn; i++)
            {
                conns[i] = new Conn();
            }
            //Socket
            listenfd = new Socket(AddressFamily.InterNetwork,
                                  SocketType.Stream, ProtocolType.Tcp);
            //Bind
            IPAddress ipAdr = IPAddress.Parse(host);
            IPEndPoint ipEp = new IPEndPoint(ipAdr, port);
            listenfd.Bind(ipEp);
            //Listen
            listenfd.Listen(maxConn);
            //Accept
            listenfd.BeginAccept(AcceptCb, null);
            Console.WriteLine("[服务器]启动成功");
        }


        //Accept回调
        private void AcceptCb(IAsyncResult ar)
        {
            try
            {
                Socket socket = listenfd.EndAccept(ar);
                int index = NewIndex();

                if (index < 0)
                {
                    socket.Close();
                    Console.Write("[警告]链接已满");
                }
                else
                {
                    Conn conn = conns[index];
                    conn.Init(socket);
                    string adr = conn.GetAdress();
                    Console.WriteLine("客户端连接 [" + adr + "] conn池ID：" + index);
                    conn.socket.BeginReceive(conn.readBuff,
                                             conn.buffCount, conn.BuffRemain(),
                                             SocketFlags.None, ReceiveCb, conn);
                }
                listenfd.BeginAccept(AcceptCb, null);
            }
            catch (Exception e)
            {
                Console.WriteLine("AcceptCb失败:" + e.Message);
            }
        }

        //关闭
        public void Close()
        {
            for (int i = 0; i < conns.Length; i++)
            {
                Conn conn = conns[i];
                if (conn == null) continue;
                if (!conn.isUse) continue;
                lock (conn)
                {
                    conn.Close();
                }
            }
        }

        private void ReceiveCb(IAsyncResult ar)
        {
            Conn conn = (Conn)ar.AsyncState;
            lock (conn)
            {
                try
                {
                    int count = conn.socket.EndReceive(ar);
                    //关闭信号
                    if (count <= 0)
                    {
                        Console.WriteLine("收到 [" + conn.GetAdress() + "] 断开链接");
                        conn.Close();
                        return;
                    }
                    conn.buffCount += count;
                    ProcessData(conn);
                    //继续接收	
                    conn.socket.BeginReceive(conn.readBuff,
                                             conn.buffCount, conn.BuffRemain(),
                                             SocketFlags.None, ReceiveCb, conn);
                }
                catch (Exception e)
                {
                    Console.WriteLine("收到 [" + conn.GetAdress() + "] 断开链接 " + e.Message);
                    conn.Close();
                }
            }
        }

        private void ProcessData(Conn conn)
        {
            //小于长度字节
            if (conn.buffCount < sizeof(Int32))
            {
                return;
            }
            //消息长度
            Array.Copy(conn.readBuff, conn.lenBytes, sizeof(Int32));
            conn.msgLength = BitConverter.ToInt32(conn.lenBytes, 0);
            if (conn.buffCount < conn.msgLength + sizeof(Int32))
            {
                return;
            }

            ByteBuffer bytes = new ByteBuffer(conn.readBuff, sizeof(Int32), conn.msgLength);
            HandleMsg(conn, bytes);
            //清除已处理的消息
            int count = conn.buffCount - conn.msgLength - sizeof(Int32);
            Array.Copy(conn.readBuff, sizeof(Int32) + conn.msgLength, conn.readBuff, 0, count);
            conn.buffCount = count;
            if (conn.buffCount > 0)
            {
                ProcessData(conn);
            }
        }

        private void HandleMsg(Conn conn, ByteBuffer bytes)
        {
            if (conn.player == null)
            {
                int pidx = gameManager.AddPlayer(conn);
               
                Console.WriteLine("创建playter：" + pidx);
                ByteBuffer ret = new ByteBuffer();
                ret.AddInt((int)eNetMsgType.SYS);
                ret.AddInt(pidx);
                //Send(conn,ret.bytes);
            }
            else
            {
                int start = 0;
                
                string jsonStr = bytes.GetString(start, ref start);
                //Console.WriteLine("get opt" + jsonStr);
                FrameOpt opt = JsonConvert.DeserializeObject<FrameOpt>(jsonStr);
                gameManager.AddOpt(opt);
            }
            
        }

        //发送
        public void Send(Conn conn, byte[] bytes)
        {
            byte[] length = BitConverter.GetBytes(bytes.Length);
            byte[] sendbuff = length.Concat(bytes).ToArray();
            try
            {
                conn.socket.BeginSend(sendbuff, 0, sendbuff.Length, SocketFlags.None, null, null);
            }
            catch (Exception e)
            {
                Console.WriteLine("[发送消息]" + conn.GetAdress() + " : " + e.Message);
            }
        }

        //广播
        public void Broadcast(byte[] bytes)
        {
            for (int i = 0; i < conns.Length; i++)
            {
                if (!conns[i].isUse)
                    continue;
                if (conns[i].player == null)
                    continue;
                Send(conns[i], bytes);
            }
        }

        //private byte[] Decode(byte[] readbuff, int start, int length)
        //{
            
        //    byte[] bytes = new byte[length];
        //    Array.Copy(readbuff, start, bytes, 0, length);
        //    return bytes;
        //}


        //主定时器
        public void HandleMainTimer(object sender, System.Timers.ElapsedEventArgs e)
        {
            //处理心跳
            //Console.WriteLine("tik");
            if (!gameManager.isStartGame && gameManager.IsReady())
            {
                gameManager.StartGame();
            }
            timer.Start();
        }

        

        //打印信息
        public void Print()
        {
            Console.WriteLine("===服务器登录信息===");
            //for (int i = 0; i < conns.Length; i++)
            //{
            //    if (conns[i] == null)
            //        continue;
            //    if (!conns[i].isUse)
            //        continue;

            //    string str = "连接[" + conns[i].GetAdress() + "] ";
            //    if (conns[i].player != null)
            //        str += "玩家id " + conns[i].player.id;

            //    Console.WriteLine(str);
            //}
        }
    }
}
