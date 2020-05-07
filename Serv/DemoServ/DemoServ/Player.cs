using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace DemoServ
{
    [Serializable]
    public class PlayerData
    {
        
    }
    class Player
    {
        public int idx;
        public Conn conn;
        public PlayerData data;
        public Player(int idx, Conn conn)
        {
            this.idx = idx;
            this.conn = conn;
        }

        public void Send(ByteBuffer data)
        {
            if (conn == null)
            {
                Console.WriteLine("无连接");
                return;
            }

            ServNet.instance.Send(conn, data.bytes);
        }

        public bool Logout()
        {
            //事件处理，稍后实现
            //ServNet.instance.handlePlayerEvent.OnLogout(this);
            
            conn.player = null;
            //conn.Close();
            conn = null;
            return true;
        }
    }
}
