using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemoServ
{
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
}
