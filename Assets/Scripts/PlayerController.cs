using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController
{

    public InputModule inputModule;
    public LogicActor Pawn;

    public List<FrameOpt> CacheOpts = new List<FrameOpt>();
    public void Init()
    {
        inputModule = GameMain.GetInstance().inputModule;
    }
    // 在逻辑帧中更新
    // 指令需要缓存吗 ？ 缓存的结果是 
    public void Update()
    {
        if(Pawn == null)
        {
            return;
        }

        Vector2Int moveVec = Vector2Int.zero;
        if (inputModule.isA)
        {
            moveVec.x -= 1;
        }
        if (inputModule.isD)
        {
            moveVec.x += 1;
        }
        if (inputModule.isS)
        {
            moveVec.y -= 1;
        }
        if (inputModule.isW)
        {
            moveVec.y += 1;
        }
        if(moveVec.x == 0 && moveVec.y == 0)
        {
            return;
        }
        FrameOpt moveOpt = new FrameOpt();
        moveOpt.actorId = Pawn.ActorId;
        moveOpt.optType = eOptType.MVOE;
        moveOpt.optContent = moveVec.x + "," + moveVec.y;

        
        //Debug.Log("cache");
        CacheOpts.Add(moveOpt);

    }


    public List<FrameOpt> opts = new List<FrameOpt>();
    public List<FrameOpt> GetOpts()
    {
        List<FrameOpt> ret = CacheOpts;
        CacheOpts = new List<FrameOpt>();
        return ret;
    }


    public void ChangePawn(LogicActor newPawn)
    {
        this.Pawn = newPawn;
    }
}
