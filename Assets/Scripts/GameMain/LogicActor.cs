using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LogicActor
{
    public Actor viewActor;

    public int ActorId = 0;

    public Vector2Int LastPos;
    public Vector2Int Pos;
    public int Rotate;

    public Vector2Int Volocity;
    public int AnimateIdx;
    public int AnimateDict;//保存着各帧长度 用来模拟动画

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

    public void Update(int dtime)
    {
        LastPos = Pos;
        Pos = LastPos + new Vector2Int(Volocity.x * dtime  * 2, Volocity.y * dtime  * 2);

        Volocity = Vector2Int.zero;
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
