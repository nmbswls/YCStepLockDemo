using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;

namespace DemoServ
{

    public enum eNetMsgType
    {
        SYS = 0,
        FRAME,
        CHAT
    }

    class GameManager
    {

        public static int playerIdx = 0;

        public bool isStartGame = false;
        private int FrameIdx = 1;
        private float ServerRate = 15;
        private float TickInteval = 0;

        public static float delay = 50;

        public const int PlayerNum = 1;
        public List<Player> GamePlayerList = new List<Player>();


        public LinkedList<LogicFrame> FrameList = new LinkedList<LogicFrame>();
        public List<FrameOpt> NowOpts = new List<FrameOpt>();

        System.Timers.Timer MainTimer;

        public GameManager()
        {
            TickInteval = 1000.0f / ServerRate;

            MainTimer = new System.Timers.Timer();
            MainTimer.Interval = TickInteval;
            MainTimer.AutoReset = true;
            MainTimer.Elapsed += new ElapsedEventHandler(MainTick);
        }

        public int AddPlayer(Conn conn)
        {
            Player ply = new Player(nextPlayIdx(),conn);
            conn.player = ply;
            lock (GamePlayerList)
            {
                GamePlayerList.Add(ply);
            }
            
            return ply.idx;
        }

        public void AddOpt(FrameOpt opt)
        {
            lock (NowOpts)
            {
                NowOpts.Add(opt);
            }
        }

        private int nextPlayIdx()
        {
            int idx = Interlocked.Increment(ref playerIdx);
            return idx - 1;
        }

        public bool IsReady()
        {
            //Console.WriteLine("check" + GamePlayerList.Count + " " + PlayerNum);
            return GamePlayerList.Count == PlayerNum;
        }


        public void StartGame()
        {
            if (isStartGame)
            {
                return;
            }
            Console.WriteLine("Game Start");
            for(int i=0;i< GamePlayerList.Count; i++)
            {
                ByteBuffer initMsgBytes = GetInitMsg(GamePlayerList[i]);
                SendMsg(GamePlayerList[i], initMsgBytes);
            }
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
            
            Broadcast(byteBuffer);
        }

        public void Broadcast(ByteBuffer byteBuffer)
        {
            for (int i = 0; i < GamePlayerList.Count; i++)
            {
                Console.WriteLine("准备发送" + GamePlayerList[i].idx);
                SendMsg(GamePlayerList[i], byteBuffer);
            }
        }



        public ByteBuffer GetInitMsg(Player ply)
        {
            ByteBuffer byteBuffer = new ByteBuffer();
            byteBuffer.AddInt((int)eNetMsgType.SYS);
            byteBuffer.AddInt(ply.idx);

            return byteBuffer;
        }

        public void SendMsg(Player player, ByteBuffer toSend)
        {
            player.Send(toSend);
            //FakeSendMsg(toSend);

        }

        
    }
}
