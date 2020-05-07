using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LogicActor
{
    public Actor viewActor;

    public int ActorId = 0;

    public VInt2 LastPos;
    public VInt2 Pos;
    public int Rotate;

    public int speed = 2; //格 每秒
    public VInt2 Volocity;
    public int AnimateIdx;
    public int AnimateDict;//保存着各帧长度 用来模拟动画

    public enum eState
    {
        IDLE,
        MOVE,
    }

    public eState state;
    
    public VInt2 LogicDiff;

    private LogicManager mgr;
    public LogicActor()
    {
        mgr = GameMain.GetInstance().logicManager;
        ActorId = mgr.logicInstIdx ++;
        mgr.AddActor(this);
    }


    public void Start()
    {

    }

    public int GetLogicDTime()
    {
        return mgr.LastDtime;
    }

    private void SwitchState(eState newState)
    {
        if (newState == state)
        {
            return;
        }

    }

    public void Update(int dtime)
    {
        LastPos = Pos;
        //dtime 单位毫秒 speed 单位 多少（0.001）格 每（毫）秒
        Vector2 dir = new Vector2(Volocity.x,Volocity.y);
        VInt2 trueDir = (VInt2)(dir.normalized);
        LogicDiff = new VInt2(trueDir.x * dtime * speed / 1000, trueDir.y * dtime * speed / 1000); //new VInt2(Volocity.x * dtime * speed, Volocity.y * dtime * speed);

        if (Volocity.x == 0 && Volocity.y == 0)
        {
            SwitchState(eState.IDLE);
        }
        else
        {
            SwitchState(eState.MOVE);
        }
        //在这里 判断攻击打出来

        //Volocity * dtime
        Pos = LastPos + LogicDiff;

        Volocity = VInt2.zero;
        if(viewActor != null)
        {
            List<Actor> ll = viewActor.contactActors;
            for(int i = 0; i < ll.Count; i++)
            {
                LogicActor other = ll[i].bindLogicActor;
                Debug.Log("碰"+other.ActorId);
            }
        }
    }

    public bool isNoMove()
    {
        return LastPos.x == Pos.x && LastPos.y == Pos.y;
    }
}
